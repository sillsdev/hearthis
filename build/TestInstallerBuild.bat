call "c:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"

pushd .
MSbuild /target:Build /property:teamcity_build_checkoutDir=..\ /property:BUILD_NUMBER="*.*.0.789"
MSbuild /target:Test;SignIfPossible;MakeDownloadPointers;ConvertReleaseNotesToHtml /property:teamcity_build_checkoutDir=..\ /property:BUILD_NUMBER="*.*.0.789"
popd
PAUSE
