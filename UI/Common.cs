namespace MeanMax.UI
{
    using System.Drawing;
    using ZoneFiveSoftware.Common.Visuals;

    public class Common
    {
        public static readonly Color ColorCadence = Color.FromArgb(78, 154, 6);
        public static readonly Color ColorElevation = Color.FromArgb(143, 89, 2);
        public static readonly Color ColorGrade = Color.FromArgb(193, 125, 17);
        public static readonly Color ColorHR = Color.FromArgb(204, 0, 0);
        public static readonly Color ColorPower = Color.FromArgb(92, 53, 102);
        public static readonly Color ColorSpeed = Color.FromArgb(32, 74, 135);
    
        public enum TrackType
        {
            Power, HR, Cadence, Grade
        }

        public static string GetText(TrackType type)
        {
            switch (type)
            {
                case TrackType.Cadence:
                    return CommonResources.Text.LabelCadence;
                case TrackType.Grade:
                    return CommonResources.Text.LabelGrade;
                case TrackType.HR:
                    return CommonResources.Text.LabelHeartRate;
                case TrackType.Power:
                    return CommonResources.Text.LabelPower;
            }

            return type.ToString();
        }

        /// <summary>
        /// Gets the default color associated with a specific track type
        /// </summary>
        /// <param name="trackType"></param>
        /// <returns>Color</returns>
        public static Color GetColor(TrackType trackType)
        {
            switch (trackType)
            {
                case Common.TrackType.Cadence:
                    return Common.ColorCadence;
                case Common.TrackType.Grade:
                    return Common.ColorGrade;
                case Common.TrackType.HR:
                    return Common.ColorHR;
                case Common.TrackType.Power:
                    return Common.ColorPower;
                default:
                    return Color.Black;
            }
        }
    }
}
