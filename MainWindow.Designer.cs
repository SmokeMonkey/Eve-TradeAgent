using BrightIdeasSoftware;
using System.Drawing;

namespace Eve_TradingAgent
{
    partial class MainWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonLoadOrderList = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.chkBoxShowOutOfReach = new System.Windows.Forms.CheckBox();
            this.lblFluctuationCap = new System.Windows.Forms.Label();
            this.OrderList = new BrightIdeasSoftware.ObjectListView();
            this.Skip = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Status = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Cooldown = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Type = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Quantity = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Price = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Station = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Region = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btnSkipNone = new System.Windows.Forms.Button();
            this.chkBoxShowCooldown = new System.Windows.Forms.CheckBox();
            this.chkBoxShowNeedNoUpdate = new System.Windows.Forms.CheckBox();
            this.chkBoxShowSkippedOrders = new System.Windows.Forms.CheckBox();
            this.btnSkipAll = new System.Windows.Forms.Button();
            this.txtBoxOrderTextFilter = new System.Windows.Forms.TextBox();
            this.grpBoxFilters = new System.Windows.Forms.GroupBox();
            this.btnToggleLog = new System.Windows.Forms.Button();
            this.numUpDownModifyCooldown = new System.Windows.Forms.NumericUpDown();
            this.lblModifyCooldown = new System.Windows.Forms.Label();
            this.lblReloadOrderListInterval = new System.Windows.Forms.Label();
            this.lblProcessInterval = new System.Windows.Forms.Label();
            this.numUpDownReloadInterval = new System.Windows.Forms.NumericUpDown();
            this.numUpDownProcessInterval = new System.Windows.Forms.NumericUpDown();
            this.numUpDownFluctuationCap = new System.Windows.Forms.NumericUpDown();
            this.numUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.lblGapBiggerThan = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.OrderList)).BeginInit();
            this.grpBoxFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownModifyCooldown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownReloadInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownProcessInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownFluctuationCap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonLoadOrderList
            // 
            this.ButtonLoadOrderList.Location = new System.Drawing.Point(12, 12);
            this.ButtonLoadOrderList.Name = "ButtonLoadOrderList";
            this.ButtonLoadOrderList.Size = new System.Drawing.Size(117, 89);
            this.ButtonLoadOrderList.TabIndex = 5;
            this.ButtonLoadOrderList.Text = "Load";
            this.ButtonLoadOrderList.UseVisualStyleBackColor = true;
            this.ButtonLoadOrderList.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(152, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 89);
            this.button2.TabIndex = 6;
            this.button2.Text = "Start Processing";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // chkBoxShowOutOfReach
            // 
            this.chkBoxShowOutOfReach.AutoSize = true;
            this.chkBoxShowOutOfReach.Enabled = false;
            this.chkBoxShowOutOfReach.Location = new System.Drawing.Point(11, 77);
            this.chkBoxShowOutOfReach.Name = "chkBoxShowOutOfReach";
            this.chkBoxShowOutOfReach.Size = new System.Drawing.Size(189, 21);
            this.chkBoxShowOutOfReach.TabIndex = 8;
            this.chkBoxShowOutOfReach.Text = "Show orders out of reach";
            this.chkBoxShowOutOfReach.UseVisualStyleBackColor = true;
            this.chkBoxShowOutOfReach.CheckedChanged += new System.EventHandler(this.chkBoxShowOutOfReach_CheckedChanged);
            // 
            // lblFluctuationCap
            // 
            this.lblFluctuationCap.AutoSize = true;
            this.lblFluctuationCap.Enabled = false;
            this.lblFluctuationCap.Location = new System.Drawing.Point(732, 69);
            this.lblFluctuationCap.Name = "lblFluctuationCap";
            this.lblFluctuationCap.Size = new System.Drawing.Size(183, 17);
            this.lblFluctuationCap.TabIndex = 10;
            this.lblFluctuationCap.Text = "Limit of price fluctuation (%)";
            // 
            // OrderList
            // 
            this.OrderList.AllColumns.Add(this.Skip);
            this.OrderList.AllColumns.Add(this.ID);
            this.OrderList.AllColumns.Add(this.Status);
            this.OrderList.AllColumns.Add(this.Cooldown);
            this.OrderList.AllColumns.Add(this.Type);
            this.OrderList.AllColumns.Add(this.Quantity);
            this.OrderList.AllColumns.Add(this.Price);
            this.OrderList.AllColumns.Add(this.Station);
            this.OrderList.AllColumns.Add(this.Region);
            this.OrderList.AllowColumnReorder = true;
            this.OrderList.CheckBoxes = true;
            this.OrderList.CheckedAspectName = "Skip";
            this.OrderList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Skip,
            this.ID,
            this.Status,
            this.Cooldown,
            this.Type,
            this.Quantity,
            this.Price,
            this.Station,
            this.Region});
            this.OrderList.EmptyListMsg = "No orders detected.";
            this.OrderList.FullRowSelect = true;
            this.OrderList.GridLines = true;
            this.OrderList.HeaderUsesThemes = false;
            this.OrderList.Location = new System.Drawing.Point(249, 121);
            this.OrderList.Name = "OrderList";
            this.OrderList.OwnerDraw = true;
            this.OrderList.ShowFilterMenuOnRightClick = false;
            this.OrderList.ShowItemCountOnGroups = true;
            this.OrderList.Size = new System.Drawing.Size(753, 601);
            this.OrderList.TabIndex = 11;
            this.OrderList.UseCompatibleStateImageBehavior = false;
            this.OrderList.UseFiltering = true;
            this.OrderList.View = System.Windows.Forms.View.Details;
            this.OrderList.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.OrderList_CellRightClick);
            this.OrderList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.OrderList_ItemChecked);
            // 
            // Skip
            // 
            this.Skip.CellPadding = null;
            this.Skip.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Skip.MaximumWidth = 85;
            this.Skip.MinimumWidth = 25;
            this.Skip.Text = "Skip";
            this.Skip.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Skip.ToolTipText = "Orders marked as \"Skip\" by user will not be processed";
            this.Skip.Width = 85;
            // 
            // ID
            // 
            this.ID.AspectName = "ID";
            this.ID.CellPadding = null;
            this.ID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ID.IsEditable = false;
            this.ID.MaximumWidth = 100;
            this.ID.MinimumWidth = 25;
            this.ID.Text = "ID";
            this.ID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ID.ToolTipText = "Unique ID of market order";
            this.ID.Width = 100;
            // 
            // Status
            // 
            this.Status.AspectName = "Status";
            this.Status.CellPadding = null;
            this.Status.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Status.MaximumWidth = 200;
            this.Status.MinimumWidth = 25;
            this.Status.Text = "Status";
            this.Status.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Status.ToolTipText = "Current status of order";
            this.Status.Width = 148;
            // 
            // Cooldown
            // 
            this.Cooldown.CellPadding = null;
            this.Cooldown.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Cooldown.MaximumWidth = 80;
            this.Cooldown.MinimumWidth = 25;
            this.Cooldown.Text = "Cooldown";
            this.Cooldown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Cooldown.ToolTipText = "Orders cannot be modified until cooldown is finished";
            this.Cooldown.Width = 80;
            // 
            // Type
            // 
            this.Type.AspectName = "Type";
            this.Type.CellPadding = null;
            this.Type.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Type.MaximumWidth = 400;
            this.Type.MinimumWidth = 25;
            this.Type.Text = "Type";
            this.Type.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Type.ToolTipText = "The item of the order";
            this.Type.Width = 121;
            // 
            // Quantity
            // 
            this.Quantity.AspectName = "QuantityRemaining";
            this.Quantity.CellPadding = null;
            this.Quantity.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Quantity.MaximumWidth = 300;
            this.Quantity.MinimumWidth = 25;
            this.Quantity.Text = "Quantity";
            this.Quantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Quantity.Width = 134;
            // 
            // Price
            // 
            this.Price.AspectName = "Price";
            this.Price.AspectToStringFormat = "{0:n2} ISK";
            this.Price.CellPadding = null;
            this.Price.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Price.MaximumWidth = 300;
            this.Price.MinimumWidth = 25;
            this.Price.Text = "Price";
            this.Price.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Price.Width = 144;
            // 
            // Station
            // 
            this.Station.AspectName = "Station";
            this.Station.CellPadding = null;
            this.Station.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Station.MaximumWidth = 600;
            this.Station.MinimumWidth = 25;
            this.Station.Text = "Station";
            this.Station.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Station.Width = 267;
            // 
            // Region
            // 
            this.Region.AspectName = "Region";
            this.Region.CellPadding = null;
            this.Region.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Region.MaximumWidth = 100;
            this.Region.MinimumWidth = 25;
            this.Region.Text = "Region";
            this.Region.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Region.Width = 100;
            // 
            // btnSkipNone
            // 
            this.btnSkipNone.Enabled = false;
            this.btnSkipNone.Location = new System.Drawing.Point(12, 350);
            this.btnSkipNone.Name = "btnSkipNone";
            this.btnSkipNone.Size = new System.Drawing.Size(215, 116);
            this.btnSkipNone.TabIndex = 12;
            this.btnSkipNone.Text = "Skip none";
            this.btnSkipNone.UseVisualStyleBackColor = true;
            this.btnSkipNone.Click += new System.EventHandler(this.btnSkipNone_Click);
            // 
            // chkBoxShowCooldown
            // 
            this.chkBoxShowCooldown.AutoSize = true;
            this.chkBoxShowCooldown.Enabled = false;
            this.chkBoxShowCooldown.Location = new System.Drawing.Point(11, 105);
            this.chkBoxShowCooldown.Name = "chkBoxShowCooldown";
            this.chkBoxShowCooldown.Size = new System.Drawing.Size(187, 21);
            this.chkBoxShowCooldown.TabIndex = 13;
            this.chkBoxShowCooldown.Text = "Show orders in cooldown";
            this.chkBoxShowCooldown.UseVisualStyleBackColor = true;
            this.chkBoxShowCooldown.CheckedChanged += new System.EventHandler(this.chkBoxShowCooldown_CheckedChanged);
            // 
            // chkBoxShowNeedNoUpdate
            // 
            this.chkBoxShowNeedNoUpdate.AutoSize = true;
            this.chkBoxShowNeedNoUpdate.Enabled = false;
            this.chkBoxShowNeedNoUpdate.Location = new System.Drawing.Point(11, 133);
            this.chkBoxShowNeedNoUpdate.Name = "chkBoxShowNeedNoUpdate";
            this.chkBoxShowNeedNoUpdate.Size = new System.Drawing.Size(213, 21);
            this.chkBoxShowNeedNoUpdate.TabIndex = 14;
            this.chkBoxShowNeedNoUpdate.Text = "Show orders need no update";
            this.chkBoxShowNeedNoUpdate.UseVisualStyleBackColor = true;
            this.chkBoxShowNeedNoUpdate.CheckedChanged += new System.EventHandler(this.chkBoxShowNeedNoUpdate_CheckedChanged);
            // 
            // chkBoxShowSkippedOrders
            // 
            this.chkBoxShowSkippedOrders.AutoSize = true;
            this.chkBoxShowSkippedOrders.Enabled = false;
            this.chkBoxShowSkippedOrders.Location = new System.Drawing.Point(11, 161);
            this.chkBoxShowSkippedOrders.Name = "chkBoxShowSkippedOrders";
            this.chkBoxShowSkippedOrders.Size = new System.Drawing.Size(220, 21);
            this.chkBoxShowSkippedOrders.TabIndex = 15;
            this.chkBoxShowSkippedOrders.Text = "Show orders marked as \"Skip\"";
            this.chkBoxShowSkippedOrders.UseVisualStyleBackColor = true;
            this.chkBoxShowSkippedOrders.CheckedChanged += new System.EventHandler(this.chkBoxShowSkippedOrders_CheckedChanged);
            // 
            // btnSkipAll
            // 
            this.btnSkipAll.Enabled = false;
            this.btnSkipAll.Location = new System.Drawing.Point(12, 482);
            this.btnSkipAll.Name = "btnSkipAll";
            this.btnSkipAll.Size = new System.Drawing.Size(215, 112);
            this.btnSkipAll.TabIndex = 16;
            this.btnSkipAll.Text = "Skip all";
            this.btnSkipAll.UseVisualStyleBackColor = true;
            this.btnSkipAll.Click += new System.EventHandler(this.btnSkipAll_Click);
            // 
            // txtBoxOrderTextFilter
            // 
            this.txtBoxOrderTextFilter.Enabled = false;
            this.txtBoxOrderTextFilter.Location = new System.Drawing.Point(11, 40);
            this.txtBoxOrderTextFilter.Name = "txtBoxOrderTextFilter";
            this.txtBoxOrderTextFilter.Size = new System.Drawing.Size(199, 22);
            this.txtBoxOrderTextFilter.TabIndex = 17;
            this.txtBoxOrderTextFilter.TextChanged += new System.EventHandler(this.txtBoxOrderTextFilter_TextChanged);
            // 
            // grpBoxFilters
            // 
            this.grpBoxFilters.Controls.Add(this.txtBoxOrderTextFilter);
            this.grpBoxFilters.Controls.Add(this.chkBoxShowOutOfReach);
            this.grpBoxFilters.Controls.Add(this.chkBoxShowCooldown);
            this.grpBoxFilters.Controls.Add(this.chkBoxShowSkippedOrders);
            this.grpBoxFilters.Controls.Add(this.chkBoxShowNeedNoUpdate);
            this.grpBoxFilters.Location = new System.Drawing.Point(12, 121);
            this.grpBoxFilters.Name = "grpBoxFilters";
            this.grpBoxFilters.Size = new System.Drawing.Size(231, 203);
            this.grpBoxFilters.TabIndex = 18;
            this.grpBoxFilters.TabStop = false;
            this.grpBoxFilters.Text = "OrderFilters";
            // 
            // btnToggleLog
            // 
            this.btnToggleLog.Location = new System.Drawing.Point(282, 12);
            this.btnToggleLog.Name = "btnToggleLog";
            this.btnToggleLog.Size = new System.Drawing.Size(110, 89);
            this.btnToggleLog.TabIndex = 19;
            this.btnToggleLog.Text = "Toggle log window";
            this.btnToggleLog.UseVisualStyleBackColor = true;
            this.btnToggleLog.Click += new System.EventHandler(this.btnToggleLog_Click);
            // 
            // numUpDownModifyCooldown
            // 
            this.numUpDownModifyCooldown.Enabled = false;
            this.numUpDownModifyCooldown.Location = new System.Drawing.Point(921, 29);
            this.numUpDownModifyCooldown.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numUpDownModifyCooldown.Minimum = new decimal(new int[] {
            390,
            0,
            0,
            0});
            this.numUpDownModifyCooldown.Name = "numUpDownModifyCooldown";
            this.numUpDownModifyCooldown.Size = new System.Drawing.Size(50, 22);
            this.numUpDownModifyCooldown.TabIndex = 21;
            this.numUpDownModifyCooldown.Value = new decimal(new int[] {
            390,
            0,
            0,
            0});
            this.numUpDownModifyCooldown.ValueChanged += new System.EventHandler(this.numUpDownModifyCooldown_ValueChanged);
            // 
            // lblModifyCooldown
            // 
            this.lblModifyCooldown.AutoSize = true;
            this.lblModifyCooldown.Location = new System.Drawing.Point(732, 31);
            this.lblModifyCooldown.Name = "lblModifyCooldown";
            this.lblModifyCooldown.Size = new System.Drawing.Size(150, 17);
            this.lblModifyCooldown.TabIndex = 22;
            this.lblModifyCooldown.Text = "Modify Cooldown (sec)";
            // 
            // lblReloadOrderListInterval
            // 
            this.lblReloadOrderListInterval.AutoSize = true;
            this.lblReloadOrderListInterval.Location = new System.Drawing.Point(420, 67);
            this.lblReloadOrderListInterval.Name = "lblReloadOrderListInterval";
            this.lblReloadOrderListInterval.Size = new System.Drawing.Size(212, 17);
            this.lblReloadOrderListInterval.TabIndex = 23;
            this.lblReloadOrderListInterval.Text = "Turns before reloading order list";
            // 
            // lblProcessInterval
            // 
            this.lblProcessInterval.AutoSize = true;
            this.lblProcessInterval.Location = new System.Drawing.Point(420, 29);
            this.lblProcessInterval.Name = "lblProcessInterval";
            this.lblProcessInterval.Size = new System.Drawing.Size(224, 17);
            this.lblProcessInterval.TabIndex = 24;
            this.lblProcessInterval.Text = "Interval of processing orders (sec)";
            // 
            // numUpDownReloadInterval
            // 
            this.numUpDownReloadInterval.Enabled = false;
            this.numUpDownReloadInterval.Location = new System.Drawing.Point(650, 67);
            this.numUpDownReloadInterval.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numUpDownReloadInterval.Name = "numUpDownReloadInterval";
            this.numUpDownReloadInterval.Size = new System.Drawing.Size(50, 22);
            this.numUpDownReloadInterval.TabIndex = 25;
            this.numUpDownReloadInterval.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numUpDownReloadInterval.ValueChanged += new System.EventHandler(this.numUpDownReloadInterval_ValueChanged);
            // 
            // numUpDownProcessInterval
            // 
            this.numUpDownProcessInterval.Enabled = false;
            this.numUpDownProcessInterval.Location = new System.Drawing.Point(650, 29);
            this.numUpDownProcessInterval.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numUpDownProcessInterval.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numUpDownProcessInterval.Name = "numUpDownProcessInterval";
            this.numUpDownProcessInterval.Size = new System.Drawing.Size(50, 22);
            this.numUpDownProcessInterval.TabIndex = 26;
            this.numUpDownProcessInterval.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numUpDownProcessInterval.ValueChanged += new System.EventHandler(this.numUpDownProcessInterval_ValueChanged);
            // 
            // numUpDownFluctuationCap
            // 
            this.numUpDownFluctuationCap.Enabled = false;
            this.numUpDownFluctuationCap.Location = new System.Drawing.Point(921, 69);
            this.numUpDownFluctuationCap.Name = "numUpDownFluctuationCap";
            this.numUpDownFluctuationCap.Size = new System.Drawing.Size(50, 22);
            this.numUpDownFluctuationCap.TabIndex = 27;
            // 
            // numUpDown1
            // 
            this.numUpDown1.Enabled = false;
            this.numUpDown1.Location = new System.Drawing.Point(638, 95);
            this.numUpDown1.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numUpDown1.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numUpDown1.Name = "numUpDown1";
            this.numUpDown1.Size = new System.Drawing.Size(68, 22);
            this.numUpDown1.TabIndex = 28;
            this.numUpDown1.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // lblGapBiggerThan
            // 
            this.lblGapBiggerThan.AutoSize = true;
            this.lblGapBiggerThan.Enabled = false;
            this.lblGapBiggerThan.Location = new System.Drawing.Point(416, 95);
            this.lblGapBiggerThan.Name = "lblGapBiggerThan";
            this.lblGapBiggerThan.Size = new System.Drawing.Size(216, 17);
            this.lblGapBiggerThan.TabIndex = 30;
            this.lblGapBiggerThan.Text = "Price gap should bigger than (%)";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 727);
            this.Controls.Add(this.lblGapBiggerThan);
            this.Controls.Add(this.numUpDown1);
            this.Controls.Add(this.numUpDownFluctuationCap);
            this.Controls.Add(this.numUpDownProcessInterval);
            this.Controls.Add(this.numUpDownReloadInterval);
            this.Controls.Add(this.lblProcessInterval);
            this.Controls.Add(this.lblReloadOrderListInterval);
            this.Controls.Add(this.lblModifyCooldown);
            this.Controls.Add(this.numUpDownModifyCooldown);
            this.Controls.Add(this.btnToggleLog);
            this.Controls.Add(this.grpBoxFilters);
            this.Controls.Add(this.btnSkipAll);
            this.Controls.Add(this.btnSkipNone);
            this.Controls.Add(this.lblFluctuationCap);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.ButtonLoadOrderList);
            this.Controls.Add(this.OrderList);
            this.MinimumSize = new System.Drawing.Size(1024, 750);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Resize += new System.EventHandler(this.MainWindow_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.OrderList)).EndInit();
            this.grpBoxFilters.ResumeLayout(false);
            this.grpBoxFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownModifyCooldown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownReloadInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownProcessInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownFluctuationCap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonLoadOrderList;
        private System.Windows.Forms.Button button2;
        private BrightIdeasSoftware.ObjectListView OrderList;
        private System.Windows.Forms.Label lblFluctuationCap;
        private BrightIdeasSoftware.OLVColumn Skip;
        private BrightIdeasSoftware.OLVColumn ID;
        private BrightIdeasSoftware.OLVColumn Status;
        private BrightIdeasSoftware.OLVColumn Type;
        private BrightIdeasSoftware.OLVColumn Quantity;
        private BrightIdeasSoftware.OLVColumn Price;
        private BrightIdeasSoftware.OLVColumn Station;
        private BrightIdeasSoftware.OLVColumn Region;
        private System.Windows.Forms.Button btnSkipNone;
        private OLVColumn Cooldown;
        private System.Windows.Forms.CheckBox chkBoxShowOutOfReach;
        private System.Windows.Forms.CheckBox chkBoxShowCooldown;
        private System.Windows.Forms.CheckBox chkBoxShowNeedNoUpdate;
        private System.Windows.Forms.CheckBox chkBoxShowSkippedOrders;
        private System.Windows.Forms.Button btnSkipAll;
        private System.Windows.Forms.TextBox txtBoxOrderTextFilter;
        private System.Windows.Forms.GroupBox grpBoxFilters;
        private System.Windows.Forms.Button btnToggleLog;
        private System.Windows.Forms.NumericUpDown numUpDownModifyCooldown;
        private System.Windows.Forms.Label lblModifyCooldown;
        private System.Windows.Forms.Label lblReloadOrderListInterval;
        private System.Windows.Forms.Label lblProcessInterval;
        private System.Windows.Forms.NumericUpDown numUpDownReloadInterval;
        private System.Windows.Forms.NumericUpDown numUpDownProcessInterval;
        private System.Windows.Forms.NumericUpDown numUpDownFluctuationCap;
        private System.Windows.Forms.NumericUpDown numUpDown1;
        private System.Windows.Forms.Label lblGapBiggerThan;
    }

    ///// <summary>
    ///// Use double buffer to avoid flashing.
    ///// TODO delete this if it proves not necessary
    ///// </summary>
    //class DoubleBufferListView : BrightIdeasSoftware.ObjectListView
    //{
    //    public DoubleBufferListView()
    //    {
    //        SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer | System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
    //        UpdateStyles();
    //    }
    //}
}

