(function () {
    "use strict";

    var POLL_MS = 15000;

    var statusLabels = { Normal: "Normal", Low: "Low Stock", OutOfStock: "Out of Stock" };
    var statusClasses = { Normal: "normal", Low: "low", OutOfStock: "outofstock" };

    var els = {
        tagRack: document.getElementById("tagRack"),
        searchInput: document.getElementById("searchInput"),
        pageSizeSelect: document.getElementById("pageSizeSelect"),
        errorBanner: document.getElementById("errorBanner"),
        emptyState: document.getElementById("emptyState"),
        table: document.getElementById("itemsTable"),
        body: document.getElementById("itemsBody"),
        pager: document.getElementById("pager"),
        pagerLabel: document.getElementById("pagerLabel"),
        prevPage: document.getElementById("prevPage"),
        nextPage: document.getElementById("nextPage"),
        liveText: document.getElementById("liveText"),
        countAll: document.getElementById("countAll"),
        countNormal: document.getElementById("countNormal"),
        countLow: document.getElementById("countLow"),
        countOut: document.getElementById("countOut"),
    };

    var params = new URLSearchParams(window.location.search);
    var state = {
        q: params.get("q") || "",
        status: params.get("status") || "",
        sort: params.get("sort") || "quantity",
        dir: params.get("dir") || "asc",
        page: parseInt(params.get("page") || "1", 10) || 1,
        pageSize: parseInt(params.get("pageSize") || "50", 10) || 50,
    };

    var requestToken = 0;

    function debounce(fn, delay) {
        var timer = null;
        return function () {
            var args = arguments;
            clearTimeout(timer);
            timer = setTimeout(function () { fn.apply(null, args); }, delay);
        };
    }

    function syncUrl() {
        var next = new URLSearchParams();
        if (state.q) next.set("q", state.q);
        if (state.status) next.set("status", state.status);
        if (state.sort !== "quantity") next.set("sort", state.sort);
        if (state.dir !== "asc") next.set("dir", state.dir);
        if (state.page !== 1) next.set("page", String(state.page));
        if (state.pageSize !== 50) next.set("pageSize", String(state.pageSize));
        var qs = next.toString();
        history.replaceState(null, "", qs ? "?" + qs : window.location.pathname);
    }

    function syncControls() {
        els.searchInput.value = state.q;
        els.pageSizeSelect.value = String(state.pageSize);

        var tags = els.tagRack.querySelectorAll(".tag");
        for (var i = 0; i < tags.length; i++) {
            tags[i].classList.toggle("is-active", tags[i].dataset.status === state.status);
        }

        var sortButtons = els.table.querySelectorAll(".sort-button");
        for (var h = 0; h < sortButtons.length; h++) {
            var btn = sortButtons[h];
            var th = btn.closest("th");
            var isSorted = btn.dataset.sort === state.sort;
            btn.classList.toggle("is-sorted", isSorted);
            btn.classList.toggle("is-desc", isSorted && state.dir === "desc");
            th.setAttribute("aria-sort", isSorted ? (state.dir === "desc" ? "descending" : "ascending") : "none");
        }
    }

    function cell(text, extraClass) {
        var td = document.createElement("td");
        if (extraClass) td.className = extraClass;
        td.textContent = text;
        return td;
    }

    function renderRows(items) {
        els.body.innerHTML = "";
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var tr = document.createElement("tr");
            tr.appendChild(cell(item.serial, "mono"));
            tr.appendChild(cell(item.description));
            tr.appendChild(cell(String(item.quantity), "num mono"));
            tr.appendChild(cell(item.retailPrice.toFixed(2), "num mono"));

            var statusTd = document.createElement("td");
            var tag = document.createElement("span");
            var key = statusClasses[item.status] || "normal";
            tag.className = "status-tag status-tag--" + key;
            tag.textContent = statusLabels[item.status] || item.status;
            statusTd.appendChild(tag);
            tr.appendChild(statusTd);

            els.body.appendChild(tr);
        }
    }

    function renderPager(page) {
        els.pagerLabel.textContent = "Page " + page.page + " of " + page.pageCount +
            " · " + page.matchingCount + " item" + (page.matchingCount === 1 ? "" : "s");
        els.prevPage.disabled = page.page <= 1;
        els.nextPage.disabled = page.page >= page.pageCount;
    }

    function renderCounts(counts) {
        els.countAll.textContent = counts.total;
        els.countNormal.textContent = counts.normal;
        els.countLow.textContent = counts.low;
        els.countOut.textContent = counts.outOfStock;
    }

    function describeFilter() {
        var parts = [];
        if (state.q) parts.push('matching "' + state.q + '"');
        if (state.status) parts.push("in " + (statusLabels[state.status] || state.status));
        return parts.length ? " " + parts.join(" ") : "";
    }

    function clearFilters() {
        state.q = "";
        state.status = "";
        state.page = 1;
        load();
    }

    function load() {
        syncUrl();
        syncControls();

        var token = ++requestToken;
        var qs = new URLSearchParams({
            q: state.q,
            status: state.status,
            sort: state.sort,
            dir: state.dir,
            page: String(state.page),
            pageSize: String(state.pageSize),
        });

        fetch("/api/items?" + qs.toString())
            .then(function (res) { return res.json(); })
            .then(function (data) {
                if (token !== requestToken) return; // a newer request already landed

                if (data.counts.error) {
                    els.errorBanner.textContent = "Can't reach the database: " + data.counts.error;
                    els.errorBanner.hidden = false;
                } else {
                    els.errorBanner.hidden = true;
                    renderCounts(data.counts);
                }

                var page = data.page;
                if (page.error) {
                    els.errorBanner.textContent = "Can't reach the database: " + page.error;
                    els.errorBanner.hidden = false;
                    els.table.hidden = true;
                    els.pager.hidden = true;
                    els.emptyState.hidden = true;
                    return;
                }

                // Page drifted past the end (filter narrowed the result set) - snap back.
                if (page.page > page.pageCount && page.page > 1) {
                    state.page = page.pageCount;
                    load();
                    return;
                }

                if (page.items.length === 0) {
                    els.table.hidden = true;
                    els.pager.hidden = true;
                    els.emptyState.hidden = false;
                    els.emptyState.textContent = "No items" + describeFilter() + ". ";
                    if (state.q || state.status) {
                        var clearBtn = document.createElement("button");
                        clearBtn.type = "button";
                        clearBtn.className = "link-button";
                        clearBtn.textContent = "Clear filters";
                        clearBtn.addEventListener("click", clearFilters);
                        els.emptyState.appendChild(clearBtn);
                    }
                } else {
                    els.table.hidden = false;
                    els.pager.hidden = false;
                    els.emptyState.hidden = true;
                    renderRows(page.items);
                    renderPager(page);
                }

                els.liveText.textContent = "updated " + new Date().toLocaleTimeString();
            })
            .catch(function () {
                if (token !== requestToken) return;
                els.errorBanner.textContent = "Can't reach the dashboard's own server. Retrying on the next refresh.";
                els.errorBanner.hidden = false;
            });
    }

    els.searchInput.addEventListener("input", debounce(function () {
        state.q = els.searchInput.value;
        state.page = 1;
        load();
    }, 300));

    els.pageSizeSelect.addEventListener("change", function () {
        state.pageSize = parseInt(els.pageSizeSelect.value, 10) || 50;
        state.page = 1;
        load();
    });

    els.tagRack.addEventListener("click", function (e) {
        var tag = e.target.closest(".tag");
        if (!tag) return;
        state.status = tag.dataset.status;
        state.page = 1;
        load();
    });

    els.table.addEventListener("click", function (e) {
        var btn = e.target.closest(".sort-button");
        if (!btn) return;
        var key = btn.dataset.sort;
        if (state.sort === key) {
            state.dir = state.dir === "asc" ? "desc" : "asc";
        } else {
            state.sort = key;
            state.dir = "asc";
        }
        state.page = 1;
        load();
    });

    els.prevPage.addEventListener("click", function () {
        if (state.page > 1) { state.page--; load(); }
    });
    els.nextPage.addEventListener("click", function () {
        state.page++; load();
    });

    syncControls();

    setInterval(function () {
        if (document.visibilityState === "visible") load();
    }, POLL_MS);
    document.addEventListener("visibilitychange", function () {
        if (document.visibilityState === "visible") load();
    });

    // First load already happened server-side (Index.cshtml.cs) - re-fetch once on script
    // init anyway so the live indicator and URL state agree with what's on screen, and so
    // a page loaded with ?q=/&status= filters active reflects those in the tag rack/sort
    // headers immediately.
    load();
})();
