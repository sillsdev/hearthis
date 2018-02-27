@REM For this to work, you need Paratext 8 installed in the default location
@echo If you need to update to the latest build first, you might find it here: \\swd-build\ParatextBuilds_BetaRelease

@pause

xcopy /Y "\Program Files (x86)\Paratext 8\ParatextData.dll"  lib\dotnet\
xcopy /Y "\Program Files (x86)\Paratext 8\SIL.Core.dll" lib\dotnet\ParatextData\
xcopy /Y "\Program Files (x86)\Paratext 8\SIL.WritingSystems.dll" lib\dotnet\ParatextData\
xcopy /Y "\Program Files (x86)\Paratext 8\SIL.Scripture.dll" lib\dotnet\ParatextData\

@pause