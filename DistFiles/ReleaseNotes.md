# (Scroll down for Release Notes)

# Getting Started
To use HearThis™ to record audio Scriptures, you will need the following:

1. A default recording and playback device capable of high-quality audio. Most likely this will mean connecting a USB headset microphone. Reportedly some laptops can do an OK recording job using a microphone connected to the 1/8 inch (3.5 mm) microphone jack, so you can experiment with that if necessary. But it is not recommended to use a built-in microphone, as it is impossible to get adequate quality.
2. One of the following:
  - [Paratext](https://paratext.org/) 8 or later installed on the machine, with the project you want to record, or
  - a Digital Bible Library [Text Release Bundle](https://app.thedigitalbiblelibrary.org/static/docs/entryref/text/index.html) with some translated Scripture, or
  - a [Glyssen](https://software.sil.org/glyssen/) script exported from Glyssen
  - A local copy of Paratext data files for the project you want to record.
3. One or more speakers of the language to record the Scriptures. Once a project is selected and a few (optional) settings are selected to meet the needs of the project, HearThis is intended to be usable by people with minimal computer skills and relatively little training. Users should be taught to navigate to the text that is to be recorded, record each script item, check the recordings, and re-record as necessary. After they get the basics, please teach them to use the keyboard, not the mouse (which is inefficient for such a repetitive task). Please take note of their reading fluency; a large proportion of the intended audience will struggle to read naturally, and that's why the interface is designed to make it easy to listen and re-record, as many times as necessary. If you are going to use speakers who are not good readers, consider employing the "prompter technique" that [International Media Services](https://www.internationalmediaservices.org/) encourages. (This technique is described briefly in the appendix of [A Media Tool for Translation and Beyond](https://scripture-engagement.org/wp-content/uploads/2020/09/Doll-M-Limmer-J-2011-Media-Tool-for-Translation-and-Beyond.pdf).)
4. If you have a question, suggestion, or encouragement for us or our backers, please visit our [web forum](https://community.scripture.software.sil.org/c/hearthis). If you have a bug report, you can get that into our "to do" list directly by emailing <hearthis_issues@sil.org>.

# What to Back Up
HearThis stores a separate WAV file for each script line, indefinitely.
When you "Export", it gathers these up, joins them into chapters, and then
converts them to the format you choose.   The individual WAV files are
stored in the Program Data folder, under the SIL\HearThis directory.  For
example, on Windows 7, Genesis chapter 1 of the Good News UK would be stored
here: <a href="file:///C:/ProgramData/SIL/HearThis/GNTUK/Genesis/1">
C:\ProgramData\SIL\HearThis\GNTUK\Genesis\1</a>. This location needs to be
backed up (a future version of MyWorkSafe will do that automatically). If for
some reason you need to get at these files, for example to delete them or run
some audio cleanup process over them, feel free to do so, but be careful. A
good backup is recommended before any potentially destructive operations.

#Notes on MegaVoice

MegaVoice uses small "tag" recordings to help the user navigate by audio.
This version of HearThis does not yet generate those files.  MegaVoice also
requires a particular bit/rate for its wav files (16bit, 44.1k, mono). HearThis does convert
the recorded files to that format, if necessary.

-----------------------------

# Release Notes

## _VERSION_ (_DATE_)
- Improvements related to opening a clip for editing in an external program.
- Updated localizations.

## 3.5.1 (February 2025)
- Allowed HearThis desktop to attempt Android synchronization when using wired network connection.
- Security patch.

## 3.4.2 (June 2024) Note: This was intended to have been released as version 3.5.0
- Added support for exporting OGG Opus audio files and targeting Kulumi devices.
- Added ability in Check for Problems Mode to open a clip for editing in an external program

## 3.4.0 (January 2024)
- Added ability to specify verse ranges that will be broken out by verse instead of by sentence.

## 3.3.1 (15 March 2023)
- Added Indonesian localization.

## 3.3.0 (25 January 2023)
- Update for use with HearThis Android 1.0.

## 3.2.0 (21 July 2022)
- Added ability to use whitespace characters as sentence delimiters (for scriptio continua languages).

## 3.1.3 (3 June 2022)
- Added Chinese (Simplified) localization.

## 3.1.0 (29 April 2022)
- Added a "Check for problems" mode to allow a user to see where the text has changed since a clip was recorded.

## 3.0.18 (23 March 2022)
- Major bug fix to Record in Parts so that recordings are not garbled.
- HearThis now just warns the user about not having a recording device, but does not exit.

## 3.0.8 (17 August 2021)
- Portuguese localization is mostly complete. (Thanks, Angie!)

## 3.0.4 (6 January 2021)
- **IMPORTANT**: This release includes a data migration that will re-align clips with the text for any places where the previous release might have caused them to get out of sync. The migration will be performed automatically when a project is opened and should be allowed to run to completion without interrupting it. For most projects it will be fairly fast and you will not notice it, but for large projects with many affected chapters, there might be a slight delay. As a precaution, if possible, we recommend making a backup of all files in C:\ProgramData\SIL\HearThis\ before starting HearThis as a safeguard against any unlikely problems that might occur during the migration. If your Scripture data does not use \r (Parallel passage reference), or any of the Introductory outline styles, you can disregard this, as the migration will not make any changes to your HearThis data files.
- Fixes an intermittent lock-up when navigating in HearThis after playing back a recorded clip.

## 12 November 2020
- Made it possible to record introductory tables of contents and parallel passage references (skipped by default).

## 3 August 2020
- Switched localization to use XLIFF format instead of TMX. More (albeit minimal) UI languages available. Translations can be done via crowdin.com.

## 3 February 2020
- HearThis 2.1 and later requires [.NET 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework) or greater and a 64-bit versions of Windows. It cannot be installed or run on 32-bit versions of Windows.
- Otherwise, this version is functionally identical to the previous 2.0 release, but it lays some important groundwork towards future support of Paratext 9.1.

## 15 October 2019
- When recording or moving the mouse over a record button, the context is muted to prevent accidental recording of the context instead of the intended block.
- Added an advanced administrator feature to allow existing clips to be shifted forward or backward to re-align them with their corresponding blocks (useful when a change to the text or program settings results in the insertion, deletion, splitting, or combining of blocks).

## 3 July 2019
- Critical performance improvement, especially affecting "Record in Parts" dialog box.

## 18 March 2019
- Increased maximum clip recording time to 10 minutes.

## 12 February 2019
- Added localized strings for French.

## 24 April 2018
- Added option to display book and chapter labels on navigation buttons. (The checkbox to turn this on is on the Interface page of the Settings dialog box.)

## 20 April 2018
- Allowed different projects to use different settings to control how the text is broken into recording blocks.

## 9 April 2018
- Added support for latest version of Glyssen script files.

## 27 February 2018
- Update to work with latest version of Paratext 8 (and later) project data.

## 3 January 2018
- Fixed bug to add support for transliterated text (in \tl field).

## HearThis 2.0
- Dramatic Reading support. You can now do dramatized recordings. Use [Glyssen](https://software.sil.org/glyssen/) to prepare a script based on your Paratext project, then export a "Glyssen Script". Double-click on that file to open it with HearThis. Each "actor" can then select their name, see the acting roles they have been given, and select one. HearThis will then walk them through recording the script for that role.
- You can now save and merge-in “HearThis Packs”, so that multiple computers can be used to do the recordings, then brought back together.
- You can now select a high contrast color scheme in Settings:Interface. This is useful for presenting with projectors.
- We've cleaned up the normal color scheme a bit and given it a higher contrast than before.
- There has always been an icon that shows you what device HearThis is listening to. We've added new icons to make that clearer, including a "warning"-looking one if HearThis detects that you are using a laptop's built-in microphone.
- If you click the device icon, HearThis will now open the Windows Control Panel that lets you change to a different default recording device.
- To use with Paratext, you must have Paratext 8 or later.


## February 2017
Fixed bug that cause HearThis to crash when not connected to the Internet.

## January 2017
Added support for recording passages of poetry in which you want to separate text at paragraph markers, instead of just punctuation. See Settings:Punctuation.

## 1.5 November 2016
Added support for Paratext 8 projects.

## 1.4 September 2016
Increased time we wait for external audio merger/converters to finish, from 1 minute to 10 minutes.

## 1.3 May 2016
* Right to left languages now align to the right in the main window. HearThis gets this setting from Paratext.

## 1.2 February 2016

* New "Record Line in Parts" dialog allows readers who are having trouble with a line to split a long line into two separate recordings.
* Now remembers window size and position between runs.
* Fixed: Now remembers font size
* The shortcut key for "play" has been changed from "." or "Enter" to only TAB. Removing "." is motivated by user reports of accidentally pressing "space" (record) when they meant to press "." (play). Removing "Enter" is needed to keep the normal Windows convention of "Enter" meaning "accept and close" on dialogs. If you try to use ".", you get a reminder that it has moved to "TAB".

## 1.1 9 July 2015
* HearThis project can now be based on a <a href="https://digitalbiblelibrary.org/static/docs/entryref/text/index.html">
text release bundle</a> rather than a Paratext project.
* You can also now just point at a Paratext Project folder and have HearThis use it, even if Paratext is not installed.
* Therefore HearThis can now run with no Paratext installed.

## 1.0.10 7 November 2014
* Added capability to produce phrase-level Audacity Label Files for Scripture App Builder.
* Fixed bug for breaking first-level quotes into separate blocks.
* It is now possible to specify additional characters that HearThis will use to break text into blocks.
* Handling of vertical bar in a \w field.

## 1.0 19 August 2014
* When you start HearThis, it now attempts to go back to where you were last working (instead of the beginning of Genesis).
* Expanded script support by adding all known sentence-ending punctuation. When replacing chevrons with quotes, HearThis now uses the quotation mark system from Paratext settings.
* Added option to allow automatic breaking of quotations into separate blocks to facilitate simple multi-voice recording scenario.
* Added option to generate Audacity Label Files for use with Scripture App Builder and Audacity.
* Use Paratext project versification information.
* Added a way to skip a block and indicate that it would not be recorded.
* Made it possible to indicate types of paragraphs that should always be omitted.
* Enabled stopping playback of recordings
* Numerous user-interface improvements and bug fixes

## 0.54 30 August 2013
* Separate Devanagari lines by the danda (।) and double danda (॥) characters (Unicode codepoints 0964 and 0965, respectively).

## 0.53 31 July 2013
* Fix: Will no longer boost the microphone level to 100% upon each press of record
* Change under review: Will no longer display \io1, \io2, \io3
* Fix: Using a new text renderer that will hopefully support Telugu script.

## 0.5 11 May 2013
* Support orthographies which have contexts in which normally sentence-ending punctuation marks don't end it, e.g. "?This is a question? and !This is an exclamation!". Also don't break on 1.2, 1:2, or "etc.," if a comma follows.

* Various UI Tweaks for clarity

* Improve accuracy of drawing progress bar and thumb and clicking in bar

* Make Clicking on a spot in the slider take you to that spot

* Fixed hotlink for downloading lame.exe (mp3 encoder)

* Don't include Table of Contents in script

* Restore prior recording after short record button press

* Remove Megavoice Envoy Micro option. Turns out the differences between the various Megavoice devices
are not significant (as far as we've discovered so far, anyway).

* Require mouse to be on the left half of the script area before brightening the context text.

## 0.5 10 May 2013
* Added a button to "give up" on recording a script line
* Added Bislama to the list of languages that can be used for localization

## 0.4 9 May 2013
* If you plug/unplug a USB mic while HearThis is running, it will now adjust automatically.
* Fixed a problem with /mt1

## 0.4 8 May 2013
* Now handles \c and \cl, which may alter what should be read.
* You can now click a button to turn on breaking at all 'pause' punctuation marks (,;:).
* Various fixes.

## 0.4 7 May 2013

* If Paratext is also running, it will stay in sync with the current HearThis script line.
* AudiBible Support (chapter-by-chapter only)
* Finished Localization work.

## 0.4 6 May 2013

* When you reach the end of a new chapter, you now get a message and a button that takes you to the next chapter.
* Centered text is now just indented, for easier reading.
* Start on making HearThis localizable (will do another round of work on this).


## 0.4 3 May 2013

* Minor UI Improvements
* Fixed New Version Downloader

## 0.4 2 May 2013

* Improved marker handling, now based on the Paratext stylesheet attached to the PT project (e.g. usfm.sty).
  * show if if style's \TextProperty contains all of: paragraph, publishable, & vernacular
  * but not if it is a note or fig

## 0.4 1 May 2013

* Fixed a problem with removed spaces when there was an inline marker.
* Context is now less visible, unless you mouse over it. Click anywhere in the script to lock in the visibility.

## 0.4 30 April 2013

* Now honors the default font as set in Paratext for the language.
* Now shows before and after context of the script line.
* Script area now adjusts to resizing the window
* Fixed problem of replacing '?' with '.'
* If the press of the space bar is too short, get a message


## 0.4 29 April 2013

* Fixed problem of the player holding on to some audio files so that you couldn't record over them.

## 0.4 14 March 2013

* Now auto-checks for updates
* refresh with current libraries, including .net (4.0)

## 0.4 November 2011

 - Improved book/chapter selection.
 - Visual indication of which books, chapters, and lines have been recorded.
 - Recording now is delayed for 1/2 second to avoid recording sound of keyboard press.  Wait for the record button to turn green before speaking.
 - Improved recording level meter.
 - Indicator for confirming which microphone HearThis is recording from.
 - Buttons to make text larger/smaller.
 - New "Sample" mode used if Paratext is not installed.
 - Visual overhaul.

## 0.3

 - Added publishing formatted for Saber, if LAME (mp3 encoder) is installed.
 - Added publishing formatted for MegaVoice (see notes below)
 - Added publishing to ogg format


## 0.2
 - Added publishing to mp3s, if LAME is installed.

## 0.1

 - Initial release. Includes publishing to FLAC format only.

