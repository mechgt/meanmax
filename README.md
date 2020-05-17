# Mean-Max
Mean-Max Plugin for SportTracks (Desktop)

Mean-max is a plugin to extend the sports anaylysis software SportTracks.  Perform advanced analysis using data captured from personal fitness devices (such as heartrate and other).

This software was open-sourced after SportTracks desktop was discontinued, and all restrictions have been removed.

### Concept:
The mean max chart is defined as this: The maximum average power/HR/cadence (Y-axis) that you could hold for a period of time (X-axis). This is from 1 peak instance in the activity, sort of like max HR: you may have hit your max HR only 1 time; likewise you may have only held this power/HR/cadence 1 time.  

The values charted here (power for instance) are also often called Critical Power.  Here you can find your '5 second power', or '20 minute power', etc. Sometimes people call this CP5s or CP20min for '5 second Critical Power' etc.  CP20 (or Critical Power over 20 minutes) is the max average power that you are able to sustain for 20 minutes as an example.  This data can be used for things like designing interval workouts.  All of the charts definitions in here also apply to heart rate and cadence as well as power, although the application to training may be different.

### Getting Started - Detail Display:
As mentioned above, this plugin will show you the maximum average output (power, heart rate, or cadence) you were able to maintain during a single or multiple activities.

This plugin adds an additional detail pane to the main activities display.  In reports view, you can select multiple activities from the reports list, and the Mean-Max will evaluate all selected activities into 1 chart and display the max overall values.  For instance, activity 1 may have the a 1 minute max value of 467 watts (as seen in the screenshot below), but the 30 second max of about 570 watts may have been found in another activity.  The idea is to show the maximum output you're capable of performing.

![mm_detail](https://mechgt.com/st/images/mm_detail.png)

Note the x-axis is a logarithmic base, instead of a linear base.  This can be seen because the first tic marks each count for 1 second - from 0:01 to 0:10, and then the next tic marks count for 10sec each - from 10 sec to 1:40 (or 100 sec), etc. (100 seconds, 1000 seconds, ...).

### Associated Charts
The primary chart data is described above.  This data is always filled, and is always on the left Y-axis.  Additional chart lines can be added to the right axis by clicking the 'More Charts' button indicated by the yellow arrow.  These values represent the observed average value during the max occurrence.  For example, examine the chart below.  For the period at 100 seconds (1:40) the maximum power produced was approximately 310 watts.  During that effort, cadence was approximately 90 RPM. 

Another example as shown in the tooltip: Maximum power produced for 2 seconds was approximately 645 watts (see purple line).  During that effort (which occurred at 27:41 from the start of the activity), average cadence was 102 RPM.

Available associated chart lines are Power, Cadence, Heart Rate, and Grade.  Note that the associated chart lines are currently only available for singe activity analysis.

![mm_detail](https://mechgt.com/st/images/mm_detail2.png)

### Report View
The Report tab (Select View > Activity Reports > Mean-Max tab/button) will allow you to track critical power/heart rate/etc. values across all of the activities in the current report (all activities listed in the report table).  In the example screenshot below, there are 3 lines shown: 5 minute (red), 20 minute (green), and 2 hour (blue).  The 5 minute line represents the maximum 5 minute power for each activity included in the report.  The same concept applies for the 20 min. and 2 hour lines.  For example in the chart below, there was an activity on Feb 28, 2008 that had a max 5 minute power of 344 watts as shown in the tool tip.

The More Charts button highlighted in yellow below, will allow you to select different time period lines.

The blue arrow drop down highlighted with a blue box will allow you to select from Power, HR, or Cadence.

![mm_detail](https://mechgt.com/st/images/mm_report.png)

NOTE: There is no settings page for this plugin, no settings are required... it just works :)

### Languages Fully Supported:
This plugin is translated into every language supported already by SportTracks!