REM For this to work, you need libpalaso (or palaso) as a sibling of this project
set palaso=libpalaso
if NOT EXIST ..\%palaso% set palaso=palaso

@REM set palasoConfigToUseForHearThis=DebugStrongName
if "%palasoConfigToUseForHearThis%"=="" (
  if "%1"=="" (
    set palasoConfigToUseForHearThis=debug
  ) ELSE (
    set palasoConfigToUseForHearThis=%1
  )
) 

if NOT EXIST ..\%palaso%\output\%palasoConfigToUseForHearThis%\*.dll (
  @echo ..\%palaso%\output\%palasoConfigToUseForHearThis% is not a valid source of libpalaso DLLs
  goto end
)

copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Windows.Forms.DblBundle.dll  lib\dotnet
copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Windows.Forms.DblBundle.pdb  lib\dotnet

copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Windows.Forms.dll  lib\dotnet
copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Windows.Forms.pdb  lib\dotnet

copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Core.dll  lib\dotnet
copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Core.pdb  lib\dotnet

copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Scripture.dll  lib\dotnet
copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Scripture.pdb  lib\dotnet

copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.DblBundle.dll  lib\dotnet
copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.DblBundle.pdb  lib\dotnet

copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\L10NSharp.dll  lib\dotnet
copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\L10NSharp.pdb  lib\dotnet

copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Media.dll  lib\dotnet
copy /Y ..\%palaso%\output\%palasoConfigToUseForHearThis%\SIL.Media.pdb  lib\dotnet

:end
@pause
