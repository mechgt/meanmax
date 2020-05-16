using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using MeanMax.Data;
using ZedGraph;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Forms;
using MeanMax.Resources;
using System.Threading;

namespace MeanMax.UI.DetailPage
{
    internal partial class MeanMaxDetailControl : UserControl
    {
        #region Fields

        private IEnumerable<IActivity> activities;
        private static MeanMaxDetailControl control;

        private const string timeId = "time";

        #endregion

        #region Constructor

        internal MeanMaxDetailControl()
        {
            InitializeComponent();

            MaximizeButton.Location = new Point(ChartBanner.Width - 50, 0);

            ChartBanner.Text = CommonResources.Text.LabelPower;
            zedChart.GraphPane.XAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(XScaleFormatEvent);
            zedChart.PointValueEvent += new ZedGraphControl.PointValueHandler(zedChart_PointValueEvent);
            control = this;

            // Setup tool tips
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip.SetToolTip(this.SaveImageButton, CommonResources.Text.ActionSave);
            toolTip.SetToolTip(this.ZoomInButton, CommonResources.Text.ActionZoomIn);
            toolTip.SetToolTip(this.ZoomOutButton, CommonResources.Text.ActionZoomOut);
            // TODO: Localize tooltips (should be in ST core resources)
            toolTip.SetToolTip(this.ZoomChartButton, "Fit to Window");
            toolTip.SetToolTip(this.ExtraChartsButton, "More Charts");
            toolTip.SetToolTip(this.ExportButton, CommonResources.Text.ActionExport);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Set activity for the detail display.  Enable/disable menu items based on whether the data tracks exist.
        /// </summary>
        internal void SetActivities(IEnumerable<IActivity> activities)
        {
            this.activities = activities;

            cadenceToolStripMenuItem.Enabled = false;
            heartRateToolStripMenuItem.Enabled = false;
            powerToolStripMenuItem.Enabled = false;

            if (activities == null)
            {
                // Simply disable menu items if no activity selected
                return;
            }

            // Enable applicable data track menu items.  Default item is set here also... power > hr > cadence
            foreach (IActivity activity in activities)
            {
                if (activity.CadencePerMinuteTrack != null)
                {
                    cadenceToolStripMenuItem.Enabled = true;
                }

                if (activity.HeartRatePerMinuteTrack != null)
                {
                    heartRateToolStripMenuItem.Enabled = true;
                }

                if (activity.PowerWattsTrack != null)
                {
                    powerToolStripMenuItem.Enabled = true;
                }

                // Simply a shortcut to stop evaluating if all items are now enabled
                if (cadenceToolStripMenuItem.Enabled && heartRateToolStripMenuItem.Enabled && powerToolStripMenuItem.Enabled)
                {
                    break;
                }
            }

            // Set default menu strip selection
            if (powerToolStripMenuItem.Enabled)
            {
                bnrMenuItem_Click(powerToolStripMenuItem, null);
            }
            else if (heartRateToolStripMenuItem.Enabled)
            {
                bnrMenuItem_Click(heartRateToolStripMenuItem, null);
            }
            else if (cadenceToolStripMenuItem.Enabled)
            {
                bnrMenuItem_Click(cadenceToolStripMenuItem, null);
            }
            else
            {
                bnrMenuItem_Click(powerToolStripMenuItem, null);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the chart type.  Note that chart is automatically 
        /// refreshed any time charttype is changed.
        /// </summary>
        internal Common.TrackType ChartType
        {
            get
            {
                return GlobalSettings.Instance.PrimaryChart;
            }

            set
            {
                GlobalSettings.Instance.PrimaryChart = value;
                SetupSecondaryAxes(GlobalSettings.Instance.ChartLines as IList<string>);
                RefreshPage();
            }
        }

        /// <summary>
        /// Gets or Sets a status indicator letting us know if this control is visible or not
        /// </summary>
        internal bool IsVisible
        {
            get;
            set;
        }

        #endregion

        #region Methods

        internal void RefreshPage()
        {
            if (activities == null)
            {
                return;
            }

            INumericTimeDataSeries meanMax = new NumericTimeDataSeries();
            INumericTimeDataSeries timeTrack = new NumericTimeDataSeries();
            INumericTimeDataSeries activityMax = new NumericTimeDataSeries();
            Dictionary<string, INumericTimeDataSeries> assocTracks = new Dictionary<string, INumericTimeDataSeries>();
            SortedList<float, float> mmTempList = new SortedList<float, float>();

            meanMax.AllowMultipleAtSameTime = true;

            DateTime start = DateTime.Now.Date;

            foreach (IActivity mmact in activities)
            {
                // Try to pull from memory if available
                activityMax = MeanMaxCache.GetMeanMaxTrack(mmact, ChartType, out timeTrack);
#if DebugOFF
                Utilities.ExportTrack(activityMax, "C:\\STexports\\" + "MeanMax" + ".csv");
                Utilities.ExportTrack(mmact.PowerWattsTrack, "C:\\STexports\\" + "RawPowerTrack" + ".csv");
                Utilities.ExportTrack(mmact.HeartRatePerMinuteTrack, "C:\\STexports\\" + "RawHeartRate" + ".csv");
                Utilities.ExportTrack(mmact.CadencePerMinuteTrack, "C:\\STexports\\" + "RawCadence" + ".csv");
#endif
                int numActivities = (activities as List<IActivity>).Count;
                if (numActivities == 1)
                {
                    // Add timetrack - Note this this only available for single activity analysis
                    assocTracks.Add(timeId, timeTrack);

                    // Add associated chart lines (matching HR, Cadence, etc.)
                    foreach (string id in GlobalSettings.Instance.ChartLines)
                    {
                        Common.TrackType lineType = ColumnDefinition.GetTrackType(id);
                        if (lineType != ChartType)
                        {
                            // Get associated track (HR, Cad, etc.)
                            // This line is similar to the Mean-max chart where the time track is the 'period'
                            //  but the data is the HR, Cad., etc. that occurred at the -same time- as the Max effort occurred
                            //  rather than being a max value in itself.  
                            //  The value is the average value during the range of time as opposed to a single entry
                            INumericTimeDataSeries track = MeanMaxCache.GetAvgTrack(mmact, lineType, timeTrack);
#if DebugOFF
                            Utilities.ExportTrack(track, "C:\\STexports\\MM" + ColumnDefinition.GetText(id) + ".csv");
#endif
                            if (!assocTracks.ContainsKey(id) && track != null)
                            {
                                // Store associated tracks for charting
                                assocTracks.Add(id, track);
                            }
                            else
                            {
                                // TODO: Combine multiple tracks for averaging.  Currently it'll only display the first activity track :(
                            }
                        }
                    }
                }

                // Compile all points together (used for multiple activities)
                foreach (TimeValueEntry<float> item in activityMax)
                {
                    /* If(entry not exist in meanMax || current value > existing value)
                     *  {
                     *   Add new MM entry
                     *   Update assoc track values
                     *  }
                     *  else
                     *  {
                     *   
                     *  }
                     */

                    // Add to temporary sortedlist
                    if (!mmTempList.ContainsKey(item.ElapsedSeconds))
                        mmTempList.Add(item.ElapsedSeconds, item.Value);
                    else if (mmTempList.ContainsKey(item.ElapsedSeconds) && mmTempList[item.ElapsedSeconds] < item.Value)
                        mmTempList[item.ElapsedSeconds] = item.Value;
                    //meanMax.Add(start.AddSeconds(item.ElapsedSeconds), item.Value);
                }

                // Copy sorted temporary list to proper numeric time series
                foreach (float seconds in mmTempList.Keys)
                    meanMax.Add(start.AddSeconds(seconds), mmTempList[seconds]);

                // Remove low points (used for multiple activities)
                for (int i = meanMax.Count - 2; i >= 0; i--)
                    if (meanMax[i].Value < meanMax[i + 1].Value)
                        meanMax.RemoveAt(i);
            }

            updateZedGraph(meanMax, assocTracks, zedChart, ChartType);
        }

        /// <summary>
        /// Combine a list of tracks by averaging all of the values together.
        /// Start times are ignored, ElapsedSeconds is used for track alignment.  Each track is weighted equally.
        /// </summary>
        /// <param name="tracks">List of tracks to average together</param>
        /// <returns>A single track representing the average data series.</returns>
        private static INumericTimeDataSeries CombineAvgTracks(IList<INumericTimeDataSeries> tracks)
        {
            // Quick data analysis... return quickly if possible
            if (tracks.Count == 0)
            {
                // No data to analyze
                return null;
            }
            else if (tracks.Count == 1)
            {
                // Only 1 track.  It is it's own average :)
                return tracks[0];
            }

            // Setup for analysis...
            INumericTimeDataSeries avgTrack = new NumericTimeDataSeries();
            INumericTimeDataSeries longTrack = tracks[0];

            // Find longest track to be used as index later
            foreach (INumericTimeDataSeries track in tracks)
            {
                if (longTrack.TotalElapsedSeconds < track.TotalElapsedSeconds)
                {
                    longTrack = track;
                }
            }

            // Average the values:  Use longTrack as the reference because it'll cover all tracks 
            //  (some may be shorter, which is OK)
            // Using this track as a reference will also be more efficient because it 
            //  won't go through every second, it'll take advantage of the logarithmic algos elsewhere.
            foreach (ITimeValueEntry<float> index in longTrack)
            {
                // Sum & count used later to determine average
                float sum = 0;
                int count = 0;

                // Collect average point in each track
                foreach (INumericTimeDataSeries track in tracks)
                {
                    ITimeValueEntry<float> item = track.GetInterpolatedValue(track.StartTime.AddSeconds(index.ElapsedSeconds));
                    if (item != null)
                    {
                        sum += item.Value;
                        count++;
                    }
                }

                // Store average
                avgTrack.Add(longTrack.EntryDateTime(index), sum / count);
            }

            return avgTrack;

        }

        public void MaximizePage(bool maximize)
        {
            switch (maximize)
            {
                case true:
                    // Maximize
                    // Need to organize the controls in the parent container before settings dock properties.
                    // This sets what 'layer' things are on.
                    this.MaximizeButton.CenterImage = CommonResources.Images.View3PaneLowerLeft16;

                    break;

                case false:
                    // Restore
                    this.MaximizeButton.CenterImage = CommonResources.Images.View2PaneLowerHalf16;
                    break;
            }
        }

        /// <summary>
        /// Add 'track' to 'graph' and apply labels based on 'chartType'
        /// </summary>
        /// <param name="track">Data track</param>
        /// <param name="graph">Which graph to stick the data on</param>
        /// <param name="chartType">This determines the labeling, coloring, etc. (all appearance related)</param>
        internal static void updateZedGraph(INumericTimeDataSeries track, Dictionary<string, INumericTimeDataSeries> assocTracks, ZedGraphControl graph, Common.TrackType chartType)
        {
            GraphPane myPane = graph.GraphPane;
            myPane.XAxis.Title.Text = CommonResources.Text.LabelTime;
            myPane.XAxis.Type = AxisType.Log;

            Color mainCurveColor = Common.GetColor(chartType);
            string tag = string.Empty;
            switch (chartType)
            {
                case Common.TrackType.Cadence:
                    myPane.YAxis.Title.Text = CommonResources.Text.LabelCadence + " (" + CommonResources.Text.LabelRPM + ")";
                    tag = ColumnDefinition.cadenceID;
                    break;
                case Common.TrackType.HR:
                    myPane.YAxis.Title.Text = CommonResources.Text.LabelHeartRate + " (" + CommonResources.Text.LabelBPM + ")";
                    tag = ColumnDefinition.hrID;
                    break;
                case Common.TrackType.Power:
                    myPane.YAxis.Title.Text = CommonResources.Text.LabelPower + " (" + CommonResources.Text.LabelWatts + ")";
                    tag = ColumnDefinition.powerID;
                    break;
            }

            myPane.XAxis.MinorTic.IsOutside = true;

            // Add primary mean max chart
            PointPairList zedTrack = new PointPairList();
            INumericTimeDataSeries timeTrack = null;

            if (assocTracks.ContainsKey(timeId))
            {
                timeTrack = assocTracks[timeId];
            }

            foreach (ITimeValueEntry<float> item in track)
            {
                float time = -1;
                if (timeTrack != null)
                {
                    int index = track.IndexOf(item);
                    time = timeTrack[index].Value;
                }

                zedTrack.Add(item.ElapsedSeconds, item.Value, time);
            }

            myPane.CurveList.Clear();
            LineItem curve = myPane.AddCurve("Curve Label", zedTrack, mainCurveColor, SymbolType.None);
            curve.Line.Width = 1f;
            curve.Line.Fill.Type = FillType.Solid;
            curve.Line.Fill.Color = Color.FromArgb(50, mainCurveColor);
            curve.Tag = tag;
            curve.Line.IsAntiAlias = true;
            myPane.YAxis.Title.FontSpec.FontColor = mainCurveColor;
            myPane.YAxis.Scale.FontSpec.FontColor = mainCurveColor;


            // Add secondary correlation charts
            int yIndex = 1;

            foreach (string id in GlobalSettings.Instance.ChartLines)
            {
                if (assocTracks.ContainsKey(id))
                {
                    zedTrack = new PointPairList();

                    // Assemble associated track data
                    foreach (ITimeValueEntry<float> item in assocTracks[id])
                    {
                        // Include time of occurrance
                        float time = -1;
                        int index = assocTracks[id].IndexOf(item);

                        if (timeTrack != null && timeTrack.Count > index)
                        {
                            time = timeTrack[index].Value;
                        }

                        zedTrack.Add(item.ElapsedSeconds, item.Value, time);
                    }

                    // Setup display properties of associated track
                    Color color = ColumnDefinition.GetTrackColor(id);
                    curve = myPane.AddCurve(id, zedTrack, color, SymbolType.None);
                    curve.Line.Width = 1f;
                    curve.Line.Fill.Type = FillType.None;
                    curve.Line.IsAntiAlias = true;
                    curve.Tag = id;
                    curve.IsY2Axis = true;
                    yIndex = myPane.Y2AxisList.IndexOfTag(id);

                    if (yIndex != -1)
                    {
                        // Set to existing axis
                        curve.YAxisIndex = yIndex;
                    }
                    else
                    {
                        // Oops... ERROR!
                    }
                }
            }

            if (track.Count > 0)
            {
                graph.AxisChange();
            }

            graph.Refresh();
        }

        #endregion

        #region Theme & Culture

        internal void ThemeChanged(ITheme visualTheme)
        {
            ButtonPanel.ThemeChanged(visualTheme);
            ButtonPanel.BackColor = visualTheme.Window;
            ChartBanner.ThemeChanged(visualTheme);
            panelMain.ThemeChanged(visualTheme);
            panelMain.BackColor = visualTheme.Window;
            zedThemeChanged(visualTheme, zedChart);
        }

        /// <summary>
        /// Setup 'graph' to look like a SportTracks chart.
        /// </summary>
        /// <param name="visualTheme">Theme to apply</param>
        /// <param name="graph">ZedChart that we're masquerading as a SportTracks chart</param>
        internal static void zedThemeChanged(ITheme visualTheme, ZedGraphControl graph)
        {
            GraphPane myPane = graph.GraphPane;

            // Overall appearance settings
            graph.BorderStyle = BorderStyle.None;
            myPane.Legend.IsVisible = false;
            myPane.Border.IsVisible = false;
            myPane.Title.IsVisible = false;

            // Add a background color
            myPane.Fill.Color = visualTheme.Window;
            myPane.Chart.Fill = new Fill(visualTheme.Window);
            myPane.Chart.Border.IsVisible = false;

            // Add gridlines to the plot, and make them gray
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.MajorGrid.Color = Color.DarkGray;
            myPane.YAxis.MajorGrid.Color = myPane.XAxis.MajorGrid.Color;
            myPane.XAxis.MajorGrid.DashOff = 1f;
            myPane.XAxis.MajorGrid.DashOff = myPane.XAxis.MajorGrid.DashOn;
            myPane.YAxis.MajorGrid.DashOff = myPane.XAxis.MajorGrid.DashOn;
            myPane.YAxis.MajorGrid.DashOff = myPane.YAxis.MajorGrid.DashOn;
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            myPane.XAxis.IsAxisSegmentVisible = true;
            myPane.YAxis.IsAxisSegmentVisible = true;

            // Update axis Tic marks
            myPane.XAxis.MinorTic.IsAllTics = false;
            myPane.XAxis.MajorTic.IsAllTics = false;
            myPane.YAxis.MinorTic.IsAllTics = false;
            myPane.YAxis.MajorTic.IsAllTics = false;
            myPane.XAxis.MajorTic.IsOutside = true;
            myPane.YAxis.MajorTic.IsOutside = true;

            // Setup Text Appearance
            string fontName = "Microsoft Sans Sarif";
            myPane.IsFontsScaled = false;
            myPane.XAxis.Title.FontSpec.Family = fontName;
            myPane.XAxis.Title.FontSpec.IsBold = true;
            myPane.XAxis.Scale.FontSpec.Family = fontName;
            myPane.XAxis.Scale.IsUseTenPower = false;

            Color mainCurveColor;
            if (myPane.CurveList.Count > 0)
            {
                mainCurveColor = myPane.CurveList[0].Color;
            }
            else
            {
                mainCurveColor = Color.Black;
            }

            myPane.YAxis.Title.FontSpec.Family = fontName;
            myPane.YAxis.Title.FontSpec.IsBold = true;
            myPane.YAxis.Title.FontSpec.FontColor = mainCurveColor;
            myPane.YAxis.Scale.FontSpec.FontColor = mainCurveColor;
            myPane.YAxis.Scale.FontSpec.Family = fontName;

            graph.Refresh();
        }

        internal void UICultureChanged(CultureInfo culture)
        {
            cadenceToolStripMenuItem.Text = CommonResources.Text.LabelCadence;
            powerToolStripMenuItem.Text = CommonResources.Text.LabelPower;
            heartRateToolStripMenuItem.Text = CommonResources.Text.LabelHeartRate;
        }

        private void SetupSecondaryAxes(IList<string> selected)
        {
            GraphPane myPane = zedChart.GraphPane;
            // Clear Y2AxisList so that when we re-add them, they'll all be 'reset' and in the right order
            myPane.Y2AxisList.Clear();

            // Add secondary axis as necessary to zedGraph
            foreach (string id in selected)
            {
                if (selected.Contains(id) && ColumnDefinition.GetTrackType(id) != ChartType)
                {
                    // Add new axis
                    int yIndex = myPane.AddY2Axis(ColumnDefinition.GetText(id));
                    Y2Axis axis = myPane.Y2AxisList[yIndex];
                    axis.Tag = id;

                    Color color = ColumnDefinition.GetTrackColor(id);
                    axis.Title.FontSpec.FontColor = color;
                    axis.Scale.FontSpec.FontColor = color;

                    axis.MajorGrid.IsVisible = false;
                    axis.MajorGrid.Color = myPane.XAxis.MajorGrid.Color;
                    axis.MajorGrid.DashOff = myPane.XAxis.MajorGrid.DashOn;
                    axis.MajorGrid.DashOff = myPane.YAxis.MajorGrid.DashOn;
                    axis.MajorGrid.IsZeroLine = false;
                    axis.IsAxisSegmentVisible = true;

                    axis.MinorTic.IsAllTics = false;
                    axis.MajorTic.IsAllTics = false;
                    axis.MajorTic.IsOutside = true;

                    // Setup Text Appearance
                    string fontName = "Microsoft Sans Sarif";
                    axis.Title.FontSpec.Family = fontName;
                    axis.Title.FontSpec.IsBold = true;
                    axis.Scale.FontSpec.Family = fontName;

                    axis.IsVisible = true;
                }
            }
        }

        #endregion

        #region Event Handlers

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            Util.SaveImage dlg = new Util.SaveImage();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
                zedChart.GetImage().Save(dlg.FileName, dlg.ImageFormat);
            }

            dlg.Dispose();
        }

        private void ZoomChartButton_Click(object sender, EventArgs e)
        {
            zedChart.ZoomOutAll(zedChart.GraphPane);
        }

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            zedChart.ZoomPane(zedChart.GraphPane, 1.1, zedChart.GraphPane.Chart.Rect.Location, false);
        }

