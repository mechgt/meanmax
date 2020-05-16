// <copyright file="ExtendActions.cs" company="N/A">
// Copyright (c) 2008 All Right Reserved
// </copyright>
// <author>mechgt</author>
// <email>mechgt@gmail.com</email>
// <date>2008-12-23</date>
namespace MeanMax.UI
{
    using System.Collections.Generic;
    using ZoneFiveSoftware.Common.Data.Fitness;
    using ZoneFiveSoftware.Common.Visuals;
    using ZoneFiveSoftware.Common.Visuals.Fitness;

    class Extend : IExtendActivityDetailPages, IExtendViews, IExtendActivityReportPages
    {
        #region IExtendActivityDetailPages Members

        public IList<IDetailPage> GetDetailPages(IDailyActivityView view, ExtendViewDetailPages.Location location)
        {
            return new IDetailPage[] { new DetailPage.MeanMaxDetail(view) };
        }

        #endregion

        #region IExtendViews Members

        public IList<IView> Views
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IExtendActivityReportPages Members

        public IList<IDetailPage> GetReportPages(IActivityReportsView view, ExtendViewDetailPages.Location location)
        {
            return new List<IDetailPage> { new ReportView.MeanMaxReport() };
        }

        #endregion
    }
}
