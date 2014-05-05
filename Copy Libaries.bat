REM For this to work, you need palaso as a sibling of this project
set palaso=libpalaso
if NOT EXIST ..\%palaso% set palaso=palaso

copy /Y ..\%palaso%\output\debug\palaso.dll lib\dotnet\
copy /Y ..\%palaso%\output\debug\palaso.xml lib\dotnet\
copy /Y ..\%palaso%\output\debug\palaso.pdb lib\dotnet\

copy /Y ..\%palaso%\output\debug\palasouiwindowsforms.dll  lib\dotnet
copy /Y ..\%palaso%\output\debug\palasouiwindowsforms.xml  lib\dotnet
copy /Y ..\%palaso%\output\debug\palasouiwindowsforms.pdb  lib\dotnet

copy /Y ..\%palaso%\output\debug\palaso.Media.dll  lib\dotnet
copy /Y ..\%palaso%\output\debug\palaso.Media.xml  lib\dotnet
copy /Y ..\%palaso%\output\debug\palaso.Media.pdb  lib\dotnet

copy /Y ..\%palaso%\output\debug\palaso.testutilities.dll lib\dotnet
copy /Y ..\%palaso%\output\debug\palaso.testutilities.xml lib\dotnet
copy /Y ..\%palaso%\output\debug\palaso.testutilities.pdb dotnet

copy /Y ..\%palaso%\output\debug\palaso.*  output\debug
copy /Y ..\%palaso%\output\debug\palaso.testutilities.*  output\debug
copy /Y ..\%palaso%\output\debug\palasouiwindowsforms.*  output\debug
copy /Y ..\%palaso%\output\debug\palaso.Media.*  output\debug

pause