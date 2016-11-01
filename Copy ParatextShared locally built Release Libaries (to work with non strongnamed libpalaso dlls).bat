@REM For this to work, you need Paratext as a sibling of this project
@echo Before using this, run BuildParatextSharedNoStrongName.bat (from Paratext)

copy /Y ..\Paratext\ParatextShared\bin\x86\Release\ParatextShared.dll lib\dotnet
copy /Y ..\Paratext\ParatextShared\bin\x86\Release\ParatextShared.pdb lib\dotnet

copy /Y ..\Paratext\ParatextShared\bin\x86\Release\Utilities.dll lib\dotnet
copy /Y ..\Paratext\ParatextShared\bin\x86\Release\Utilities.pdb lib\dotnet

copy /Y ..\Paratext\ParatextShared\bin\x86\Release\NetLoc.dll lib\dotnet
copy /Y ..\Paratext\ParatextShared\bin\x86\Release\NetLoc.pdb lib\dotnet

copy /Y ..\Paratext\ParatextShared\bin\x86\Release\NetLoc.XmlSerializers.dll lib\dotnet

@pause