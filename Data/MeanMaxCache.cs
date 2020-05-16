using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using MeanMax.UI;
using System.ComponentModel;

namespace MeanMax.Data
{
    static class MeanMaxCache
    {
        #region Fields

        private static SortedList<string, INumericTimeDataSeries> tracks = new SortedList<string, INumericTimeDataSeries>();
        private static SortedList<string, float> criticalCache = new SortedList<string, float>();
        private static int progress;

        #endregion

        /// <summary>
        /// Gets the cached track if available, or calculates from scratch if not available.
        /// </summary>
        /// <param name="activity">Activity to calculate</param>
        /// <param name="chartType">Which track to calculate</param>
        /// <returns>Cached track if available, or empty track if not found.</returns>
        internal static INumericTimeDataSeries GetMeanMaxTrack(IActivity activity, Common.TrackType chartType, out INumericTimeDataSeries timeTrack)
        {
            return GetMeanMaxTrack(activity, chartType, out timeTrack, true);
        }

        /// <summary>
        /// Gets the cached track if available, or calculates from scratch if not available.
        /// </summary>
        /// <param name="activity">Activity to calculate</param>
        /// <param name="chartType">Which track to calculate</param>
        /// <param name="create">Cache the track if it doesn't exist.  
        /// True will always return a track.
        /// False will return the cached track, or null if not already cached.</param>
        /// <returns>Cached track if available, or empty track if not found.</returns>
        private static INumericTimeDataSeries GetMeanMaxTrack(IActivity activity, Common.TrackType chartType, out INumericTimeDataSeries timeTrack, bool create)
        {
            if (activity == null)
            {
                timeTrack = new NumericTimeDataSeries();
                return new NumericTimeDataSeries();
            }

            string id = activity.ReferenceId + chartType;

            if (tracks.ContainsKey(id))
            {
                // Returned cached value from memory :)
                timeTrack = tracks[id + "T"];
                return tracks[id];
            }
            else if (create)
            {
                // Not in cache, create a new mean-max track :(
                INumericTimeDataSeries track = new NumericTimeDataSeries();
                timeTrack = null;

                switch (chartType)
                {
                    case Common.TrackType.HR:
                        {
                            track = GetMeanMaxTrack(activity.HeartRatePerMinuteTrack, out timeTrack, activity.StartTime);
                            break;
                        }

                    case Common.TrackType.Power:
                        {
                            track = GetMeanMaxTrack(activity.PowerWattsTrack, out timeTrack, activity.StartTime);
                            break;
                        }

                    case Common.TrackType.Cadence:
                        {
                            track = GetMeanMaxTrack(activity.CadencePerMinuteTrack, out timeTrack, activity.StartTime);
                            break;
                        }
                }

                // Add data track and related 'T'ime track to cache for next time
                MeanMaxCache.AddTrack(track, id);
                MeanMaxCache.AddTrack(timeTrack, id + "T");

                return track;
            }
            else
            {
                // Not previously cached, AND requested not to create a new cached item.
                timeTrack = new NumericTimeDataSeries();
                return null;
            }
        }

