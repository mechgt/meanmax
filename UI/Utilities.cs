// <copyright file="Utilities.cs" company="N/A">
// Copyright (c) 2008 All Right Reserved
// </copyright>
// <author>mechgt</author>
// <email>mechgt@gmail.com</email>
// <date>2008-12-23</date>
namespace MeanMax.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;
    using ZoneFiveSoftware.Common.Data.Fitness;
    using ZoneFiveSoftware.Common.Visuals;
    using ZoneFiveSoftware.Common.Data;
    using System.Drawing;
    using ZoneFiveSoftware.Common.Data.Measurement;

    /// <summary>
    /// Generic utilities class that can be used on many projects
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Export track to .csv file
        /// </summary>
        /// <param name="track"></param>
        public static void ExportTrack(INumericTimeDataSeries track, string name)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                System.IO.StreamWriter writer = new System.IO.StreamWriter(name);

                // Write Header
                writer.WriteLine(track.StartTime.ToLocalTime() + ", " + System.IO.Path.GetFileNameWithoutExtension(name));
                writer.WriteLine("Seconds, Value");

                foreach (ITimeValueEntry<float> item in track)
                {
                    // Write data
                    writer.WriteLine(item.ElapsedSeconds + ", " + item.Value);
                }

                writer.Close();
                MessageDialog.Show(CommonResources.Text.MessageExportComplete, Resources.Strings.Label_MeanMax, MessageBoxButtons.OK);
            }
            catch { }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Export multiple tracks to single .csv file.  These tracks are indexed by date.
        /// </summary>
        /// <param name="tracks">Dataseries (keys) with descriptive names (values)</param>
        /// <param name="filename">Full filename (including path and extension) to save .csv file</param>
        public static void ExportTrack(Dictionary<INumericTimeDataSeries, string> tracks, string filename)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(filename);
                SortedList<DateTime, string> data = new SortedList<DateTime, string>();

                // Header line
                string header = CommonResources.Text.LabelDate;

                // Collect data
                foreach (KeyValuePair<INumericTimeDataSeries, string> track in tracks)
                {
                    // Append header line
                    header += ", " + track.Value;

                    foreach (ITimeValueEntry<float> item in track.Key)
                    {
                        // Organize data
                        DateTime date = track.Key.EntryDateTime(item);
                        string value = item.Value.ToString("0.#", System.Globalization.CultureInfo.CurrentCulture);

                        if (data.ContainsKey(date))
                        {
                            data[date] = data[date] + ", " + value;
                        }
                        else
                        {
                            data.Add(date, value);
                        }
                    }
                }

                // Write header
                writer.WriteLine(header);

                // Write values
                foreach (KeyValuePair<DateTime, string> item in data)
                {
                    writer.WriteLine(item.Key.ToShortDateString() + ", " + item.Value);
                }

                writer.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Get an icon from an image
        /// </summary>
        /// <param name="image">Icon image (bitmap)</param>
        internal static Icon GetIcon(Image image)
        {
            Bitmap bitmap = image as Bitmap;

            if (bitmap != null)
            {
                return Icon.FromHandle(bitmap.GetHicon());
            }

            return null;
        }

        /// <summary>
        /// Rainbow will return a distinct list of colors based on ROYGBIV
        /// </summary>
        /// <param name="totalItems">Number of colors to generate</param>
        /// <returns>Returns a distinct list of colors</returns>
        internal static List<Color> Rainbow(int totalItems, int alpha)
        {
            List<Color> colors = new List<Color>();
            double red;
            double green;
            double blue;
            double scaleFactor;

            // Harshness of the color. Max is 255
            int harshness = 150;

            // Manually add the colors if there are less than 6 items
            if (totalItems == 1)
            {
                colors.Add(Color.FromArgb(alpha, harshness, 0, 0));
            }
            else if (totalItems == 2)
            {
                colors.Add(Color.FromArgb(alpha, harshness, 0, 0));
                colors.Add(Color.FromArgb(alpha, 0, harshness, 0));
            }
            else if (totalItems == 3)
            {
                colors.Add(Color.FromArgb(alpha, harshness, 0, 0));
                colors.Add(Color.FromArgb(alpha, 0, harshness, 0));
                colors.Add(Color.FromArgb(alpha, 0, 0, harshness));
            }
            else if (totalItems == 4)
            {
                colors.Add(Color.FromArgb(alpha, harshness, 0, 0));
                colors.Add(Color.FromArgb(alpha, harshness, harshness, 0));
                colors.Add(Color.FromArgb(alpha, 0, harshness, 0));
                colors.Add(Color.FromArgb(alpha, 0, 0, harshness));
            }
            else if (totalItems == 5)
            {
                colors.Add(Color.FromArgb(alpha, harshness, 0, 0));
                colors.Add(Color.FromArgb(alpha, harshness, harshness, 0));
                colors.Add(Color.FromArgb(alpha, 0, harshness, 0));
                colors.Add(Color.FromArgb(alpha, 0, harshness, harshness));
                colors.Add(Color.FromArgb(alpha, 0, 0, harshness));
            }

            // Make sure we have a multiple of 6 to rainbow
            while (totalItems % 6 != 0)
            {
                totalItems += 1;
            }

            // Find the factor to which we will scale the colors
            scaleFactor = ((double)harshness / totalItems) * 6f;

            // Red is our starting point
            red = harshness;
            green = 0;
            blue = 0;

            // Add red to the list
            colors.Add(Color.FromArgb(alpha, (int)red, (int)green, (int)blue));

            // Work your way through the spectrum to build the colors
            while (green < harshness)
            {
                green += scaleFactor;

                // Catch any potential rounding issues
                if (green > harshness)
                {
                    green = harshness;
                }
                colors.Add(Color.FromArgb(alpha, (int)red, (int)green, (int)blue));
            }

            while (red > 0)
            {
                red -= scaleFactor;

                // Catch any potential rounding issues
                if (red < 0)
                {
                    red = 0;
                }

                colors.Add(Color.FromArgb(alpha, (int)red, (int)green, (int)blue));
            }

            while (blue < harshness)
            {
                blue += scaleFactor;

                // Catch any potential rounding issues
                if (blue > harshness)
                {
                    blue = harshness;
                }

                colors.Add(Color.FromArgb(alpha, (int)red, (int)green, (int)blue));
            }

            while (green > 0)
            {
                green -= scaleFactor;

                // Catch any potential rounding issues
                if (green < 0)
                {
                    green = 0;
                }

                colors.Add(Color.FromArgb(alpha, (int)red, (int)green, (int)blue));
            }

            while (red < harshness)
            {
                red += scaleFactor;

                // Catch any potential rounding issues
                if (red > harshness)
                {
                    red = harshness;
                }

                colors.Add(Color.FromArgb(alpha, (int)red, (int)green, (int)blue));
            }

            while (blue > 0)
            {
                blue -= scaleFactor;

                // Catch any potential rounding issues
                if (blue < 0)
                {
                    blue = 0;
                }

                colors.Add(Color.FromArgb(alpha, (int)red, (int)green, (int)blue));
            }

            // The last color and the first color should be the same.  Remove the last color
            colors.RemoveAt(colors.Count - 1);

            // Return the colors list
            return colors;
        }

        /// <summary>
        /// Perform a smoothing operation using a moving average on the data series
        /// </summary>
        /// <param name="track">The data series to smooth</param>
        /// <param name="period">The range to smooth.  This is the total number of seconds to smooth across (slightly different than the ST method.)</param>
        /// <param name="min">An out parameter set to the minimum value of the smoothed data series</param>
        /// <param name="max">An out parameter set to the maximum value of the smoothed data series</param>
        /// <returns></returns>
        internal static INumericTimeDataSeries Smooth(INumericTimeDataSeries track, uint period, out float min, out float max)
        {
            min = float.NaN;
            max = float.NaN;
            INumericTimeDataSeries smooth = new NumericTimeDataSeries();

            if (track != null && track.Count > 0 && period > 1)
            {
                int start = 0;
                int index = 0;
                float value = 0;
                float delta;

                float per = period;
                int offset = (int)period / 2;

                // TODO: Handle smoothing beginning and end of track.  Remove debug line below and replace with real code.  Debug establishes the first point.
                // TODO: This NEEDS to be fixed: Smoothing
                smooth.Add(track.StartTime, 0);
                //  Idea: Fill unavailable points with first (and last) fully averaged values to weight everything appropriately.

                //// Find first average set
                //while (track[index].ElapsedSeconds < track[start].ElapsedSeconds + period)
                //{
                //    delta = track[index + 1].ElapsedSeconds - track[index].ElapsedSeconds;
                //    firstAvg += track[index].Value * delta;
                //    index++;
                //}

                //// Finish value calculation
                //per = track[index].ElapsedSeconds - track[start].ElapsedSeconds;
                //firstAvg = firstAvg / per;

                // Iterate through track
                // For each point, create average starting with 'start' index and go forward averaging 'period' seconds.
                // Stop when last 'full' period can be created ([start].ElapsedSeconds + 'period' seconds >= TotalElapsedSeconds)
                index = 0;
                while (track[start].ElapsedSeconds + period < track.TotalElapsedSeconds)
                {
                    while (track[index].ElapsedSeconds < track[start].ElapsedSeconds + period)
                    {
                        delta = track[index + 1].ElapsedSeconds - track[index].ElapsedSeconds;
                        value += track[index].Value * delta;
                        index++;
                    }

                    // Finish value calculation
                    per = track[index].ElapsedSeconds - track[start].ElapsedSeconds;
                    value = value / per;

                    // Add value to track - Offset will align it so that the value is centered within the period
                    smooth.Add(track.EntryDateTime(track[index]).AddSeconds(-offset), value);

                    // Remove beginning point for next cycle
                    delta = track[start + 1].ElapsedSeconds - track[start].ElapsedSeconds;
                    value = (per * value - delta * track[start].Value);

                    // Next point
                    start++;
                }

                max = smooth.Max;
                min = smooth.Min;
            }
            else if (track != null && track.Count > 0 && period == 1)
            {
                min = track.Min;
                max = track.Max;
                return track;
            }

            return smooth;
        }

        internal static INumericTimeDataSeries STSmooth(INumericTimeDataSeries data, uint seconds, out float min, out float max)
        {
            min = float.NaN;
            max = float.NaN;
            if (data.Count == 0)
            {
                // Special case, no data
                return new ZoneFiveSoftware.Common.Data.NumericTimeDataSeries();
            }
            else if (data.Count == 1 || seconds < 1)
            {
                // Special case
                INumericTimeDataSeries copyData = new ZoneFiveSoftware.Common.Data.NumericTimeDataSeries();
                min = data[0].Value;
                max = data[0].Value;
                foreach (ITimeValueEntry<float> entry in data)
                {
                    copyData.Add(data.StartTime.AddSeconds(entry.ElapsedSeconds), entry.Value);
                    min = Math.Min(min, entry.Value);
                    max = Math.Max(max, entry.Value);
                }
                return copyData;
            }
            min = float.MaxValue;
            max = float.MinValue;
            int smoothWidth = Math.Max(0, (int)seconds * 2); // Total width/period.  'seconds' is the half-width... seconds on each side to smooth
            int denom = smoothWidth * 2; // Final value to divide by.  It's divide by 2 because we're double-adding everything
            INumericTimeDataSeries smoothedData = new ZoneFiveSoftware.Common.Data.NumericTimeDataSeries();

            // Loop through entire dataset
            for (int nEntry = 0; nEntry < data.Count; nEntry++)
            {
                ITimeValueEntry<float> entry = data[nEntry];
                // This should not reset value & index markers, instead continue data here...
                double value = 0;
                double delta;
                // Data prior to entry
                long secondsRemaining = seconds;
                ITimeValueEntry<float> p1, p2;
                int increment = -1;
                int pos = nEntry - 1;
                p2 = data[nEntry];


                while (secondsRemaining > 0 && pos >= 0)
                {
                    p1 = data[pos];
                    if (SumValues(p2, p1, ref value, ref secondsRemaining))
                    {
                        pos += increment;
                        p2 = p1;
                    }
                    else
                    {
                        break;
                    }
                }
                if (secondsRemaining > 0)
                {
                    // Occurs at beginning of track when period extends before beginning of track.
                    delta = data[0].Value * secondsRemaining * 2;
                    value += delta;
                }
                // Data after entry
                secondsRemaining = seconds;
                increment = 1;
                pos = nEntry;
                p1 = data[nEntry];
                while (secondsRemaining > 0 && pos < data.Count - 1)
                {
                    p2 = data[pos + 1];
                    if (SumValues(p1, p2, ref value, ref secondsRemaining))
                    {
                        // Move to next point
                        pos += increment;
                        p1 = p2;
                    }
                    else
                    {
                        break;
                    }
                }
                if (secondsRemaining > 0)
                {
                    // Occurs at end of track when period extends past end of track
                    value += data[data.Count - 1].Value * secondsRemaining * 2;
                }
                float entryValue = (float)(value / denom);
                smoothedData.Add(data.StartTime.AddSeconds(entry.ElapsedSeconds), entryValue);
                min = Math.Min(min, entryValue);
                max = Math.Max(max, entryValue);

                // Someone should remove 'first' p1 & p2 SumValues from 'value'
                if (data[nEntry].ElapsedSeconds - seconds < 0)
                {
                    // Remove 1 second worth of first data point (multiply by 2 because everything is double here)
                    value -= data[0].Value * 2;
                }
                else
                {
                    // Remove data in middle of track (typical scenario)
                    //value -= 
                }
            }
            return smoothedData;
        }

        private static bool SumValues(ITimeValueEntry<float> p1, ITimeValueEntry<float> p2, ref double value, ref long secondsRemaining)
        {
            double spanSeconds = Math.Abs((double)p2.ElapsedSeconds - (double)p1.ElapsedSeconds);
            if (spanSeconds <= secondsRemaining)
            {
                value += (p1.Value + p2.Value) * spanSeconds;
                secondsRemaining -= (long)spanSeconds;
                return true;
            }
            else
            {
                double percent = (double)secondsRemaining / (double)spanSeconds;
                value += (p1.Value * ((float)2 - percent) + p2.Value * percent) * secondsRemaining;
                secondsRemaining = 0;
                return false;
            }
        }

        /// <summary>
        /// Removes paused (but not stopped?) times in track.
        /// </summary>
        /// <param name="sourceTrack">Source data track to remove paused times</param>
        /// <param name="activity"></param>
        /// <returns>Returns an INumericTimeDataSeries with the paused times removed.</returns>
        internal static INumericTimeDataSeries RemovePausedTimesInTrack(INumericTimeDataSeries sourceTrack, IActivity activity)
        {
            ActivityInfo activityInfo = ActivityInfoCache.Instance.GetInfo(activity);

            if (activityInfo != null && sourceTrack != null)
            {
                if (activityInfo.NonMovingTimes.Count == 0)
                {
                    return sourceTrack;
                }
                else
                {
                    INumericTimeDataSeries result = new NumericTimeDataSeries();
                    DateTime currentTime = sourceTrack.StartTime;
                    IEnumerator<ITimeValueEntry<float>> sourceEnumerator = sourceTrack.GetEnumerator();
                    IEnumerator<IValueRange<DateTime>> pauseEnumerator = activityInfo.NonMovingTimes.GetEnumerator();
                    double totalPausedTimeToDate = 0;
                    bool sourceEnumeratorIsValid;
                    bool pauseEnumeratorIsValid;

                    pauseEnumeratorIsValid = pauseEnumerator.MoveNext();
                    sourceEnumeratorIsValid = sourceEnumerator.MoveNext();

                    while (sourceEnumeratorIsValid)
                    {
                        bool addCurrentSourceEntry = true;
                        bool advanceCurrentSourceEntry = true;

                        // Loop to handle all pauses up to this current track point
                        if (pauseEnumeratorIsValid)
                        {
                            if (currentTime >= pauseEnumerator.Current.Lower &&
                                currentTime <= pauseEnumerator.Current.Upper)
                            {
                                addCurrentSourceEntry = false;
                            }
                            else if (currentTime > pauseEnumerator.Current.Upper)
                            {
                                // Advance pause enumerator
                                totalPausedTimeToDate += (pauseEnumerator.Current.Upper - pauseEnumerator.Current.Lower).TotalSeconds;
                                pauseEnumeratorIsValid = pauseEnumerator.MoveNext();

                                // Make sure we retry with the next pause
                                addCurrentSourceEntry = false;
                                advanceCurrentSourceEntry = false;
                            }
                        }

                        if (addCurrentSourceEntry)
                        {
                            result.Add(currentTime - new TimeSpan(0, 0, (int)totalPausedTimeToDate), sourceEnumerator.Current.Value);
                        }

                        if (advanceCurrentSourceEntry)
                        {
                            sourceEnumeratorIsValid = sourceEnumerator.MoveNext();
                            currentTime = sourceTrack.StartTime + new TimeSpan(0, 0, (int)sourceEnumerator.Current.ElapsedSeconds);
                        }
                    }

                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Open a popup treelist.
        /// </summary>
        /// <typeparam name="T">The type of items to be listed</typeparam>
        /// <param name="theme">Visual Theme</param>
        /// <param name="items">Items to be listed</param>
        /// <param name="control">The control that the list will appear attached to</param>
        /// <param name="selected">selected item</param>
        /// <param name="selectHandler">Handler that will handle when an item is clicked</param>
        internal static void OpenListPopup<T>(ITheme theme, IList<T> items, System.Windows.Forms.Control control, T selected, TreeListPopup.ItemSelectedEventHandler selectHandler)
        {
            TreeListPopup popup = new TreeListPopup();
            popup.ThemeChanged(theme);
            popup.Tree.Columns.Add(new TreeList.Column());
            popup.Tree.RowData = items;
            if (selected != null)
            {
                popup.Tree.Selected = new object[] { selected };
            }

            popup.ItemSelected += delegate(object sender, TreeListPopup.ItemSelectedEventArgs e)
            {
                if (e.Item is T)
                {
                    selectHandler((T)e.Item, e);
                }
            };
            popup.Popup(control.Parent.RectangleToScreen(control.Bounds));
        }

        /// <summary>
        /// Open a context menu.
        /// </summary>
        /// <param name="theme">Visual Theme</param>
        /// <param name="items">Items to be listed</param>
        /// <param name="mouse"></param>
        /// <param name="selectHandler">Handler that will handle when an item is clicked</param>
        internal static void OpenContextPopup(ITheme theme, ToolStripItemCollection items, MouseEventArgs mouse, ToolStripItemClickedEventHandler selectHandler)
        {
            ContextMenuStrip menuStrip = new ContextMenuStrip();

            menuStrip.Items.AddRange(items);

            menuStrip.ItemClicked += delegate(object sender, ToolStripItemClickedEventArgs e)
            {
                selectHandler(e.ClickedItem, e);
            };
            menuStrip.Show(mouse.Location);
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        internal static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return constructedString;
        }

        /// <summary>
        /// Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        internal static byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
    }
}