        private void ZoomInButton_Click(object sender, EventArgs e)
        {
            zedChart.ZoomPane(zedChart.GraphPane, 0.9, zedChart.GraphPane.Chart.Rect.Location, false);
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            // TODO: How to export multiple activities...?
            IActivity activity = ZoneFiveSoftware.Common.Visuals.Util.CollectionUtils.GetFirstItemOfType<IActivity>(activities);

            // Nothing to export if activity is empty
            if (activity == null)
            {
                return;
            }

            // Open File Save dialog to create new CSV Document
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = "MeanMax " + activity.StartTime.ToLocalTime().ToString("yyyy-MM-dd");
            saveFile.Filter = "All Files (*.*)|*.*|Comma Separated Values (*.csv)|*.csv";
            saveFile.FilterIndex = 2;
            saveFile.DefaultExt = "csv";
            saveFile.OverwritePrompt = true;

            string comma = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            // Cancel if user doesn't select a file
            if (saveFile.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Export mean-max data
            // TODO: Export related data as well as MM data
            INumericTimeDataSeries timeTrack;
            INumericTimeDataSeries track = MeanMaxCache.GetMeanMaxTrack(activity, ChartType, out timeTrack);
            //Dictionary<INumericTimeDataSeries, string> tracks = new Dictionary<INumericTimeDataSeries, string>();

            //tracks.Add(track, GlobalSettings.Instance.PrimaryChart.ToString());
            //tracks.Add(timeTrack, CommonResources.Text.LabelTime);

            //Utilities.ExportTrack(tracks, saveFile.FileName);
            Utilities.ExportTrack(track, saveFile.FileName);
        }

        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            Maximize.Invoke(sender, e);
        }

