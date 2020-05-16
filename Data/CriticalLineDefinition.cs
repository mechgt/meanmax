using System.Xml.Serialization;
using ZoneFiveSoftware.Common.Data.Measurement;
using System;
using System.Drawing;

namespace MeanMax.Data
{
    [XmlRootAttribute(ElementName = "CriticalPowerLine", IsNullable = false)]
    public class CriticalLineDefinition
    {
        private Guid id;

        public CriticalLineDefinition()
        { }

        public CriticalLineDefinition(float value, Time.TimeRange units)
            : this(value, units, false)
        { }

        public CriticalLineDefinition(float value, Time.TimeRange units, bool selected)
        {
            switch (units)
            {
                case Time.TimeRange.Second:
                    Seconds = (int)value;
                    break;
                case Time.TimeRange.Minute:
                    Seconds = (int)TimeSpan.FromMinutes(value).TotalSeconds;
                    break;
                case Time.TimeRange.Hour:
                    Seconds = (int)TimeSpan.FromHours(value).TotalSeconds;
                    break;
                default:
                    // Error: Invalid time range
                    return;
            }

            Selected = selected;
            DisplayUnit = units;
        }

        public string ReferenceId
        {
            get
            {
                if (id == null || id == Guid.Empty)
                {
                    id = Guid.NewGuid();
                }

                return id.ToString("D");
            }
            set { id = new Guid(value); }
        }

        public string Name
        {
            get
            {
                int time;

                switch (DisplayUnit)
                {
                    default:
                    // Error: Invalid time range
                    case Time.TimeRange.Second:
                        time = Seconds;
                        break;
                    case Time.TimeRange.Minute:
                        time = (int)TimeSpan.FromSeconds(Seconds).TotalMinutes;
                        break;
                    case Time.TimeRange.Hour:
                        time = (int)TimeSpan.FromSeconds(Seconds).TotalHours;
                        break;
                }

                return string.Format("{0} {1}", time, Time.LabelAbbr(DisplayUnit));
            }
        }

        public int Seconds { get; set; }
        public Time.TimeRange DisplayUnit { get; set; }
        public bool Selected { get; set; }
        public Color LineColor { get; set; }

        public override bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
