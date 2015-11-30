using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;
using System.Windows.Forms;
using System.Diagnostics;
using InnerSpaceAPI;

namespace Eve_TradingAgent
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            _addDelegates();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            _logWindow = new LogWindow();
            _agent = new TradeAgent(this);
            _config = _agent.Config;
            _logWindow.SetConfig(_config);
            _character = _agent.ActiveCharacter;

            TimerOrderListRedraw.Elapsed += new System.Timers.ElapsedEventHandler(RedrawIntervalElapsedHandler);
            TimerOrderListRedraw.Start();
        }

        public void DrawOrderList()
        {
            TimerOrderListRedraw.Stop();

            if (!_agent.IsOrderListOccupied)
            {
                _agent.IsOrderListOccupied = true;

                if (_firstDraw)
                {
                    try
                    {
                        OrderList.SetObjects(_agent.OrderSet.Orders);
                        _firstDraw = false;
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("Error while trying to draw order list:");
                        Log.WriteLog(e);
                    }
                }
                else
                {
                    try
                    {
                        OrderList.BuildList(true);
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog("Error while trying to redraw order list:");
                        Log.WriteLog(e);
                    }
                }

                _agent.IsOrderListOccupied = false;
            }

            TimerOrderListRedraw.Start();
        }

        /// <summary>
        /// Output event message to log window.
        /// </summary>
        /// <param name="evt">event</param>
        public void WriteLogToWindow(object evt)
        {
            Debug.Assert((evt is string) || (evt is Exception));

            //List<String> 
            string NewMessage = System.DateTime.Now.ToString() + "  |  ";
            if (evt is string)
            {
                NewMessage += (evt as string);
            }
            else if (evt is Exception)
            {
                NewMessage += "Exception: " + (evt as Exception).Message;

                if ((evt as Exception).InnerException != null)
                {
                    NewMessage += "InnerException: " + (evt as Exception).InnerException.Message;
                }
            }

            if (_consoleMessages.Count > Config.MaxiumLogNumberToDisplay)
            {
                _consoleMessages.RemoveRange(0, _consoleMessages.Count - Config.MaxiumLogNumberToDisplay);
            }
            _consoleMessages.Add(NewMessage);

            _logWindow.DataSource = null;
            _logWindow.DataSource = _consoleMessages;

            // TODO maybe keep user selection when it exists.

            // If we try to access the selected index before ever showing it,
            // any value rather than -1 will throw out of range.
            if (_logWindow.HasEverBeenShown)
            {
                _logWindow.SelectedIndex = _logWindow.ItemCount - 1;
            }
        }

        private TradeAgent _agent;
        private Config _config;
        private Character _character;
        private List<String> _consoleMessages = new List<string>();
        private System.Timers.Timer TimerOrderListRedraw = new System.Timers.Timer(1000);
        private bool _firstDraw = true;
        private LogWindow _logWindow;
        private bool _isExecuting = false;
        private bool _hasLoaded = false;

        private void button1_Click(object sender, EventArgs e)
        {
            _agent.UpdateInfo();
            TimerOrderListRedraw.Start();
            _hasLoaded = true;
        }

        public void EnableControls()
        {
            txtBoxOrderTextFilter.Enabled = true;

            chkBoxShowCooldown.Enabled = true;
            chkBoxShowSkippedOrders.Enabled = true;
            chkBoxShowNeedNoUpdate.Enabled = true;
            chkBoxShowOutOfReach.Enabled = true;

            btnSkipAll.Enabled = true;
            btnSkipNone.Enabled = true;

            numUpDown1.Enabled = true;
            numUpDownModifyCooldown.Enabled = true;
            numUpDownProcessInterval.Enabled = true;
            numUpDownReloadInterval.Enabled = true;
        }

        public void DisableControls()
        {
            txtBoxOrderTextFilter.Enabled = false;

            chkBoxShowCooldown.Enabled = false;
            chkBoxShowSkippedOrders.Enabled = false;
            chkBoxShowNeedNoUpdate.Enabled = false;
            chkBoxShowOutOfReach.Enabled = false;

            btnSkipAll.Enabled = false;
            btnSkipNone.Enabled = false;

            numUpDown1.Enabled = false;
            numUpDownModifyCooldown.Enabled = false;
            numUpDownProcessInterval.Enabled = false;
            numUpDownReloadInterval.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!_hasLoaded)
            {
                button1_Click(sender, e);
            }

            if (!_isExecuting)
            {
                // _agent.TimerReloadOrderList.Start();
                _agent.TimerProcessOrder.Start();
                _agent.ShouldStopTimer = false;
                button2.Text = "Pause Processing";
                _isExecuting = true;
            }
            else
            {
                // _agent.TimerReloadOrderList.Stop();
                _agent.TimerProcessOrder.Stop();
                _agent.ShouldStopTimer = true;
                button2.Text = "Start Processing";
                _isExecuting = false;
            }
        }

        void RedrawIntervalElapsedHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            DrawOrderList();
        }

        /// <summary>
        /// Other initialize operations not allowed in InitializeComponent()
        /// </summary
        private void _addDelegates()
        {
            // force group by sell/buy
            OLVColumn ColumnOrderType = new OLVColumn("invisible", "OrderType");
            this.OrderList.AlwaysGroupByColumn = ColumnOrderType;
            this.OrderList.AlwaysGroupBySortOrder = SortOrder.Ascending;
            this.OrderList.SortGroupItemsByPrimaryColumn = false;
            this.OrderList.ModelFilter = new ModelFilter(delegate(object order)
            {
                bool shouldShow = true;

                // TODO update out of range logic when necessary

                if (_config != null)
                {
                    shouldShow = shouldShow && ((((MyMarketOrder)order).StationID == _character.CurrentStationID) || _config.ShowOrdersOutOfReach);

                    shouldShow = shouldShow && (((MyMarketOrder)order).IsModifyCoolDownFinished || _config.ShowOrdersCoolingDown);
                    shouldShow = shouldShow && (((MyMarketOrder)order).Status == OrderStatus.NeedNoUpdate || _config.ShowOrdersNeedNoUpdate);
                    shouldShow = shouldShow && (!((MyMarketOrder)order).Skip || _config.ShowOrdersMarkedSkip);

                    shouldShow = shouldShow && (string.IsNullOrEmpty(txtBoxOrderTextFilter.Text) ||
                                                ((MyMarketOrder)order).Type.ToLowerInvariant().Contains(txtBoxOrderTextFilter.Text.ToLowerInvariant()));
                }

                return shouldShow;
            });

            // commented out for it seems not working.
            // TextMatchFilter filter = TextMatchFilter.Contains(this.OrderList, txtBoxOrderTextFilter.Text);
            // this.OrderList.DefaultRenderer = new HighlightTextRenderer(filter);

            //TextMatchFilter.Contains(this.OrderList
            //this.OrderList.ShowSortIndicators = true;
            // this.OrderList.TintSortColumn = true;

            // use custom sorting for column 'do not touch' and 'quantity'
            this.OrderList.BeforeSorting += new EventHandler<BeforeSortingEventArgs>(_customSorting);

            this.OrderList.FormatRow += new EventHandler<FormatRowEventArgs>(_customRowFormat);

            // setup "list is empty" overlay
            TextOverlay textOverlay = this.OrderList.EmptyListMsgOverlay as TextOverlay;
            textOverlay.TextColor = Color.Gray;
            textOverlay.BackColor = Color.Transparent;
            textOverlay.BorderColor = Color.Transparent;

            // setup "Skip" checkbox

            // setup "Skip" column

            // setup "ID" column

            // setup "Status" column
            this.Status.AspectGetter = _formatStateText;

            // setup "Cooldown" column
            this.Cooldown.AspectGetter = _formatCoolDownText;

            // setup "type" column

            // setup "Quantity" column
            this.Quantity.AspectGetter = _formatQuantityText;

            // setup "Price" column

            // setup "Price" column

            // setup "Station" column

            // setup "Region" column

        }

        #region delegates for orderlist

        private class _columnQuantityComparer : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                MyMarketOrder oa = (MyMarketOrder)a;
                MyMarketOrder ob = (MyMarketOrder)b;

                if (oa.QuantityRemaining > ob.QuantityRemaining)
                {
                    return 1;
                }
                if (oa.QuantityRemaining < ob.QuantityRemaining)
                {
                    return -1;
                }
                else
                    return 0;
            }
        }

        private AspectGetterDelegate _formatStateText = delegate(object order)
            {
                string StatusText = "";

                switch (((MyMarketOrder)order).Status)
                {
                    case OrderStatus.Unknown:
                        StatusText = "Unknown";
                        break;
                    case OrderStatus.OutOfReach:
                        StatusText = "Out of reach";
                        break;
                    case OrderStatus.NeedNoUpdate:
                        StatusText = "Price need no update";
                        break;
                    case OrderStatus.Hesitate:
                        StatusText = "Can't decide new price";
                        break;
                    case OrderStatus.AutoModified:
                        StatusText = "Price auto modified";
                        break;
                    case OrderStatus.ManuallySpecified:
                        StatusText = "Price manually modified";
                        break;
                    default:
                        break;
                }
                return StatusText;
            };

        private AspectGetterDelegate _formatCoolDownText = delegate(object order)
        {
            string CooldownTimeText = "";

            if (!((MyMarketOrder)order).IsModifyCoolDownFinished)
            {
                CooldownTimeText = (((MyMarketOrder)order).ModifyCoolDownEndTime - System.DateTime.UtcNow).ToString((@"mm\:ss"));
            }

            return CooldownTimeText;
        };

        private AspectGetterDelegate _formatQuantityText = delegate(object order)
        {
            return ((MyMarketOrder)order).QuantityRemaining + " \\ " + ((MyMarketOrder)order).InitialQuantity;
        };

        private void _customSorting(object sender, BeforeSortingEventArgs e)
        {
            if (e.ColumnToSort == this.Quantity)
            {
                this.OrderList.Sort(new OLVColumn("invisible", "QuantityRemaining"));
                e.Handled = true;
            }
            else if (e.ColumnToSort == this.Skip)
            {
                this.OrderList.Sort(new OLVColumn("invisible", "Skip"));
                e.Handled = true;
            }
        }
        private void _customRowFormat(object sender, FormatRowEventArgs e)
        {
            MyMarketOrder o = (MyMarketOrder)e.Model;

            //switch (e.DisplayIndex % 5)
            //{
            //    case 0:
            //        e.Item.BackColor = Color.DarkGray;
            //        break;
            //    case 1:
            //        e.Item.BackColor = Color.PaleGreen;
            //        break;
            //    case 2:
            //        e.Item.BackColor = Color.SandyBrown;
            //        break;
            //    case 3:
            //        e.Item.BackColor = Color.LightSkyBlue;
            //        break;
            //    case 4:
            //        e.Item.BackColor = Color.Plum;
            //        break;
            //    default:
            //        break;
            //}


            if (o.Status == OrderStatus.OutOfReach)
            {

                e.Item.ForeColor = Color.LightGray;
                e.Item.BackColor = Color.DarkGray;
            }
            else
            {
                if (!o.IsModifyCoolDownFinished)
                {
                    e.Item.ForeColor = Color.Gray;
                }
                switch (o.Status)
                {
                    case OrderStatus.NeedNoUpdate:
                        e.Item.BackColor = Color.PaleGreen;
                        break;
                    case OrderStatus.Hesitate:
                        e.Item.BackColor = Color.SandyBrown;
                        break;
                    case OrderStatus.AutoModified:
                        e.Item.BackColor = Color.LightSkyBlue;
                        break;
                    case OrderStatus.ManuallySpecified:
                        e.Item.BackColor = Color.Plum;
                        break;
                    case OrderStatus.Unknown:
                    default:
                        break;
                }
            }
        }

        #endregion

        public void RestoreCharacterConfig()
        {
            //TODO add load config method
            if (_config != null)
            {
                if (_config.ListViewState != null)
                {
                    OrderList.RestoreState(_config.ListViewState);
                }

                chkBoxShowCooldown.Checked = _config.ShowOrdersCoolingDown;
                chkBoxShowSkippedOrders.Checked = _config.ShowOrdersMarkedSkip;
                chkBoxShowNeedNoUpdate.Checked = _config.ShowOrdersNeedNoUpdate;
                chkBoxShowOutOfReach.Checked = _config.ShowOrdersOutOfReach;

                // Do not close log window if it is originally shown
                _logWindow.Visible = _config.ShowLogWindow = (_config.ShowLogWindow || _logWindow.Visible);

                numUpDownModifyCooldown.Value = _config._orderModifyIntervalInMilliSec / 1000;
                numUpDownProcessInterval.Value = _config._orderProcessIntervalInMilliSec / 1000;
                numUpDownReloadInterval.Value = _config._turnsBetweenReloadOrderList;

                numUpDown1.Value = _config.ValidGapSizeRatio;
            }
        }

        #region click event handling
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO add save config 
            // if the character is not loaded yet, this value will not be saved
            if (_config != null)
            {
                _config.ListViewState = OrderList.SaveState();
            }
        }

        private void chkBoxShowOutOfReach_CheckedChanged(object sender, EventArgs e)
        {
            _config.ShowOrdersOutOfReach = chkBoxShowOutOfReach.Checked;

            DrawOrderList();
        }

        private void chkBoxShowCooldown_CheckedChanged(object sender, EventArgs e)
        {
            _config.ShowOrdersCoolingDown = chkBoxShowCooldown.Checked;

            DrawOrderList();
        }

        private void chkBoxShowNeedNoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            _config.ShowOrdersNeedNoUpdate = chkBoxShowNeedNoUpdate.Checked;

            DrawOrderList();
        }

        private void chkBoxShowSkippedOrders_CheckedChanged(object sender, EventArgs e)
        {
            _config.ShowOrdersMarkedSkip = chkBoxShowSkippedOrders.Checked;

            DrawOrderList();
        }

        private void OrderList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // To improve the visual response speed when shorting by DoNotTouch cloumn.
            DrawOrderList();
        }

        private void txtBoxOrderTextFilter_TextChanged(object sender, EventArgs e)
        {
            DrawOrderList();
        }

        private void btnSkipNone_Click(object sender, EventArgs e)
        {
            this.OrderList.CheckedObjects = null;
        }

        private void btnSkipAll_Click(object sender, EventArgs e)
        {
            this.OrderList.CheckedObjectsEnumerable = this.OrderList.Objects;
        }

        private void btnToggleLog_Click(object sender, EventArgs e)
        {
            _logWindow.Visible = !_logWindow.Visible;
            if (_config != null)
            {
                _config.ShowLogWindow = _logWindow.Visible;
            }
        }


        #region right click menu of order list

        private void OrderList_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            // or e.column to get column info
            e.MenuStrip = this.DecideRightClickMenu(e.Model);
        }

        private ContextMenuStrip DecideRightClickMenu(object model)
        {
            if (model != null)
            {
                ContextMenuStrip myContextMenu = new ContextMenuStrip();

                ToolStripMenuItem menuItemTryProcessOrder = new ToolStripMenuItem("TryProcess");
                ToolStripMenuItem menuItemCancelOrder = new ToolStripMenuItem("Cancel");
                ToolStripSeparator menuSeparator = new ToolStripSeparator();
                // show when evaluate cooldown finsih
                ToolStripMenuItem menuItemTryEvaluate = new ToolStripMenuItem("Try Evaluate");
                // todo show try modify when cooldown finish


                // Clear all previously added MenuItems.
                myContextMenu.Items.Clear();

                myContextMenu.Items.Add(menuItemTryProcessOrder);
                myContextMenu.Items.Add(menuItemCancelOrder);
                myContextMenu.Items.Add(menuSeparator);
                myContextMenu.Items.Add(menuItemTryEvaluate);

                menuItemTryEvaluate.Enabled = false;
                if (!((MyMarketOrder)model).IsModifyCoolDownFinished)
                {
                    menuItemTryProcessOrder.Enabled = false;
                    menuItemCancelOrder.Enabled = false;
                }
                if (!((MyMarketOrder)model).IsEvaluateCoolDownFinished)
                {
                    menuItemTryProcessOrder.Enabled = false;
                }

                menuItemTryProcessOrder.Click += delegate(object obj, EventArgs e)
                {
                    // Order may be updated between show menu and click
                    // TODO check order range
                    if (((MyMarketOrder)model).Status != OrderStatus.OutOfReach)
                    {
                        if (((MyMarketOrder)model).IsModifyCoolDownFinished && ((MyMarketOrder)model).IsEvaluateCoolDownFinished)
                        {
                            if (MessageBox.Show("Are you sure you want to try process order \"" + ((MyMarketOrder)model).Type + "\" ?", "Confirm process", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                _agent.EvaluateAndUpdateOrder((MyMarketOrder)model);
                            }
                        }
                    }
                };

                menuItemCancelOrder.Click += delegate(object obj, EventArgs e)
                {
                    // Order may be updated between show menu and click
                    if (((MyMarketOrder)model).IsModifyCoolDownFinished)
                    {
                        if (MessageBox.Show("Are you sure you want to cancel order \"" + ((MyMarketOrder)model).Type + "\" ?", "Confirm cancel", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ((MyMarketOrder)model).Cancel();
                        }
                    }
                };

                return myContextMenu;
            }
            else
            {
                return null;
            }
        }

        #endregion

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            OrderList.Size = new System.Drawing.Size(this.Width - OrderList.Left - 25, this.Height - OrderList.Top - 55);
        }

        #endregion

        private void numUpDownProcessInterval_ValueChanged(object sender, EventArgs e)
        {
            _config._orderProcessIntervalInMilliSec = (int)numUpDownProcessInterval.Value * 1000;
        }

        private void numUpDownReloadInterval_ValueChanged(object sender, EventArgs e)
        {
            _config._turnsBetweenReloadOrderList = (int)numUpDownReloadInterval.Value;
        }

        private void numUpDownModifyCooldown_ValueChanged(object sender, EventArgs e)
        {
            _config._orderModifyIntervalInMilliSec = (int)numUpDownModifyCooldown.Value * 1000;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _config.ValidGapSizeRatio = (int)numUpDown1.Value;
        }
    }
}
