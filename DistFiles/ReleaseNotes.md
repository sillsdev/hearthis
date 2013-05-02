## 0.4 2 May 2013

* Improved marker handling, now based on the Paratext Stylesheet attached to the PT project (e.g. usfm.sty).
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

Thanks for participating in this experiment!
The point of this experiment is to answer this question: Can we empower non-technical users to do their own scripture recording if we build an application designed specifically for them?<p>
At the moment, we break that into the following parts:


1. What are the usability hurdles such an application would need to address?
2. What are the output quality issues such an application would need to address?
3. How much training is required for non-technical users?
4. What would it cost to build and deploy such an app (I'm answering that, internally).

### How to participate
1. Set up a headset microphone and (probably) an external A/D USB converter.
		Reportedly some laptops can do an OK recording job right out of the microphone
		jack, so if you can't locate an external convert, please do some experimenting
		anyways.</p>
2. Install Paratext, and get a project with some translated scripture on it.
		If you want to just use English, that's fine too. Currently SayMore hides most
		resource texts, but you are allowed to choose "GNTUK" if you have that resource
		installed.</p>
3. Either try it yourself, or if possible, get a non-technical native speaker to
record for a while in his/her own language. Teach them to find a text, record
each script item, check their recording, and re-record as necessary. After they
get the basics, please teach them to use the keyboard, not the mouse (which is
inefficient for such a repetitive task). Please take note of their reading
fluency; a large proportion of the intended audience will struggle to read
naturally, and that's why the interface is designed to make it easy to listen
and re-record, as many times as necessary.  So it would help to hear
results from people with a range of reading abilities.
4. mail to john_hatton@sil.org:
 - In what country did you do the test?
 - What language did you try?
 -  How long did it take to train yourself or someone else?
 - Are there any parts of HearThis that you particularly like?
 - Are there parts that you don't like? Parts that are difficult?
 - How would you characterize the quality of the resulting recordings?
 - If you have done recordings in more traditional ways, how would you characterize the two approaches?
 - What the pros/cons do you see?
 - If this were a completed product, would you recommend its use to your entity?


Note: Since this is an experiment, and not a polished product, there are dozens
of  rough edges and missing features.  I am trying to limit the app to
just what we need to answer the experiments questions.  Then, if we
determine that such an app would make a big difference, we can move into
production mode as programming resources become available.

Please report those problems which interfere with doing the experiment.
For example, if you notice that a final quote mark is missing, you only need to
report that if it seemed to be a roadblock for the reader.  If you notice
that the font doesn't match Paratext, but this isn't interfering with the
experiment, then don't bother reporting it at this time.

#Important features that are missing

 - The display of books and chapters is currently only a binary thing: gray if it
has at least one verse, otherwise light gray. What we would eventually want is
blue=has been recorded.  We would also like to get an indication if the
book/chapter in question is fully or partially translated, and fully or
partially recorded. There are other visual/interaction improvements that are
yearning to be done in this area.
 - There may need to be a control for choosing how big a chunk each script line should be: Paragraph? Verse? Sentence?  Currently, you always get a            sentence.
 - It would probably help to split out book introduction material (everything that comes before the first chapter) into it's own "chapter" group (e.g. chapter 0             would be the book intro). Currently, this material just shows up as part of chapter 1.
 - Some sort of review workflow, in which a reviewer can listen and mark which verses have problems and need to be re-recorded. Then there would be an easy way for the narrator to find those spots and re-record.
 - Auto record the audio "tag files" used by MegaVoice for navigation
 - Help with setting up and testing the recording equipment and levels.
 - While most of the script text comes from the Paratext files themselves, a few words, like "Chapter" are always in English.  There will need to be a editable small list of such words in the vernacular.

# Under the hood
HearThis stores a separate WAV file for each script line, indefinetly.
When you "Publish", it gathers these up, joins them into chapters, and then
converts them to the format you choose.   The individual WAV files are
stored in the Program Data folder, under the SIL\HearThis directory.  For
example, on Windows 7, Genesis chapter 1 of the Good News UK would be stored
here: <a href="file:///C:/ProgramData/SIL/HearThis/GNTUK/Genesis/1">
C:\ProgramData\SIL\HearThis\GNTUK\Genesis\1</a>.  This location needs to be
backed up (a future version of MyWorkSafe will do that automatically). If for
some reason you need to get at these files, for example to delete them or run
some audio cleanup process over them, feel free to do so.

#Notes on Publishing

To publish in mp3 format, you'll need to download something extra. MP3 is
patented, so we can't just include an encoder in HearThis without infringing on
patents in some countries. If you install "<a
	href="http://audacity.sourceforge.net/help/faq?s=install&amp;item=lame-mp3">LAME
for Audacity</a>", then HearThis will use it to create MP3s.  This format is also needed for publishing to Saber devices.

#Notes on MegaVoice

MegaVoice uses small "tag" recordings to help the user navigate by audio.
This version of HearThis does not yet generate those files.  MegaVoice also
requires a particular bit/rate for its wav files (16bit, 44.1k, mono).  HearThis does convert
the recorded files to that format, if necessary.
