Using HearThis™, communities can record and distribute their translated Scripture for themselves.

# Users

You're in the wrong place. Head over to https://software.sil.org/hearthis .

## Testers

Please see [Tips for Testing Palaso Software](https://docs.google.com/document/d/1dkp0edjJ8iqkrYeXdbQJcz3UicyilLR7GxMRIUAGb1E/edit)

# Developers

## IDE

- Download and install [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- Download and Install the NUnit 3 Test Adapter extension from within Visual Studio

## Building

1. Run build/get-dependencies-windows.sh (e.g. from within Git Bash)
1. Build the solution. This will download and install NuGet dependencies.
1. Assemblies not available on the build server or via nuget can be checked in to the repo in lib/dotnet. When working on the code, the development team needs to decide if any of these assemblies should be replaced with the latest version. The same is true for the myriad nuget packages that HearThis depends on.

## Paratext and ICU DLLs

Note that a Release build cannot be run directly (i.e., without properly installing HearThis) -- at least on a system that has Paratext -- because of the way Paratext attempts to locate the ICU DLLs. If you need to track down a bug that is only reproducible in a Release build, run the Installer and then replace the installed EXE with a locally-built one if necessary.

## Running Tests

1. Ensure NUnit 3 Test Adapter extension is installed (see above)
1. Test -> Run All Tests
The "BreakIntoChunks_SpeedTest" test is CPU dependent and is safe to ignore

## RoadMap / Day-to-day progress

See the [HearThis Trello Board](https://trello.com/b/5ejUB2EF/hearthis)

## Continuous Build System

Each time code is checked in, an automatic build begins on our [TeamCity build server](https://build.palaso.org/project.html?projectId=HearThis&tab=projectOverview), running all the unit tests. This automatic build doesn't publish a new installer, however. That kind of build is launched manually, by pressing a button on the TeamCity page.  This "publish" process builds HearThis, makes an installer, rsyncs it to the distribution server, and writes out a a json file that the downloads page reads so that it can display options to the user.

## Coding Standards

Please avoid making spurious white space changes. HearThis uses tabs, not spaces, for indentation.