        private void ChartBanner_MenuClicked(object sender, EventArgs e)
        {
            mnuDetail.Show(ChartBanner, new Point(ChartBanner.Right - 2, ChartBanner.Bottom), ToolStripDropDownDirection.BelowLeft);
        }

        /// <summary>
        /// Change ChartType (HR, Power, Cadence, etc.) from menu
        /// </summary>
        /// <param name="sender">menu item that was clicked</param>
        /// <param name="e">This item is not used</param>
        private void bnrMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem selected = sender as ToolStripMenuItem;

            for (int i = 0; i < mnuDetail.Items.Count; i++)
            {
                ToolStripMenuItem item = mnuDetail.Items[i] as ToolStripMenuItem;

                if (item != null)
                {
                    if (item != selected)
                    {
                        item.Checked = false;
                    }
                    else
                    {
                        item.Checked = true;
                    }
                }
                else
                {
                    // ToolStrip Separator encountered.  Stop evaluating
                    break;
                }
            }

            ChartBanner.Text = selected.Text.ToString();
            ChartType = (Common.TrackType)Enum.Parse(typeof(Common.TrackType), selected.Tag.ToString());
        }

        /// <summary>
        /// Formats labels for x-axis of ZedChart.
        /// </summary>
        /// <param name="pane">The parameter is not used.</param>
        /// <param name="axis">The parameter is not used.</param>
        /// <param name="val">The parameter is not used.</param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string XScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
        {
            TimeSpan span = new TimeSpan(0, 0, (int)Math.Pow(10, index));
            return (int)span.TotalMinutes + ":" + span.Seconds.ToString("00");
        }

