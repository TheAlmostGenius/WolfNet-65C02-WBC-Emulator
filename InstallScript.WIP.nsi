!define NAME "WolfNet 65C02 WBC Emulator"
!define REGPATH_UNINSTSUBKEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${NAME}"
Name "${NAME}"
OutFile "Latest.exe"
Unicode True
RequestExecutionLevel Admin ; Request admin rights on WinVista+ (when UAC is turned on)
InstallDir "$ProgramFiles\$(^Name)"
InstallDirRegKey HKLM "${REGPATH_UNINSTSUBKEY}" "UninstallString"

!include LogicLib.nsh
!include Integration.nsh
!include MUI2.nsh

!define MUI_ICON ".\Emulator\icon.ico"
!define MUI_UNICON ${MUI_ICON}
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "Banner.bmp"
!define MUI_HEADERIMAGE_UNBITMAP ${MUI_HEADERIMAGE_BITMAP}
!define MUI_HEADERIMAGE_BITMAP_STRETCH AspectFitHeight
!define MUI_PAGE_HEADER_TEXT "WolfNet 65C02 WBC Emulator"
!define MUI_WELCOMEPAGE_TITLE "Welcome"
!define MUI_WELCOMEPAGE_TEXT "Thank you for using (or developing for) the WolfNet \
65C02 WorkBench Computer. Please accept and use this Emulator as thanks from us."
!define MUI_LICENSEPAGE_TEXT_TOP "Licensing"
!define MUI_DIRECTORYPAGE_TEXT_TOP "Choose a directory to install to."

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE LICENSE
!insertmacro MUI_PAGE_DIRECTORY
;!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
;!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

!macro EnsureAdminRights
  UserInfo::GetAccountType
  Pop $0
  ${If} $0 != "admin" ; Require admin rights on WinNT4+
    MessageBox MB_IconStop "Administrator rights required!"
    SetErrorLevel 740 ; ERROR_ELEVATION_REQUIRED
    Quit
  ${EndIf}
!macroend

Function .onInit
  SetShellVarContext All
  !insertmacro EnsureAdminRights
FunctionEnd

Function un.onInit
  SetShellVarContext All
  !insertmacro EnsureAdminRights
FunctionEnd

Section "Program files (Required)"
  SectionIn Ro

  SetOutPath $InstDir
  WriteUninstaller "$InstDir\Uninst.exe"
  WriteRegStr HKLM "${REGPATH_UNINSTSUBKEY}" "DisplayName" "${NAME}"
  WriteRegStr HKLM "${REGPATH_UNINSTSUBKEY}" "DisplayIcon" "$InstDir\Emulator.exe,0"
  WriteRegStr HKLM "${REGPATH_UNINSTSUBKEY}" "UninstallString" '"$InstDir\Uninst.exe"'
  WriteRegStr HKLM "${REGPATH_UNINSTSUBKEY}" "QuietUninstallString" '"$InstDir\Uninst.exe" /S'
  WriteRegDWORD HKLM "${REGPATH_UNINSTSUBKEY}" "NoModify" 1
  WriteRegDWORD HKLM "${REGPATH_UNINSTSUBKEY}" "NoRepair" 1

  File /r /x Emulator.application /x app.publish .\Emulator\bin\x86\Publish\*.dll
  File .\Emulator\bin\x86\Publish\*.exe
SectionEnd

Section "Start Menu shortcut"
  CreateShortcut /NoWorkingDir "$SMPrograms\${NAME}.lnk" "$InstDir\Emulator.exe"
SectionEnd

!macro DeleteFileOrAskAbort path
  ClearErrors
  Delete "${path}"
  IfErrors 0 +3
    MessageBox MB_ABORTRETRYIGNORE|MB_ICONSTOP 'Unable to delete "${path}"!' IDRETRY -3 IDIGNORE +2
    Abort "Aborted"
!macroend

Section -Uninstall
  !insertmacro DeleteFileOrAskAbort "$InstDir\MyApp.exe"
  Delete "$InstDir\Uninst.exe"
  RMDir "$InstDir"
  DeleteRegKey HKLM "${REGPATH_UNINSTSUBKEY}"

  ${UnpinShortcut} "$SMPrograms\${NAME}.lnk"
  Delete "$SMPrograms\${NAME}.lnk"
SectionEnd
