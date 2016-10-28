@REM For this to work, you need Paratext as a sibling of this project

copy /Y ..\Paratext\ParatextShared\bin\x86\Debug\ParatextShared.dll lib\dotnet
copy /Y ..\Paratext\ParatextShared\bin\x86\Debug\ParatextShared.pdb lib\dotnet

copy /Y ..\Paratext\ParatextShared\bin\x86\Debug\Utilities.dll lib\dotnet
copy /Y ..\Paratext\ParatextShared\bin\x86\Debug\Utilities.pdb lib\dotnet

copy /Y ..\Paratext\ParatextShared\bin\x86\Debug\NetLoc.dll lib\dotnet
copy /Y ..\Paratext\ParatextShared\bin\x86\Debug\NetLoc.pdb lib\dotnet

copy /Y ..\Paratext\ParatextShared\bin\x86\Debug\NetLoc.XmlSerializers.dll lib\dotnet
copy /Y ..\Paratext\ParatextShared\bin\x86\Debug\NetLoc.XmlSerializers.pdb lib\dotnet

@pause