        /// <summary>
        /// Formats the tooltip popups
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="pane">The parameter is not used.</param>
        /// <param name="curve">The curve containing the points</param>
        /// <param name="iPt">The index of the point of interest</param>
        /// <returns>A tooltip string</returns>
        string zedChart_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            string tooltip;
            string id = curve.Tag as string;
            TimeSpan period = new TimeSpan(0, 0, (int)curve[iPt].X);

            tooltip = period.ToString() + "\r\n";
            tooltip += curve[iPt].Y.ToString("0", CultureInfo.CurrentCulture);

            switch (id)
            {
                case ColumnDefinition.cadenceID:
                    tooltip += " " + CommonResources.Text.LabelRPM;
                    break;
                case ColumnDefinition.hrID:
                    tooltip += " " + CommonResources.Text.LabelBPM;
                    break;
                case ColumnDefinition.powerID:
                    tooltip += " " + CommonResources.Text.LabelWatts;
                    break;
                case ColumnDefinition.gradeID:
                    tooltip += " %";
                    break;
            }

            // Add time of occurance if available
            if (curve[iPt].Z != -1)
            {
                TimeSpan time = new TimeSpan(0, 0, (int)curve[iPt].Z);
                tooltip += "\r\n" + CommonResources.Text.LabelTime + ": " + time.ToString();
            }

