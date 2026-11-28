; Inno Setup script for Baclig Minimart's app suite (Inventory POS + Remittancce +
; InfraMonitor + Launcher).
; Build the Release configuration of all four first:
;   msbuild _conv.sln /p:Configuration=Release
; then compile this script with ISCC.exe (Inno Setup 6).
;
; Installed layout keeps each app in its own subfolder under {app} - Launcher.exe discovers
; its sibling apps at {app}\<ProjectFolder>\<exe> (see LauncherForm.FindExePath), so don't
; flatten these into one folder.

#define MyAppName "Baclig Minimart"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Baclig Minimart"
#define MyLauncherExeName "Launcher.exe"
#define MyInventoryBuildDir "..\Inventory\bin\Release"
#define MyRemittanceBuildDir "..\Remittancce\bin\Release"
#define MyInfraMonitorBuildDir "..\InfraMonitor\bin\Release"
#define MyLauncherBuildDir "..\Launcher\bin\Release"
#define MyInventoryUpdaterBuildDir "..\InventoryUpdater\bin\Release"

[Setup]
; Fixed AppId so future versions upgrade in place instead of side-by-side installing.
AppId={{9F1B2C3D-4E5F-4A6B-8C7D-1A2B3C4D5E6F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=Output
OutputBaseFilename=AppInventory-Setup-{#MyAppVersion}
SetupIconFile=..\Inventory\Resources\AppLogo.ico
UninstallDisplayIcon={app}\Launcher\{#MyLauncherExeName}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; Ship everything from each project's Release build output except ClickOnce artifacts,
; PDB debug symbols, and XML IntelliSense docs (not needed at runtime). Each app keeps its
; own subfolder so Launcher.exe can find its siblings at {app}\<ProjectFolder>\<exe>.
Source: "{#MyInventoryBuildDir}\*"; DestDir: "{app}\Inventory"; Flags: ignoreversion recursesubdirs createallsubdirs; \
    Excludes: "*.pdb,*.xml,*.application,*.exe.manifest,app.publish\*,app.publish"
Source: "{#MyRemittanceBuildDir}\*"; DestDir: "{app}\Remittancce"; Flags: ignoreversion recursesubdirs createallsubdirs; \
    Excludes: "*.pdb,*.xml,*.application,*.exe.manifest,app.publish\*,app.publish"
Source: "{#MyInfraMonitorBuildDir}\*"; DestDir: "{app}\InfraMonitor"; Flags: ignoreversion recursesubdirs createallsubdirs; \
    Excludes: "*.pdb,*.xml,*.application,*.exe.manifest,app.publish\*,app.publish"
Source: "{#MyLauncherBuildDir}\*"; DestDir: "{app}\Launcher"; Flags: ignoreversion recursesubdirs createallsubdirs; \
    Excludes: "*.pdb,*.xml,*.application,*.exe.manifest,app.publish\*,app.publish"
; InventoryUpdater is a hidden helper used by Inventory's self-update feature (SelfUpdateService.cs) -
; never launched by a person, so it gets no Start Menu/Desktop icon below.
Source: "{#MyInventoryUpdaterBuildDir}\*"; DestDir: "{app}\InventoryUpdater"; Flags: ignoreversion recursesubdirs createallsubdirs; \
    Excludes: "*.pdb,*.xml"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\Launcher\{#MyLauncherExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\Launcher\{#MyLauncherExeName}"; Tasks: desktopicon

[Run]
; Inventory's self-update feature (SelfUpdateService.cs / InventoryUpdater.exe) rewrites files
; inside {app}\Inventory at runtime under whatever standard, non-admin Windows account the till
; is logged in as - Program Files is admin-write by default, so without this grant every silent
; install attempt would fail (logged, never shown to the cashier) instead of actually applying.
; Scoped to only this one subfolder - the other three apps keep the default admin-only ACL.
Filename: "{sys}\icacls.exe"; Parameters: "\"{app}\Inventory\" /grant *S-1-5-32-545:(OI)(CI)M /T"; Flags: runhidden
Filename: "{app}\Launcher\{#MyLauncherExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNet472OrNewer: Boolean;
var
  release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', release)
    and (release >= 461808); { 461808 = .NET Framework 4.7.2 }
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
  if not IsDotNet472OrNewer then
  begin
    if MsgBox('This application requires .NET Framework 4.7.2 or later, which was not detected on this computer.' + #13#10 +
      'Setup will continue, but the application may fail to run until it is installed.' + #13#10#13#10 +
      'Continue anyway?', mbConfirmation, MB_YESNO) = IDNO then
      Result := False;
  end;
end;
