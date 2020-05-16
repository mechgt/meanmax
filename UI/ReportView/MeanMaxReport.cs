namespace MeanMax.UI.ReportView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Forms;
    using ZoneFiveSoftware.Common.Visuals;
    using ZoneFiveSoftware.Common.Visuals.Fitness;

    class MeanMaxReport : IDetailPage
    {
        #region Fields

        private static bool visible;
        private static bool maximized;
        private static MeanMaxReport instance;
        private MeanMaxReportControl control;

        /// <summary>
        /// Type of chart to be displayed.  Setting comes from menu selection.
        /// </summary>
        private static Common.TrackType chartBasis = Common.TrackType.HR;

        #endregion

        #region Properties

        public static MeanMaxReport Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating TL's mode: HR or Power.  HR and Power are currently the only valid modes.  Invalid modes will result in HR mode.
        /// </summary>
        internal static Common.TrackType ChartBasis
        {
            get
            {
                return chartBasis;
            }

            set
            {
                if (chartBasis != value)
                {
                    chartBasis = value;
                }
            }
        }

        internal bool Visible
        {
            get { return visible; }
        }

        /// <summary>
        /// Gets the active view as a reports view
        /// </summary>
        internal IActivityReportsView View
        {
            get
            {
                IActivityReportsView view = PluginMain.GetApplication().ActiveView as IActivityReportsView;
                return view;
            }
        }

        #endregion

        #region Constructors

        public MeanMaxReport()
        {
            instance = this;
        }
        #endregion

        #region IDialogPage Members

        public Control CreatePageControl()
        {
            if (control == null)
            {
                control = new MeanMaxReportControl();
            }

            return control;
        }

        public bool HidePage()
        {
            visible = false;
            PluginMain.GetApplication().ActiveView.PropertyChanged -= new PropertyChangedEventHandler(ActiveView_PropertyChanged);
            return true;
        }

        public string PageName
        {
            get
            {
                return Resources.Strings.Label_MeanMax;
            }
        }

        public void ShowPage(string bookmark)
        {
            visible = true;
            PluginMain.GetApplication().ActiveView.PropertyChanged += new PropertyChangedEventHandler(ActiveView_PropertyChanged);
            View.ActiveReport.PropertyChanged += new PropertyChangedEventHandler(ActiveView_PropertyChanged);
            control.RefreshPage();
        }

        void ActiveView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubTitle" || e.PropertyName == "ActiveReport")
            {
                control.RefreshPage();
            }
        }

        public IPageStatus Status
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            CreatePageControl();
            control.ThemeChanged(visualTheme);
        }

        public string Title
        {
            get
            {
                return Resources.Strings.Label_MeanMax;
            }
        }

        public void UICultureChanged(CultureInfo culture)
        {
            control.UICultureChanged(culture);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDetailPage Members

        public Guid Id
        {
            get { return GUIDs.MeanMaxView; }
        }

        public bool MenuEnabled
        {
            get { return true; }
        }

        public IList<string> MenuPath
        {
            get { return new List<string> { CommonResources.Text.LabelPower }; }
        }

        public bool MenuVisible
        {
            get
            {
                return true;
            }
        }

        public bool PageMaximized
        {
            get
            {
                return maximized;
            }

            set
            {
                maximized = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("PageMaximized"));
                }
            }
        }

        #endregion
    }
}
