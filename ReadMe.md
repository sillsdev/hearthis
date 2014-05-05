# Users

You're in the wrong place. Head over to http://hearthis.palaso.org.

## Testers

Please see [Tips for Testing Palaso Software](https://docs.google.com/document/d/1dkp0edjJ8iqkrYeXdbQJcz3UicyilLR7GxMRIUAGb1E/edit)

# Developers

## Getting dependencies

1. Ensure you have something that can run bash scripts
1. Run build/get-dependencies.sh
1. Ensure you have [Nuget](http://nuget.codeplex.com/) installed
1. Building the solution should automatically pull down the nuget dependencies.

## RoadMap / Day-to-day progress

See the [HearThis Trello Board](https://trello.com/b/5ejUB2EF/hearthis)

## Continuous Build System

Each time code is checked in, an automatic build begins on our [TeamCity build server](http://build.palaso.org/project.html?projectId=project16&tab=projectOverview), running all the unit tests. Similarly, when there is a new version of some SayMore dependency (e.g. Palaso, LocalizationManager), that server automatically rebuilds SayMore . This automatic build doesn't publish a new installer, however. That kind of build is launched manually, by pressing a button on the TeamCity page.  This "publish" process builds SayMore , makes and installer, rsyncs it to the distribution server, and writes out a little bit of html which the [SayMore download page](http://SayMore.palaso.org/download/) then displays to the user.


