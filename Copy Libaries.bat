REM For this to work, you need palaso as a sibling of this project
set palaso=libpalaso
if NOT EXIST ..\%palaso% set palaso=palaso

copy /Y ..\%palaso%\output\debug\SIL.Windows.Forms.DblBundle.dll  lib\dotnet
copy /Y ..\%palaso%\output\debug\SIL.Windows.Forms.DblBundle.pdb  lib\dotnet

copy /Y ..\%palaso%\output\debug\SIL.Windows.Forms.dll  lib\dotnet
copy /Y ..\%palaso%\output\debug\SIL.Windows.Forms.pdb  lib\dotnet

copy /Y ..\%palaso%\output\debug\SIL.Core.dll  lib\dotnet
copy /Y ..\%palaso%\output\debug\SIL.Core.pdb  lib\dotnet

copy /Y ..\%palaso%\output\debug\SIL.Scripture.dll  lib\dotnet
copy /Y ..\%palaso%\output\debug\SIL.Scripture.pdb  lib\dotnet

copy /Y ..\%palaso%\output\debug\SIL.DblBundle.dll  lib\dotnet
copy /Y ..\%palaso%\output\debug\SIL.DblBundle.pdb  lib\dotnet

copy /Y ..\%palaso%\output\debug\L10NSharp.dll  lib\dotnet
copy /Y ..\%palaso%\output\debug\L10NSharp.pdb  lib\dotnet

copy /Y ..\%palaso%\output\debug\SIL.Media.dll  lib\dotnet
copy /Y ..\%palaso%\output\debug\SIL.Media.pdb  lib\dotnet

pause