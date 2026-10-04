; Inno Setup script for A10D-up (Baclig Minimart POS)
; Build the Release configuration first (msbuild _conv\_conv.csproj /p:Configuration=Release),
; then compile this script with ISCC.exe (Inno Setup 6).

#define MyAppName "A10D-up"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Baclig Minimart"
#define MyAppExeName "A10D-up.exe"
#define MyBuildDir "..\_conv\bin\Release"

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
SetupIconFile=..\_conv\Resources\AppLogo.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
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
; Ship everything from the Release build output except ClickOnce artifacts,
; PDB debug symbols, and XML IntelliSense docs (not needed at runtime).
Source: "{#MyBuildDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; \
    Excludes: "*.pdb,*.xml,*.application,*.exe.manifest,app.publish\*,app.publish"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

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
