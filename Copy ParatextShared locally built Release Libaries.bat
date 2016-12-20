@REM For this to work, you need Paratext as a sibling of this project
@echo Before using this, build Paratext release

xcopy /Y ..\Paratext\ParatextShared\bin\x86\Release\ParatextShared.dll lib\dotnet
xcopy /Y ..\Paratext\ParatextShared\bin\x86\Release\ParatextShared.pdb lib\dotnet

xcopy /Y ..\Paratext\ParatextShared\bin\x86\Release\Utilities.dll lib\dotnet
xcopy /Y ..\Paratext\ParatextShared\bin\x86\Release\Utilities.pdb lib\dotnet

xcopy /Y ..\Paratext\ParatextShared\bin\x86\Release\NetLoc.dll lib\dotnet
xcopy /Y ..\Paratext\ParatextShared\bin\x86\Release\NetLoc.pdb lib\dotnet

xcopy /Y "..\Paratext\ParatextShared\bin\x86\Release\SIL.Core.dll" lib\dotnet\ParatextShared
xcopy /Y "..\Paratext\ParatextShared\bin\x86\Release\SIL.WritingSystems.dll" lib\dotnet\ParatextShared
xcopy /Y "..\Paratext\ParatextShared\bin\x86\Release\SIL.Windows.Forms.Keyboarding.dll" lib\dotnet\ParatextShared

@pause