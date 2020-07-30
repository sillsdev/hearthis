## HearThis Localization

### Updating Crowdin with source string changes - UPLOAD TO CROWDIN NOT YET ENABLED

All the strings that are internationalized in the HearThis project are uploaded to Crowdin in HearThis.en.xlf

The L10nSharp tool ExtractXliff is run on the project to get any updates to the source strings resulting in a new HearThis.en.xlf file.

Overcrowdin is used to upload this file to Crowdin. * NOT YET *

This process is run automatically by a GitHub action if the commit comment mentions any of 'localize, l10n, i18n, internationalize, spelling' * NOT YET *

Because HearThis does not (yet) use GitVersion, the version number is hard-coded in build.proj.
The hard-coded version of the l10n.proj file should be updated to match the new current version
whenever the crowdin sources are being regenerated.

It can also be run manually as follows:
```
dotnet tool install -g overcrowdin
set CROWDIN_HEARTHIS_KEY=TheApiKeyForTheHearThisProject
msbuild l10n.proj /t:UpdateCrowdin
```