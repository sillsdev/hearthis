Rebuilding this requires ruby and https://github.com/chrisvire/BuildUpdate
Here's the command line I used:

<your path to buildupdate.rb>\buildupdate.rb -t HearThis_HearThisWinMasterContinuous -f getDependencies-windows.sh -r ..

Explanation:

"-t HearThis_HearThisWinDevContinuousPt8" points at the configuration that tracks this branch
"-f ____" gives what I want the file to be called
"-r .." takes care of moving the context up from build to the root directory