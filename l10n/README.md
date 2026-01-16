## HearThis Localization

### Updating Crowdin with source string changes - UPLOAD TO CROWDIN NOT YET ENABLED

All the strings that are internationalized in the HearThis project are uploaded to [Crowdin](https://crowdin.com/project/hearthis) in `HearThis.en.xlf`

The L10nSharp tool [`ExtractXliff`](https://github.com/sillsdev/l10nsharp/tree/master/src/ExtractXliff#extractxliff-tool) is run on the project to get any updates to the source strings resulting in a new `HearThis.en.xlf file`.

Overcrowdin is used to upload this file to Crowdin. * NOT YET *

This process is run automatically by a GitHub action if the commit comment mentions any of 'localize, l10n, i18n, internationalize, spelling' * NOT YET *

Because HearThis does not (yet) use GitVersion, the version number is hard-coded in [`build.proj`](../build/build.proj).
The hard-coded version of the [`l10n.proj`](l10n.proj) file should be updated to match the new current version
whenever the crowdin sources are being regenerated.

It can also be run manually as follows:
```
dotnet tool install -g overcrowdin
set CROWDIN_HEARTHIS_KEY=TheApiKeyForTheHearThisProject
msbuild l10n.proj /t:UpdateCrowdin
```

Note that `l10n.proj` does not have the built-in smarts to restart if a package is missing, so if there is an error that looks like it could be caused by a missing package, run it a second time and it will probably succeed.

If you manually download the xlf files from Crowdin and want versions of them that do not contain entries for the untranslated strings, copy them into Downloaded_HearThis_xlf_files and run
```
python remove_needs_translation.py
```
Note that there is a small problem with this script in that it introduces a small change in the XML header (losing a namespace). Do NOT push that change into the repository.

### Localizing the Sample Project

The `Sample` project included with HearThis can also be localized, so that a version in the target language is available for training. To generate a fully functional sample project in the localized language, two things need to be done:
 - [Localize](#localizing-sample-script-strings) the `Sample` strings
 - [Record](#recording-sample-clips) five short audio clips to be included with HearThis

#### Localizing Sample Script Strings
The strings that HearThis uses to generate a localized Sample project are available in [Crowdin](https://crowdin.com/project/hearthis) in `HearThis.en.xlf` along with all the other localizable UI text.

If you filter the strings to show only those with an ID containing `Sample.`, you will see:
 - **Error messages**
     > These should be localized even if you do not intend to have HearThis generate a localized `Sample` project
 - **Strings marked “Only for sample data”**
     > In Crowdin, these strings have a Comment that says *Only for sample data*. They are boilerplate strings that will be reused for the Sample project text. Translate them like any other UI text in HearThis.
 - Strings with IDs beginning with `Sample.RecordingText.`
     > These correspond to special passages in the Sample project that demonstrate the **Check for Problems** feature of HearThis. To make this work properly, you should both localize these strings and [produce corresponding audio clips](#recording-sample-clips).
    >
    > When localizing these strings, pay attention to the relationship between the English sample text and the corresponding clip. Specifically, where the English text includes intentional misspellings, punctuation mistakes, or incorrect use of homophones, it will be important to introduce similar issues in your translation so that the training examples remain useful. It is *not* necessary to reproduce the exact number or type of mistakes—only to provide analogous examples.

#### Recording Sample Clips

In Crowdin, there are several [sources](https://crowdin.com/project/hearthis/sources/files) that are WAV files. When you open one to localize it, the `Context & References` area shows a comment with the text of the source file.
 - Localize that text into the target language.
 - Record it as a WAV file in the same format as the original (mono, 44.1 kHz, 16-bit). You may use any recording software (e.g., Audacity).
 - Upload your recording into Crowdin as the translation of the source file.
