@REM For this to work, you need Paratext as a sibling of this project

xcopy /Y ..\Paratext\ParatextShared\bin\x86\Debug\ParatextShared.dll lib\dotnet\
xcopy /Y ..\Paratext\ParatextShared\bin\x86\Debug\ParatextShared.pdb lib\dotnet\

xcopy /Y ..\Paratext\ParatextShared\bin\x86\Debug\Utilities.dll lib\dotnet\
xcopy /Y ..\Paratext\ParatextShared\bin\x86\Debug\Utilities.pdb lib\dotnet\

xcopy /Y ..\Paratext\ParatextShared\bin\x86\Debug\NetLoc.dll lib\dotnet\
xcopy /Y ..\Paratext\ParatextShared\bin\x86\Debug\NetLoc.pdb lib\dotnet\

xcopy /Y "..\Paratext\ParatextShared\bin\x86\Debug\SIL.Core.dll" lib\dotnet\ParatextShared\
xcopy /Y "..\Paratext\ParatextShared\bin\x86\Debug\SIL.WritingSystems.dll" lib\dotnet\ParatextShared\
xcopy /Y "..\Paratext\ParatextShared\bin\x86\Debug\SIL.Windows.Forms.Keyboarding.dll" lib\dotnet\ParatextShared\

@pause