            return tooltip;
        }

        private void ExtraChartsButton_Click(object sender, EventArgs e)
        {
            ListSettingsDialog listDialog = new ListSettingsDialog();
            ICollection<IListColumnDefinition> available = new List<IListColumnDefinition>();

            available.Add(new ColumnDefinition(ColumnDefinition.cadenceID, 100));
            available.Add(new ColumnDefinition(ColumnDefinition.gradeID, 100));
            available.Add(new ColumnDefinition(ColumnDefinition.hrID, 100));
            available.Add(new ColumnDefinition(ColumnDefinition.powerID, 100));

            List<string> selected = new List<string>();

            foreach (string id in GlobalSettings.Instance.ChartLines)
            {
                foreach (ColumnDefinition column in available)
                {
                    if (id == column.Id)
                    {
                        selected.Add(column.Id);
                        break;
                    }
                }
            }

            listDialog.AvailableColumns = available;
            listDialog.SelectedColumns = selected;
            listDialog.Text = CommonResources.Text.LabelCharts;

            listDialog.SelectedItemListLabel = Resources.Strings.Label_SelectedCharts;
            listDialog.AddButtonLabel = CommonResources.Text.ActionAdd;
            listDialog.AllowFixedColumnSelect = false;
            listDialog.AllowZeroSelected = true;
            listDialog.Icon = Utilities.GetIcon(Images.Charts);

            listDialog.ThemeChanged(PluginMain.GetApplication().VisualTheme);

            if (listDialog.ShowDialog() == DialogResult.OK)
            {
                GlobalSettings.Instance.ChartLines = listDialog.SelectedColumns as List<string>;

                SetupSecondaryAxes(listDialog.SelectedColumns);

                RefreshPage();
            }

            listDialog.Close();
            listDialog.Dispose();

            return;
        }

        #endregion

        #region Events

        internal event EventHandler Maximize;

        #endregion
    }
}
