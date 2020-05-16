using System;
using System.Globalization;
using System.Windows.Forms;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Util;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace MeanMax.UI.DetailPage
{
    public class MeanMaxDetail : IDetailPage
    {
        #region Fields

        private MeanMaxDetailControl control;
        private bool maximized;
        private Guid id;
        private IView view;

        #endregion

        #region Properties

        internal bool IsReportView
        {
            get { return view.Id == GUIDs.ActivityReportsView; }
        }

        internal bool IsDailyActivityView
        {
            get { return view.Id == GUIDs.DailyActivityView; }
        }

        #endregion

        #region Constructor

        public MeanMaxDetail(IDailyActivityView view)
        {
            this.view = view;
            id = GUIDs.MeanMaxDetail;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles changing selected activities (handles both Daily Activity view and Report View
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnViewSelectedItemsChanged(object sender, EventArgs e)
        {
            // Get selected activities
            IEnumerable<IActivity> activities = GetActivities();

            // Set activities in detail pane
            SetActivities(activities);
        }

        void ActiveReport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetActivities(GetActivities(true));
        }

        void control_Maximize(object sender, EventArgs e)
        {
            PageMaximized = !PageMaximized;
        }

        #endregion

        public void SetActivities(IEnumerable<IActivity> activities)
        {
            if (control != null)
            {
                control.SetActivities(activities);
            }
        }

        /// <summary>
        /// Get activities from current view
        /// </summary>
        /// <param name="all">Get All activities if True, or just selected activities if False.
        /// NOTE: This only applies to Reports View.</param>
        /// <returns>Activities from this page's associated view (defined in constructor)</returns>
        internal IEnumerable<IActivity> GetActivities(bool all)
        {
            IList<IActivity> activities = new List<IActivity>();

            // Prevent null ref error during startup
            if (PluginMain.GetApplication().Logbook == null ||
                PluginMain.GetApplication().ActiveView == null)
            {
                return activities;
            }

            IView view = PluginMain.GetApplication().ActiveView;

            if (view != null && IsDailyActivityView)
            {
                IDailyActivityView activityView = view as IDailyActivityView;
                activities = CollectionUtils.GetAllContainedItemsOfType<IActivity>(activityView.SelectionProvider.SelectedItems);
            }
            else if (view != null && IsReportView)
            {
                IActivityReportsView reportsView = view as IActivityReportsView;

                if (all)
                {
                    activities = reportsView.ActiveReport.Activities;
                }
                else
                {
                    activities = CollectionUtils.GetAllContainedItemsOfType<IActivity>(reportsView.SelectionProvider.SelectedItems);
                }
            }

            return activities;
        }

        /// <summary>
        /// Get selected activities from current view
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<IActivity> GetActivities()
        {
            return GetActivities(false);
        }

        #region IDialogPage Members

        public Control CreatePageControl()
        {
            if (control == null)
            {
                control = new MeanMaxDetailControl();
                SetActivities(null);
            }

            return control;
        }

        public bool HidePage()
        {
            // Active view has changed by the time this is called so we need to use the view
            //  that is assigned to us via the constructor.
            if (view != null && Id == GUIDs.MeanMaxDetail)
            {
                IDailyActivityView activityView = view as IDailyActivityView;
                activityView.SelectionProvider.SelectedItemsChanged -= OnViewSelectedItemsChanged;
            }
            else if (view != null && Id == GUIDs.MeanMaxReport)
            {
                IActivityReportsView reportsView = view as IActivityReportsView;
                reportsView.SelectionProvider.SelectedItemsChanged -= OnViewSelectedItemsChanged;
                reportsView.PropertyChanged -= ActiveReport_PropertyChanged;
            }

            CreatePageControl();
            control.Maximize -= control_Maximize;

            control.IsVisible = false;
            return true;
        }

        public string PageName
        {
            get { return Resources.Strings.Label_MeanMax; }
        }

        public void ShowPage(string bookmark)
        {
            CreatePageControl();

            // This requires use of the constructor view (as opposed to ActiveView)
            if (view != null && Id == GUIDs.MeanMaxDetail)
            {
                IDailyActivityView activityView = view as IDailyActivityView;
                activityView.SelectionProvider.SelectedItemsChanged += OnViewSelectedItemsChanged;
            }

            control.IsVisible = true;
            control.Maximize += new EventHandler(control_Maximize);
            OnViewSelectedItemsChanged(null, null);
        }

        public IPageStatus Status
        {
            get { throw new NotImplementedException(); }
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            if (control != null)
            {
                control.ThemeChanged(visualTheme);
            }
        }

        public string Title
        {
            get { return Resources.Strings.Label_MeanMax; }
        }

        public void UICultureChanged(CultureInfo culture)
        {
            control = CreatePageControl() as MeanMaxDetailControl;
            control.UICultureChanged(culture);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDetailPage Members

        public Guid Id
        {
            get { return id; }
        }

        public bool MenuEnabled
        {
            get { return true; }
        }

        public IList<string> MenuPath
        {
            get { return null; }
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
                    control.MaximizePage(maximized);
                }
            }
        }

        #endregion
    }
}
