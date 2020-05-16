using System.Xml.Serialization;
using System;
using System.Globalization;
using MeanMax.UI;
using System.Collections.Generic;
using System.Security.Policy;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data.Measurement;

namespace MeanMax.Data
{
    [XmlRootAttribute(ElementName = "MeanMax", IsNullable = false)]
    public class GlobalSettings
    {
        private static List<string> chartLines;
        private static GlobalSettings settings;
        private static Common.TrackType primaryChart;
        private static Common.TrackType reportChart;
        private static List<CriticalLineDefinition> criticalPowerLines;
        internal static bool Initializing;

        [XmlIgnore]
        public static GlobalSettings Instance
        {
            get
            {
                if (settings == null)
                {
                    settings = new GlobalSettings();
                }

                return settings;
            }
        }

        /// <summary>
        /// Additional lines displayed on Detail View (secondary Y axis)
        /// </summary>
        public List<string> ChartLines
        {
            get
            {
                if (chartLines == null)
                {
                    chartLines = new List<string>();
                }

                return chartLines;
            }
            set
            {
                chartLines = value;
            }
        }

        /// <summary>
        /// Selected chart type for Detail view (power, HR, etc.)
        /// </summary>
        public Common.TrackType PrimaryChart
        {
            get { return primaryChart; }
            set { primaryChart = value; }
        }

        /// <summary>
        /// Selected chart type for report view (power, HR, etc.)
        /// </summary>
        public Common.TrackType ReportChart
        {
            get { return reportChart; }
            set { reportChart = value; }
        }

        public List<CriticalLineDefinition> CriticalPowerLines
        {
            get
            {
                if ((criticalPowerLines == null || criticalPowerLines.Count == 0) && !Initializing)
                {
                    criticalPowerLines = new List<CriticalLineDefinition>{
                    new CriticalLineDefinition(5, Time.TimeRange.Second,true),
                    new CriticalLineDefinition(10, Time.TimeRange.Second,false),
                    new CriticalLineDefinition(20, Time.TimeRange.Second,false),
                    new CriticalLineDefinition(30, Time.TimeRange.Second,true),
                    new CriticalLineDefinition(1, Time.TimeRange.Minute,true),
                    new CriticalLineDefinition(2, Time.TimeRange.Minute,false),
                    new CriticalLineDefinition(5, Time.TimeRange.Minute,true),
                    new CriticalLineDefinition(10, Time.TimeRange.Minute,false),
                    new CriticalLineDefinition(15, Time.TimeRange.Minute,false),
                    new CriticalLineDefinition(20, Time.TimeRange.Minute,false),
                    new CriticalLineDefinition(30, Time.TimeRange.Minute,false),
                    new CriticalLineDefinition(45, Time.TimeRange.Minute,false),
                    new CriticalLineDefinition(1, Time.TimeRange.Hour,false),
                    new CriticalLineDefinition(2, Time.TimeRange.Hour,false),
                    new CriticalLineDefinition(3, Time.TimeRange.Hour)};

                    return criticalPowerLines;
                }

                return criticalPowerLines;
            }
            set
            {
                criticalPowerLines = value;
            }
        }

        [XmlIgnore]
        public List<CriticalLineDefinition> SelectedCriticalLines
        {
            get
            {
                List<CriticalLineDefinition> selected = new List<CriticalLineDefinition>();

                foreach (CriticalLineDefinition line in CriticalPowerLines)
                {
                    if (line.Selected)
                    {
                        selected.Add(line);
                    }
                }

                return selected;
            }
        }

        [XmlIgnore]
        public float TCc
        { get { return 45; } }

        internal static void InitializeSettings()
        {
            chartLines = null;
            primaryChart = Common.TrackType.Power;
            reportChart = Common.TrackType.Power;
            criticalPowerLines = null;
        }
    }
}
