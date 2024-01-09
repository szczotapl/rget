[Setup]
; General settings for the installer
AppName=RivioxGet
AppVersion=1.4
DefaultDirName={pf}\rget 
OutputDir=Output 
OutputBaseFilename=rgetInstaller 
Compression=lzma 
SolidCompression=yes 
AppPublisherURL=https://riviox.is-a.dev/ 
AppSupportURL=https://riviox.is-a.dev/

[Files]
Source: "bin\*"; DestDir: "{app}"; Flags: ignoreversion

[Tasks]
Name: "AddToPath"; Description: "Add rget to the system PATH"

[Run]
Filename: "cmd.exe"; Parameters: "/c setx path ""%path%;{app}"""; Flags: runhidden; Tasks: AddToPath; \
   StatusMsg: "Adding rget to PATH..."; Description: "Adding rget to PATH...";
