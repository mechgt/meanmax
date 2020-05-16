namespace MeanMax.UI.ReportView
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using MeanMax.Data;
    using MeanMax.UI;
    using MeanMax.Util;
    using ZedGraph;
    using ZoneFiveSoftware.Common.Data;
    using ZoneFiveSoftware.Common.Data.Fitness;
    using ZoneFiveSoftware.Common.Data.Measurement;
    using ZoneFiveSoftware.Common.Visuals;
    using ZoneFiveSoftware.Common.Visuals.Chart;
    using ZoneFiveSoftware.Common.Visuals.Forms;
    using MeanMax.Resources;
    using ZoneFiveSoftware.Common.Visuals.Fitness;

    public partial class MeanMaxReportControl : UserControl
    {
        #region Fields

        private int blinkCount;
        private int progressLine;

        #endregion

        #region Constructor

        public MeanMaxReportControl()
        {
            InitializeComponent();

            //zedChart.GraphPane.XAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(XScaleFormatEvent);
            zedChart.PointValueEvent += new ZedGraphControl.PointValueHandler(zedChart_PointValueEvent);
            MeanMaxCache.ProgressUpdated += new System.ComponentModel.PropertyChangedEventHandler(MeanMaxCache_ProgressUpdated);

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
        /// Set Activities to be used for calculation
        /// </summary>
        internal IEnumerable<IActivity> Activities
        {
            get
            {
                IActivityReportsView view = PluginMain.GetApplication().ActiveView as IActivityReportsView;

                if (view != null)
                {
                    return view.ActiveReport.Activities;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the chart type.  Note that chart is automatically 
        /// refreshed any time this value is updated.
        /// </summary>
        internal Common.TrackType ChartType
        {
            get
            {
                return GlobalSettings.Instance.ReportChart;
            }

            set
            {
                GlobalSettings.Instance.ReportChart = value;
                RefreshPage();
            }
        }

        #endregion

        #region Methods

        public void RefreshPage()
        {
            if (Activities == null)
            {
                // Nothing to do.  Exit.
                return;
            }

            // Update display
            bnrReport.Text = MeanMax.Resources.Strings.Label_MeanMax + ": " + Common.GetText(GlobalSettings.Instance.ReportChart);

            // Track list: One track per selected line.
            Dictionary<CriticalLineDefinition, INumericTimeDataSeries> trackList = new Dictionary<CriticalLineDefinition, INumericTimeDataSeries>();
            List<Color> colors = Utilities.Rainbow(GlobalSettings.Instance.SelectedCriticalLines.Count, 255);
            progressLine = 0;

            foreach (CriticalLineDefinition line in GlobalSettings.Instance.CriticalPowerLines)
            {
                if (line.Selected)
                {
                    // Assign rainbowed line color
                    line.LineColor = colors[progressLine++];

                    // Get critical track described by this line from all Activities
                    INumericTimeDataSeries criticalTrack = MeanMaxCache.GetCriticalTrack(Activities,
                                                                                    GlobalSettings.Instance.ReportChart,
                                                                                    line.Seconds);
                    // Add track to list
                    trackList.Add(line, criticalTrack);
                }
            }

            // Display tracks on graph
            updateZedGraph(trackList, zedChart, GlobalSettings.Instance.ReportChart);

            ZoomChartButton_Click(null, null);
        }

        internal void ThemeChanged(ITheme visualTheme)
        {
            zedThemeChanged(visualTheme, zedChart);
        }

        internal void UICultureChanged(CultureInfo culture)
        {
            bnrReport.Text =
            cadenceToolStripMenuItem.Text = CommonResources.Text.LabelCadence;
            powerToolStripMenuItem.Text = CommonResources.Text.LabelPower;
            heartRateToolStripMenuItem.Text = CommonResources.Text.LabelHeartRate;
        }

        /// <summary>
        /// Add 'track' to 'graph' and apply labels based on 'chartType'
        /// </summary>
        /// <param name="track">Data track</param>
        /// <param name="graph">Which graph to stick the data on</param>
        /// <param name="chartType">This determines the labeling, coloring, etc. (all appearance related)</param>
        internal static void updateZedGraph(Dictionary<CriticalLineDefinition, INumericTimeDataSeries> tracks, ZedGraphControl graph, Common.TrackType chartType)
        {
            GraphPane myPane = graph.GraphPane;
            myPane.XAxis.Title.Text = CommonResources.Text.LabelDate;
            myPane.XAxis.Type = AxisType.Date;

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

            LineItem curve;
            myPane.CurveList.Clear();
            myPane.YAxis.Title.FontSpec.FontColor = mainCurveColor;
            myPane.YAxis.Scale.FontSpec.FontColor = mainCurveColor;

            // Add each critical values chart
            foreach (KeyValuePair<CriticalLineDefinition, INumericTimeDataSeries> track in tracks)
            {
                PointPairList zedTrack = new PointPairList();
                DateTime itemDate;
                // Assemble associated track data
                foreach (ITimeValueEntry<float> item in track.Value)
                {
                    itemDate = track.Value.EntryDateTime(item);
                    zedTrack.Add(new XDate(itemDate), item.Value);
                }

                // Setup display properties of associated track
                Color color = track.Key.LineColor;
                curve = myPane.AddCurve(track.Key.Name, zedTrack, color, SymbolType.None);
                curve.Line.Width = 1f;
                curve.Line.Fill.Type = FillType.None;
                curve.Line.IsAntiAlias = true;
                //curve.Line.IsSmooth = true;
                //curve.Line.SmoothTension = .15f;
                curve.Tag = track.Key.ReferenceId;
            }

            if (tracks.Count > 0)
            {
                graph.AxisChange();
            }

            graph.Refresh();
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
            myPane.Border.IsVisible = true;
            myPane.Border.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            myPane.Border.Color = visualTheme.Border;

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
            myPane.XAxis.IsAxisSegmentVisible = true;
            myPane.YAxis.IsAxisSegmentVisible = true;
            myPane.XAxis.MajorGrid.IsZeroLine = false;
            myPane.YAxis.MajorGrid.IsZeroLine = false;

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

            Color mainCurveColor = Common.GetColor(GlobalSettings.Instance.ReportChart);

            myPane.YAxis.Title.FontSpec.Family = fontName;
            myPane.YAxis.Title.FontSpec.IsBold = true;
            myPane.YAxis.Title.FontSpec.FontColor = mainCurveColor;
            myPane.YAxis.Scale.FontSpec.FontColor = mainCurveColor;
            myPane.YAxis.Scale.FontSpec.Family = fontName;

            graph.Refresh();
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
            GraphPane myPane = zedChart.GraphPane;

            if (myPane.CurveList.Count > 0)
            {
                double xMax = double.MinValue, yMax = double.MinValue, xMin = double.MaxValue, yMin = double.MaxValue, xMaxCurve, yMaxCurve, xMinCurve, yMinCurve;


                foreach (LineItem curve in myPane.CurveList)
                {
                    curve.GetRange(out xMinCurve, out xMaxCurve, out yMinCurve, out yMaxCurve, false, false, myPane);

                    xMin = Math.Min(xMin, xMinCurve);
                    xMax = Math.Max(xMax, xMaxCurve);
                    yMin = Math.Min(yMin, yMinCurve);
                    yMax = Math.Max(yMax, yMaxCurve);
                }

                myPane.YAxis.Scale.Max = yMax;
                myPane.YAxis.Scale.Min = yMin;
                myPane.XAxis.Scale.Max = xMax;
                myPane.XAxis.Scale.Min = xMin;
            }
            else
            {
                zedChart.ZoomOutAll(zedChart.GraphPane);
            }

            if (zedChart.GraphPane.CurveList.Count > 0)
            {
                zedChart.AxisChange();
            }

            zedChart.Refresh();
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
            // Nothing to export if activity is empty
            if (Activities == null)
            {
                return;
            }

            // Open File Save dialog to create new CSV Document
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = MeanMaxReport.Instance.Title + ".csv";
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

            Dictionary<INumericTimeDataSeries, string> lines = new Dictionary<INumericTimeDataSeries, string>();

            foreach (CriticalLineDefinition line in GlobalSettings.Instance.CriticalPowerLines)
            {
                if (line.Selected)
                {
                    lines.Add(MeanMaxCache.GetCriticalTrack(Activities, GlobalSettings.Instance.ReportChart, line.Seconds), line.Name);
                }
            }

            Utilities.ExportTrack(lines, saveFile.FileName);
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
            // TODO: Update tooltip for report view
            string tooltip = string.Empty;
            string id = curve.Tag as string;
            DateTime date = XDate.XLDateToDateTime(curve[iPt].X);

            // 5 sec
            foreach (CriticalLineDefinition def in GlobalSettings.Instance.CriticalPowerLines)
            {
                if (def.ReferenceId == id)
                {
                    tooltip = def.Name + "\r\n";
                    break;
                }
            }

            // 11/13/2010
            tooltip += date.ToShortDateString() + "\r\n";

            // 508 watts
            tooltip += curve[iPt].Y.ToString("0", CultureInfo.CurrentCulture);
            switch (GlobalSettings.Instance.ReportChart)
            {
                case Common.TrackType.Cadence:
                    tooltip += " " + CommonResources.Text.LabelRPM;
                    break;
                case Common.TrackType.HR:
                    tooltip += " " + CommonResources.Text.LabelBPM;
                    break;
                case Common.TrackType.Power:
                    tooltip += " " + CommonResources.Text.LabelWatts;
                    break;
                case Common.TrackType.Grade:
                    tooltip += " %";
                    break;
            }

            return tooltip;
        }

        private void ExtraChartsButton_Click(object sender, EventArgs e)
        {
            ListSettingsDialog listDialog = new ListSettingsDialog();
            ICollection<IListColumnDefinition> available = new List<IListColumnDefinition>();
            List<string> selected = new List<string>();

            // Define available and selected items 
            foreach (CriticalLineDefinition line in GlobalSettings.Instance.CriticalPowerLines)
            {
                available.Add(new ColumnDefinition(line.Name, 100));

                if (line.Selected)
                {
                    selected.Add(line.Name);
                }
            }

            // Setup list selection dialog
            listDialog.AvailableColumns = available;
            listDialog.SelectedColumns = selected;
            listDialog.Text = CommonResources.Text.LabelCharts;
            listDialog.SelectedItemListLabel = Resources.Strings.Label_SelectedCharts;
            listDialog.AddButtonLabel = CommonResources.Text.ActionAdd;
            listDialog.AllowFixedColumnSelect = false;
            listDialog.AllowZeroSelected = false;
            listDialog.Icon = Utilities.GetIcon(Images.Charts);
            listDialog.ThemeChanged(PluginMain.GetApplication().VisualTheme);

            // Popup list dialog
            if (listDialog.ShowDialog() == DialogResult.OK)
            {
                selected = listDialog.SelectedColumns as List<string>;

                // Save selected lines
                int countSelected = 0;
                foreach (CriticalLineDefinition line in GlobalSettings.Instance.CriticalPowerLines)
                {
                    // NOTE: Eval limitation: Number of charts
                    if (selected.Contains(line.Name))
                    {
                        countSelected++;
                        line.Selected = true;
                    }
                    else
                    {
                        line.Selected = false;
                    }
                }

                RefreshPage();
            }

            listDialog.Close();
            listDialog.Dispose();

            return;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            RefreshPage();
        }

        private void bnrReport_MenuClicked(object sender, EventArgs e)
        {
            mnuDetail.Show(bnrReport, new Point(bnrReport.Right - 2, bnrReport.Bottom), ToolStripDropDownDirection.BelowLeft);
        }

        /// <summary>
        /// Change ChartType (HR, Power, Cadence, etc.) from menu
        /// </summary>
        /// <param name="sender">menu item that was clicked</param>
        /// <param name="e">This item is not used</param>
        private void bannerMenuItem_Click(object sender, EventArgs e)
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

            bnrReport.Text = selected.Text.ToString();
            ChartType = (Common.TrackType)Enum.Parse(typeof(Common.TrackType), selected.Tag.ToString());
        }

        void MeanMaxCache_ProgressUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (MeanMaxCache.Progress <= 0)
            {
                bnrReport.Text = MeanMax.Resources.Strings.Label_MeanMax + ": " + Common.GetText(GlobalSettings.Instance.ReportChart);
                return;
            }

            float value = MeanMaxCache.Progress + (progressLine - 1) * 100;
            value = value / (float)GlobalSettings.Instance.SelectedCriticalLines.Count;
            bnrReport.Text = MeanMax.Resources.Strings.Label_MeanMax + ": " + Common.GetText(GlobalSettings.Instance.ReportChart) + string.Format(" ({0}%)", (int)value);
        }

        #endregion
    }
}
