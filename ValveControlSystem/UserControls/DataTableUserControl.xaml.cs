using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ValveControlSystem.UserControls
{
    /// <summary>
    /// Interaction logic for DataTableUserControl.xaml
    /// </summary>
    public partial class DataTableUserControl : UserControl
    {
        private ObservableCollection<TableData> _tableDatas = new ObservableCollection<TableData>();
        private bool _stopScroll = false;

        public ObservableCollection<TableData> TableDatas
        {
            get
            {
                return _tableDatas;
            }

            set
            {
                _tableDatas = value;
            }
        }

        public DataTableUserControl()
        {
            InitializeComponent();
        }

        public void HandleData(byte[] dataArray)
        {
            try
            {
                for (int i = 0; i < 8; i++)
                {
                    int[] monitorDataArray = new int[7];

                    monitorDataArray[0] = (dataArray[11 + i * 32] << 8) + dataArray[12 + i * 32];
                    monitorDataArray[1] = (dataArray[15 + i * 32] << 8) + dataArray[16 + i * 32];
                    monitorDataArray[2] = (dataArray[19 + i * 32] << 8) + dataArray[20 + i * 32];
                    monitorDataArray[3] = (dataArray[23 + i * 32] << 8) + dataArray[24 + i * 32];
                    monitorDataArray[4] = (dataArray[27 + i * 32] << 8) + dataArray[28 + i * 32];
                    monitorDataArray[5] = (dataArray[31 + i * 32] << 8) + dataArray[32 + i * 32];
                    monitorDataArray[6] = (dataArray[35 + i * 32] << 8) + dataArray[36 + i * 32];

                    AddData(monitorDataArray);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        public void AddData(int[] monitorDataArray)
        {
            TableData tableData = new TableData();
            tableData.SolenoidValveVoltage = monitorDataArray[0];
            tableData.PositivePowerMonitor = monitorDataArray[1];
            tableData.NegativePowerMonitor = monitorDataArray[2];
            tableData.Tool1TestValveDriveCurrent = monitorDataArray[3];
            tableData.Tool1CycleValveDriveCurrent = monitorDataArray[4];
            tableData.Tool2TestValveDriveCurrent = monitorDataArray[5];
            tableData.Tool2CycleValveDriveCurrent = monitorDataArray[6];

            this.TableDatas.Add(tableData);
            ScrollControl();
        }

        private void ScrollControl()
        {
            if (!_stopScroll)
            {
                this.dgDataTable.ScrollIntoView(this.TableDatas[this.TableDatas.Count - 1]);
            }
        }

        private void dataTableUserCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgDataTable.Items.Count; i++)
            {
                DataGridRow row = getRow(this.dgDataTable, i);
                if (row != null)
                {
                    DataGridRowEventArgs eImport = new DataGridRowEventArgs(row);
                    dgDataTable_LoadingRow(sender, eImport);
                }
            }
        }

        public void ReloadUserControl()
        {
            this.RaiseEvent(new RoutedEventArgs(DataTableUserControl.LoadedEvent));
        }

        private void tbtnPin_Checked(object sender, RoutedEventArgs e)
        {
            _stopScroll = true;
        }

        private void tbtnPin_Unchecked(object sender, RoutedEventArgs e)
        {
            _stopScroll = false;
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ToggleButton b = sender as ToggleButton;
            if (b != null)
            {
                b.BorderThickness = new Thickness(0);
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ToggleButton b = sender as ToggleButton;
            if (b != null)
            {
                b.BorderThickness = new Thickness(1);
            }
        }

        private void dgDataTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dgr = e.Row;

            ContextMenu cm = new ContextMenu();

            MenuItem copyCellMenu = new MenuItem();
            copyCellMenu.Header = string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("copyCell").ToString());
            copyCellMenu.Click += CopyCellOnClick;
            copyCellMenu.InputGestureText = "Ctrl+D";
            cm.Items.Add(copyCellMenu);

            MenuItem copyLines = new MenuItem();
            copyLines.Header = string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("copyLines").ToString());
            copyLines.Click += CopyLinesOnClick;
            copyLines.InputGestureText = "Ctrl+C";
            cm.Items.Add(copyLines);
            ContextMenuService.SetContextMenu(dgr, cm);
        }
        private void CopyCellOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                if (this.dgDataTable.SelectedItem != null)
                {
                    int i = dgDataTable.CurrentColumn.DisplayIndex;
                    string sColumnValue = ((TextBlock)this.dgDataTable.Columns[i].GetCellContent(this.dgDataTable.SelectedItem)).Text.Trim();
                    Clipboard.SetText(sColumnValue);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void CopyLinesOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                forms.SendKeys.SendWait("^c");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取DataGrid的行
        /// </summary>
        /// <param name="dataGrid">DataGrid控件</param>
        /// <param name="rowIndex">DataGrid行号</param>
        /// <returns>指定的行号</returns>
        private DataGridRow getRow(DataGrid dataGrid, int rowIndex)
        {
            DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (rowContainer == null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            }
            return rowContainer;
        }

        private void CommandBinding_CopyCell_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.dgDataTable.SelectedItem != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void CommandBinding_CopyCell_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCellOnClick(sender, e);
        }

        public void ClearTable()
        {
            this.TableDatas.Clear();
        }
    }

    public class TableData
    {
        public int SolenoidValveVoltage { get; set; }
        public int PositivePowerMonitor { get; set; }
        public int NegativePowerMonitor { get; set; }
        public int Tool1TestValveDriveCurrent { get; set; }
        public int Tool1CycleValveDriveCurrent { get; set; }
        public int Tool2TestValveDriveCurrent { get; set; }
        public int Tool2CycleValveDriveCurrent { get; set; }
    }
}
