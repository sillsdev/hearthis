@REM For this to work, you need Paratext 8 installed in the default location
@echo If you need to update to the latest build first, you might find it here: \\swd-build\ParatextBuilds_BetaRelease

@pause

copy /Y "\Program Files (x86)\Paratext 8\ParatextShared.dll"  lib\dotnet
copy /Y "\Program Files (x86)\Paratext 8\Utilities.dll" lib\dotnet
copy /Y "\Program Files (x86)\Paratext 8\NetLoc.dll" lib\dotnet
copy /Y "\Program Files (x86)\Paratext 8\NetLoc.XmlSerializers.dll" lib\dotnet

@pause