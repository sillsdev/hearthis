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

Note that l10n.proj does not have the built-in smarts to restart if a package is missing, so if there is an error that looks like it could be caused by a missing package, run it a second time and it will probably succeed.

If you manually download the xlf files from Crowdin and want versions of them that do not contain entries for the untranslated strings, copy them into Downloaded_HearThis_xlf_files and run
```
python remove_needs_translation.py
```
Note that there is a small problem with this script in that it introduces a small change in the XML header (losing a namespace). Do NOT push that change into the repository.
