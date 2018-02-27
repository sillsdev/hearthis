@REM For this to work, you need Paratext as a sibling of this project
@echo Before using this, build Paratext release

xcopy /Y ..\Paratext\ParatextData\bin\x86\Release\ParatextData.dll lib\dotnet
xcopy /Y ..\Paratext\ParatextData\bin\x86\Release\ParatextData.pdb lib\dotnet

xcopy /Y "..\Paratext\ParatextData\bin\x86\Release\SIL.Core.dll" lib\dotnet\ParatextData
xcopy /Y "..\Paratext\ParatextData\bin\x86\Release\SIL.WritingSystems.dll" lib\dotnet\ParatextData
xcopy /Y "..\Paratext\ParatextData\bin\x86\Release\SIL.Scripture.dll" lib\dotnet\ParatextData

@pause