        /// <summary>
        /// Calculate Mean-Max track from scratch.  Elapsed seconds = amount of time maintained at point.Value.  
        /// </summary>
        /// <param name="source">Data track to calculate</param>
        /// <param name="timeTrack">Out parameter describing when each 'mean-max' instance was found.  
        /// Time track is the period (matching the primary mean-max track).
        /// Value of each point is the .ElapsedSeconds (relative to activity start) for each Mean-Max occurrance.
        /// This provides a key to locate associated data in other parts of the activity that occurred at the
        /// same time (such as HR, cadence, etc.)  Elapsed Seconds (relative to activity start, not track start) 
        /// is the start of the occurance.  Elapsed + period would define the full range of the occurance.</param>
        /// <param name="activityStart">Activity start time.  This is used as a reference point
        /// to align the data tracks.  See 'timeTrack' parameter.</param>
        /// <returns>Primary Mean-max data track, & 'timeTrack' as an out parameter.</returns>
        private static INumericTimeDataSeries GetMeanMaxTrack(INumericTimeDataSeries source, out INumericTimeDataSeries timeTrack, DateTime activityStart)
        {
            INumericTimeDataSeries meanMaxTrack = new NumericTimeDataSeries(); // Stores the mean-max data
            timeTrack = new NumericTimeDataSeries(); // Stores correlation data to link when max efforts occurred

            float previous = 0;

            if (source != null && source.Count > 0)
            {
                meanMaxTrack.AllowMultipleAtSameTime = true;

                float min, max, seconds = 0;
                uint maxCalc = source.TotalElapsedSeconds - 1;
                float resolution = 1.07f; // <-- This number will change how closely the calculated points are to one another.  Big number is grainy, small number is precise.
                maxCalc = maxCalc / 2;

                // Store offset (difference between activity start time and track start).  
                // Sometimes data doesn't start at same time as activity and
                // the activity start time is used as a common reference to relate other tracks together.
                int trackOffset = (int)(source.StartTime - activityStart).TotalSeconds;

                // Add the 'first' mean max point.  This is necessary to align all other points with start.
                // Without this, mean-max data would slide to the left.
                meanMaxTrack.Add(activityStart, source.Max);

                // Assign first timeTrack point as well.
                foreach (ITimeValueEntry<float> point in source)
                {
                    if (point.Value == source.Max)
                    {
                        // Set to elapsed time (in seconds) relative to activity start date.
                        timeTrack.Add(activityStart, point.ElapsedSeconds + trackOffset);
                        break;
                    }
                }

                for (uint period = maxCalc; period >= 1;)
                {
                    // Progress status
                    float percent = (((float)period / (float)maxCalc) - 1F) * -100F;

                    try
                    {
                        Application.DoEvents();
                    }
                    catch { }

                    // NOTE: Smoothing is # seconds on each side FOR ST ALGO... not total seconds (i.e. '30' would be smoothing over 60 total seconds... 30 on each side)
                    //INumericTimeDataSeries smooth = ZoneFiveSoftware.Common.Data.Algorithm.NumericTimeDataSeries.Smooth(source, (int)period, out min, out max);
                    //INumericTimeDataSeries smooth = Utilities.STSmooth(source, period, out min, out max);
                    //Utilities.ExportTrack(smooth, "C:\\Smooth\\" + period + "_ST.csv");
                    INumericTimeDataSeries smooth = Utilities.Smooth(source, period * 2, out min, out max);

                    // Add current point to data track
                    if (max >= previous)
                    {
                        // Find where max occurred
                        foreach (TimeValueEntry<float> item in smooth)
                        {
                            if (item.Value == max)
                            {
                                // Store exactly this occurs
                                seconds = item.ElapsedSeconds + trackOffset - period;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Keep values from previous iteration.  'seconds' will remain untouched.
                        max = previous;
                    }

                    // DateTime portion (first entry) is irrelevant (can be anything) to tracks below.
                    meanMaxTrack.Add(activityStart.AddSeconds(period * 2), max);
                    timeTrack.Add(activityStart.AddSeconds(period * 2), seconds);

                    period = (uint)Math.Min(period - 1, (float)period / resolution);
                    previous = max;
                }
            }

            // Return populated data track, or new (empty) data track if appropriate
            return meanMaxTrack;
        }

        /// <summary>
        /// Gets a data track that represents the corresponding data value for each maximum effort.
        /// Example (single data point): 300 watts for 10 minutes:  What was the avg cadence over this max. effort?  HR, etc.
        /// This data series will return the track to answer this question for every point in the max effort.
        /// </summary>
        /// <param name="activity">Activity to analyze.</param>
        /// <param name="pointType">Cadence, power, hr, etc.</param>
        /// <param name="timeTrack">Coded data track describing when the max efforts occurred.  
        /// Time track aligns with the max effort period (same as normal mean max time scale)
        /// Values represent max effort start time.
        /// Start time + Elapsed seconds = end time of max effort</param>
        /// <returns></returns>
        internal static INumericTimeDataSeries GetAvgTrack(IActivity activity, Common.TrackType pointType, INumericTimeDataSeries timeTrack)
        {
            if (activity == null || timeTrack == null || timeTrack.Count == 0)
            {
                return null;
            }

            INumericTimeDataSeries track = new NumericTimeDataSeries();

            foreach (TimeValueEntry<float> point in timeTrack)
            {
                // Start time is relative to activity start time.  
                //  Some data tracks don't start at same time as activity
                // point.Elapsed seconds is the period
                // point.Value is the start time relative to activity start
                float value = GetAvgValue(activity, pointType, activity.StartTime.AddSeconds(point.Value), point.ElapsedSeconds);
                if (!float.IsNaN(value))
                {
                    track.Add(timeTrack.EntryDateTime(point), value);
                }
                else if (point.ElapsedSeconds == 0)
                {
                    // This is to address the first point where the period = 0
                    value = GetAvgValue(activity, pointType, activity.StartTime, 1);
                    track.Add(timeTrack.StartTime, value);
                }
            }

            return track;
        }

        /// <summary>
        /// Gets an average value over a specific timeframe in an activity track.
        /// Used to calculate associated values, for example.
        /// </summary>
        /// <param name="activity">Activity to analyze</param>
        /// <param name="pointType">Which data track is data requested from</param>
        /// <param name="start">Start time of requested average period (in UTC time)</param>
        /// <param name="period">Duration of time (in seconds) to evaluate with respect to start time</param>
        /// <returns>The average value of X track, from 'start' to 'period' seconds later.</returns>
        internal static float GetAvgValue(IActivity activity, Common.TrackType pointType, DateTime start, uint period)
        {
            // Check for bad data
            if (activity == null || period < 0)
            {
                return float.NaN;
            }

            INumericTimeDataSeries track = null;

            switch (pointType)
            {
                case Common.TrackType.Cadence:
                    track = activity.CadencePerMinuteTrack;
                    break;
                case Common.TrackType.HR:
                    track = activity.HeartRatePerMinuteTrack;
                    break;
                case Common.TrackType.Power:
                    track = activity.PowerWattsTrack;
                    break;
                case Common.TrackType.Grade:
                    track = new NumericTimeDataSeries(ActivityInfoCache.Instance.GetInfo(activity).SmoothedGradeTrack);
                    for (int i = 0; i < track.Count; i++)
                    {
                        track.SetValueAt(i, track[i].Value * 100f);
                    }
                    break;
            }

            // No track data
            if (track == null)
            {
                return float.NaN;
            }

            // Find average value:  Sum entries every 1 second, then divide it out at the end
            float sum = 0;
            DateTime time = start;
            TimeSpan span = TimeSpan.Zero;

            while (time < start.AddSeconds(period))
            {
                // Sum all values (we'll divide it out at the end)
                ITimeValueEntry<float> item = track.GetInterpolatedValue(time);

                // Ignore bad values... they're simply excluded from calculation 
                //  and the average is taken from a smaller subset of values
                if (item != null)
                {
                    sum += item.Value;
                    span = span.Add(TimeSpan.FromSeconds(1));
                }

                time = time.AddSeconds(1);
            }

            // Divide sum by period to get the average value
            if (span.TotalSeconds > 0)
            {
                return sum / (int)span.TotalSeconds;
            }
            else
            {
                // Bad data.  Oops, requested time range didn't exist for the requested track.
                return float.NaN;
            }
        }

        /// <summary>
        /// Adds track to cache for later recall.  Replaces cache with new 
        /// data track, if track already exists in cache.
        /// </summary>
        /// <param name="track">Data track to cache</param>
        /// <param name="refId">Unique Id of track</param>
        internal static void AddTrack(INumericTimeDataSeries track, string refId)
        {
            if (tracks.ContainsKey(refId))
            {
                tracks.Remove(refId);
            }

            tracks.Add(refId, track);
        }

        /// <summary>
        /// Gets the 'critical value' for a particular activity.  For instance, 5 minute power in an given activity.
        /// </summary>
        /// <param name="activity">Activity to analyze</param>
        /// <param name="chartType">Which data track to analyze</param>
        /// <param name="seconds">Seconds representing the period.  For example, 5 minute power would be 300 seconds.</param>
        /// <returns>The critical value requested.  5 minute power might return 350 watts for example.</returns>
        internal static float GetCriticalValue(IActivity activity, Common.TrackType chartType, float seconds)
        {
            float min, criticalValue;
            INumericTimeDataSeries source, mmTrack, timeTrack;
            string id = activity.ReferenceId + chartType + seconds;

            mmTrack = GetMeanMaxTrack(activity, chartType, out timeTrack, false);

            // Return value from cache
            if (criticalCache.ContainsKey(id))
            {
                // Critical cache
                return criticalCache[id];
            }
            else if (mmTrack != null)
            {
                // value was cached previously.  saves a little time.
                ITimeValueEntry<float> point = mmTrack.GetInterpolatedValue(mmTrack.StartTime.AddSeconds(seconds));
                if (point != null)
                {
                    criticalValue = point.Value;
                }
                else
                {
                    // Value does not exist for this activity
                    criticalValue = float.NaN;
                }
            }
            else
            {
                // Calculate value
                switch (chartType)
                {
                    case Common.TrackType.Cadence:
                        source = activity.CadencePerMinuteTrack;
                        break;
                    case Common.TrackType.Power:
                        source = activity.PowerWattsTrack;
                        break;
                    default:
                    case Common.TrackType.HR:
                        source = activity.HeartRatePerMinuteTrack;
                        break;
                }

                Utilities.Smooth(source, (uint)seconds, out min, out criticalValue);
            }

            // Save to cache
            if (!float.IsNaN(criticalValue) && criticalValue != 0)
            {
                criticalCache.Add(id, criticalValue);
            }

            return criticalValue;
        }

        /// <summary>
        /// Get critical value track.  This is the full track of critical values for various activities over a date range until 'now'.
        /// /// </summary>
        /// <param name="activities">The list of activities to consider</param>
        /// <param name="trackType">Which data track to evaluate</param>
        /// <param name="seconds">The critical period, measured in seconds.  Example, for 5 minute power this would be 300.</param>
        /// <returns>Returns a data track of values over a date range.  1 value per day.</returns>
        internal static INumericTimeDataSeries GetCriticalTrack(IEnumerable<IActivity> activities, Common.TrackType trackType, float seconds)
        {
            float criticalValue = 0;
            SortedList<DateTime, float> criticalData = new SortedList<DateTime, float>();
            INumericTimeDataSeries criticalTrack = new NumericTimeDataSeries();
            float i = 0, count = (activities as List<IActivity>).Count;


            // Populate activity data
            foreach (IActivity activity in activities)
            {
                Progress = (int)(i++ / count * 100f);
                Application.DoEvents();

                // Filter bad data
                if (activity.StartTime.Year != 1)
                {
                    criticalValue = MeanMaxCache.GetCriticalValue(activity, trackType, seconds);
                    if (!float.IsNaN(criticalValue))
                    {
                        DateTime activityDate = activity.StartTime.Add(activity.TimeZoneUtcOffset).Date;

                        if (criticalData.ContainsKey(activityDate))
                        {
                            // Handle days with multiple activities
                            // Update with higher value
                            criticalData[activityDate] = Math.Max(criticalValue, criticalData[activityDate]);
                        }
                        else if (criticalValue != 0)
                        {
                            // Add critical value to data set
                            criticalData.Add(activityDate, criticalValue);
                        }
                    }
                }
            }

            // Construct critical data track
            DateTime firstDate;
            if (criticalData.Count > 0)
            {
                firstDate = criticalData.Keys[0];
                criticalValue = criticalData[firstDate];
            }
            else
            {
                firstDate = DateTime.Now;
            }
            float value;

            for (DateTime day = firstDate; day < DateTime.Now;)
            {
                // Decay the critical value first, value for 'today'
                //criticalValue = criticalValue - criticalValue / GlobalSettings.Instance.TCc;

                if (criticalData.TryGetValue(day.Date, out value))
                {
                    // TODO: Determine best way to incorporate decay values
                    // Value recorded for today or previous (decayed) value
                    //criticalValue = Math.Max(value, criticalValue);
                    //criticalTrack.Add(day, criticalValue);
                    criticalTrack.Add(day, value);
                }
                else
                {
                    // Decayed value
                    //criticalTrack.Add(day, criticalValue);
                }
                // Next day
                day = day.AddDays(1);
            }

            Progress = -1;

            return criticalTrack;
        }

        /// <summary>
        /// Clear a particular track from the cache.
        /// </summary>
        /// <param name="refId"></param>
        internal static void ClearTrack(string refId)
        {
            if (tracks.ContainsKey(refId))
            {
                tracks.Remove(refId);
            }
        }

        /// <summary>
        /// Clear a particular critical value from the cache.
        /// </summary>
        /// <param name="refId"></param>
        internal static void ClearCritical(string refId)
        {
            // TODO: Monitor activities and clear modified activities from cache
            if (criticalCache.ContainsKey(refId))
            {
                criticalCache.Remove(refId);
            }
        }

        public static event PropertyChangedEventHandler ProgressUpdated;

        /// <summary>
        /// Gets or sets the current progress
        /// </summary>
        internal static int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                ProgressUpdated.Invoke(null, new PropertyChangedEventArgs("Progress"));
            }
        }
    }
}
