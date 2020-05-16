namespace MeanMax.UI.ReportView
{
    partial class MeanMaxReportControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeanMaxReportControl));
            this.zedChart = new ZedGraph.ZedGraphControl();
            this.bnrReport = new ZoneFiveSoftware.Common.Visuals.ActionBanner();
            this.ButtonPanel = new ZoneFiveSoftware.Common.Visuals.Panel();
            this.btnRefresh = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ZoomInButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ZoomOutButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ZoomChartButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ExportButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.ExtraChartsButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.SaveImageButton = new ZoneFiveSoftware.Common.Visuals.Button();
            this.mnuDetail = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.heartRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.powerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cadenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonPanel.SuspendLayout();
            this.mnuDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // zedChart
            // 
            this.zedChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.zedChart.IsShowPointValues = true;
            this.zedChart.IsZoomOnMouseCenter = true;
            this.zedChart.Location = new System.Drawing.Point(0, 45);
            this.zedChart.Name = "zedChart";
            this.zedChart.ScrollGrace = 0D;
            this.zedChart.ScrollMaxX = 0D;
            this.zedChart.ScrollMaxY = 0D;
            this.zedChart.ScrollMaxY2 = 0D;
            this.zedChart.ScrollMinX = 0D;
            this.zedChart.ScrollMinY = 0D;
            this.zedChart.ScrollMinY2 = 0D;
            this.zedChart.Size = new System.Drawing.Size(867, 441);
            this.zedChart.TabIndex = 3;
            // 
            // bnrReport
            // 
            this.bnrReport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bnrReport.BackColor = System.Drawing.Color.Transparent;
            this.bnrReport.HasMenuButton = true;
            this.bnrReport.Location = new System.Drawing.Point(0, 0);
            this.bnrReport.Name = "bnrReport";
            this.bnrReport.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.bnrReport.Size = new System.Drawing.Size(867, 24);
            this.bnrReport.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header2;
            this.bnrReport.TabIndex = 4;
            this.bnrReport.UseStyleFont = true;
            this.bnrReport.MenuClicked += new System.EventHandler(this.bnrReport_MenuClicked);
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonPanel.BackColor = System.Drawing.Color.Transparent;
            this.ButtonPanel.Border = ZoneFiveSoftware.Common.Visuals.ControlBorder.Style.Square;
            this.ButtonPanel.BorderColor = System.Drawing.Color.Gray;
            this.ButtonPanel.Controls.Add(this.btnRefresh);
            this.ButtonPanel.Controls.Add(this.ZoomInButton);
            this.ButtonPanel.Controls.Add(this.ZoomOutButton);
            this.ButtonPanel.Controls.Add(this.ZoomChartButton);
            this.ButtonPanel.Controls.Add(this.ExportButton);
            this.ButtonPanel.Controls.Add(this.ExtraChartsButton);
            this.ButtonPanel.Controls.Add(this.SaveImageButton);
            this.ButtonPanel.HeadingBackColor = System.Drawing.Color.LightBlue;
            this.ButtonPanel.HeadingFont = null;
            this.ButtonPanel.HeadingLeftMargin = 0;
            this.ButtonPanel.HeadingText = null;
            this.ButtonPanel.HeadingTextColor = System.Drawing.Color.Black;
            this.ButtonPanel.HeadingTopMargin = 0;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 23);
            this.ButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(867, 24);
            this.ButtonPanel.TabIndex = 7;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnRefresh.CenterImage = ((System.Drawing.Image)(resources.GetObject("btnRefresh.CenterImage")));
            this.btnRefresh.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnRefresh.HyperlinkStyle = false;
            this.btnRefresh.ImageMargin = 2;
            this.btnRefresh.LeftImage = null;
            this.btnRefresh.Location = new System.Drawing.Point(697, 0);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(0);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.PushStyle = true;
            this.btnRefresh.RightImage = null;
            this.btnRefresh.Size = new System.Drawing.Size(24, 24);
            this.btnRefresh.TabIndex = 18;
            this.btnRefresh.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnRefresh.TextLeftMargin = 2;
            this.btnRefresh.TextRightMargin = 2;
            this.btnRefresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // ZoomInButton
            // 
            this.ZoomInButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomInButton.BackColor = System.Drawing.Color.Transparent;
            this.ZoomInButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.ZoomInButton.CenterImage = ((System.Drawing.Image)(resources.GetObject("ZoomInButton.CenterImage")));
            this.ZoomInButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ZoomInButton.HyperlinkStyle = false;
            this.ZoomInButton.ImageMargin = 2;
            this.ZoomInButton.LeftImage = null;
            this.ZoomInButton.Location = new System.Drawing.Point(841, 0);
            this.ZoomInButton.Margin = new System.Windows.Forms.Padding(0);
            this.ZoomInButton.Name = "ZoomInButton";
            this.ZoomInButton.PushStyle = true;
            this.ZoomInButton.RightImage = null;
            this.ZoomInButton.Size = new System.Drawing.Size(24, 24);
            this.ZoomInButton.TabIndex = 0;
            this.ZoomInButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.ZoomInButton.TextLeftMargin = 2;
            this.ZoomInButton.TextRightMargin = 2;
            this.ZoomInButton.Click += new System.EventHandler(this.ZoomInButton_Click);
            // 
            // ZoomOutButton
            // 
            this.ZoomOutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomOutButton.BackColor = System.Drawing.Color.Transparent;
            this.ZoomOutButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.ZoomOutButton.CenterImage = ((System.Drawing.Image)(resources.GetObject("ZoomOutButton.CenterImage")));
            this.ZoomOutButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ZoomOutButton.HyperlinkStyle = false;
            this.ZoomOutButton.ImageMargin = 2;
            this.ZoomOutButton.LeftImage = null;
            this.ZoomOutButton.Location = new System.Drawing.Point(817, 0);
            this.ZoomOutButton.Margin = new System.Windows.Forms.Padding(0);
            this.ZoomOutButton.Name = "ZoomOutButton";
            this.ZoomOutButton.PushStyle = true;
            this.ZoomOutButton.RightImage = null;
            this.ZoomOutButton.Size = new System.Drawing.Size(24, 24);
            this.ZoomOutButton.TabIndex = 0;
            this.ZoomOutButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.ZoomOutButton.TextLeftMargin = 2;
            this.ZoomOutButton.TextRightMargin = 2;
            this.ZoomOutButton.Click += new System.EventHandler(this.ZoomOutButton_Click);
            // 
            // ZoomChartButton
            // 
            this.ZoomChartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomChartButton.BackColor = System.Drawing.Color.Transparent;
            this.ZoomChartButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.ZoomChartButton.CenterImage = global::MeanMax.Resources.Images.ZoomFit;
            this.ZoomChartButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ZoomChartButton.HyperlinkStyle = false;
            this.ZoomChartButton.ImageMargin = 2;
            this.ZoomChartButton.LeftImage = null;
            this.ZoomChartButton.Location = new System.Drawing.Point(793, 0);
            this.ZoomChartButton.Margin = new System.Windows.Forms.Padding(0);
            this.ZoomChartButton.Name = "ZoomChartButton";
            this.ZoomChartButton.PushStyle = true;
            this.ZoomChartButton.RightImage = null;
            this.ZoomChartButton.Size = new System.Drawing.Size(24, 24);
            this.ZoomChartButton.TabIndex = 0;
            this.ZoomChartButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.ZoomChartButton.TextLeftMargin = 2;
            this.ZoomChartButton.TextRightMargin = 2;
            this.ZoomChartButton.Click += new System.EventHandler(this.ZoomChartButton_Click);
            // 
            // ExportButton
            // 
            this.ExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExportButton.BackColor = System.Drawing.Color.Transparent;
            this.ExportButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.ExportButton.CenterImage = ((System.Drawing.Image)(resources.GetObject("ExportButton.CenterImage")));
            this.ExportButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ExportButton.HyperlinkStyle = false;
            this.ExportButton.ImageMargin = 2;
            this.ExportButton.LeftImage = null;
            this.ExportButton.Location = new System.Drawing.Point(745, 0);
            this.ExportButton.Margin = new System.Windows.Forms.Padding(0);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.PushStyle = true;
            this.ExportButton.RightImage = null;
            this.ExportButton.Size = new System.Drawing.Size(24, 24);
            this.ExportButton.TabIndex = 0;
            this.ExportButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.ExportButton.TextLeftMargin = 2;
            this.ExportButton.TextRightMargin = 2;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // ExtraChartsButton
            // 
            this.ExtraChartsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExtraChartsButton.BackColor = System.Drawing.Color.Transparent;
            this.ExtraChartsButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.ExtraChartsButton.CenterImage = global::MeanMax.Resources.Images.Charts;
            this.ExtraChartsButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ExtraChartsButton.HyperlinkStyle = false;
            this.ExtraChartsButton.ImageMargin = 2;
            this.ExtraChartsButton.LeftImage = null;
            this.ExtraChartsButton.Location = new System.Drawing.Point(721, 0);
            this.ExtraChartsButton.Margin = new System.Windows.Forms.Padding(0);
            this.ExtraChartsButton.Name = "ExtraChartsButton";
            this.ExtraChartsButton.PushStyle = true;
            this.ExtraChartsButton.RightImage = null;
            this.ExtraChartsButton.Size = new System.Drawing.Size(24, 24);
            this.ExtraChartsButton.TabIndex = 0;
            this.ExtraChartsButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.ExtraChartsButton.TextLeftMargin = 2;
            this.ExtraChartsButton.TextRightMargin = 2;
            this.ExtraChartsButton.Click += new System.EventHandler(this.ExtraChartsButton_Click);
            // 
            // SaveImageButton
            // 
            this.SaveImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveImageButton.BackColor = System.Drawing.Color.Transparent;
            this.SaveImageButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.SaveImageButton.CenterImage = ((System.Drawing.Image)(resources.GetObject("SaveImageButton.CenterImage")));
            this.SaveImageButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SaveImageButton.HyperlinkStyle = false;
            this.SaveImageButton.ImageMargin = 2;
            this.SaveImageButton.LeftImage = null;
            this.SaveImageButton.Location = new System.Drawing.Point(769, 0);
            this.SaveImageButton.Margin = new System.Windows.Forms.Padding(0);
            this.SaveImageButton.Name = "SaveImageButton";
            this.SaveImageButton.PushStyle = true;
            this.SaveImageButton.RightImage = null;
            this.SaveImageButton.Size = new System.Drawing.Size(24, 24);
            this.SaveImageButton.TabIndex = 0;
            this.SaveImageButton.TextAlign = System.Drawing.StringAlignment.Center;
            this.SaveImageButton.TextLeftMargin = 2;
            this.SaveImageButton.TextRightMargin = 2;
            this.SaveImageButton.Click += new System.EventHandler(this.SaveImageButton_Click);
            // 
            // mnuDetail
            // 
            this.mnuDetail.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.heartRateToolStripMenuItem,
            this.powerToolStripMenuItem,
            this.cadenceToolStripMenuItem});
            this.mnuDetail.Name = "mnuDetailMenu";
            this.mnuDetail.Size = new System.Drawing.Size(130, 70);
            // 
            // heartRateToolStripMenuItem
            // 
            this.heartRateToolStripMenuItem.Name = "heartRateToolStripMenuItem";
            this.heartRateToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.heartRateToolStripMenuItem.Tag = "HR";
            this.heartRateToolStripMenuItem.Text = "Heart Rate";
            this.heartRateToolStripMenuItem.Click += new System.EventHandler(this.bannerMenuItem_Click);
            // 
            // powerToolStripMenuItem
            // 
            this.powerToolStripMenuItem.Checked = true;
            this.powerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.powerToolStripMenuItem.Name = "powerToolStripMenuItem";
            this.powerToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.powerToolStripMenuItem.Tag = "Power";
            this.powerToolStripMenuItem.Text = "Power";
            this.powerToolStripMenuItem.Click += new System.EventHandler(this.bannerMenuItem_Click);
            // 
            // cadenceToolStripMenuItem
            // 
            this.cadenceToolStripMenuItem.Name = "cadenceToolStripMenuItem";
            this.cadenceToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.cadenceToolStripMenuItem.Tag = "Cadence";
            this.cadenceToolStripMenuItem.Text = "Cadence";
            this.cadenceToolStripMenuItem.Click += new System.EventHandler(this.bannerMenuItem_Click);
            // 
            // MeanMaxReportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.bnrReport);
            this.Controls.Add(this.zedChart);
            this.Name = "MeanMaxReportControl";
            this.Size = new System.Drawing.Size(867, 486);
            this.ButtonPanel.ResumeLayout(false);
            this.mnuDetail.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zedChart;
        private ZoneFiveSoftware.Common.Visuals.ActionBanner bnrReport;
        private ZoneFiveSoftware.Common.Visuals.Panel ButtonPanel;
        private ZoneFiveSoftware.Common.Visuals.Button ZoomInButton;
        private ZoneFiveSoftware.Common.Visuals.Button ZoomOutButton;
        private ZoneFiveSoftware.Common.Visuals.Button ZoomChartButton;
        private ZoneFiveSoftware.Common.Visuals.Button ExportButton;
        private ZoneFiveSoftware.Common.Visuals.Button ExtraChartsButton;
        private ZoneFiveSoftware.Common.Visuals.Button SaveImageButton;
        private System.Windows.Forms.ContextMenuStrip mnuDetail;
        private System.Windows.Forms.ToolStripMenuItem heartRateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem powerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cadenceToolStripMenuItem;
        private ZoneFiveSoftware.Common.Visuals.Button btnRefresh;
    }
}
