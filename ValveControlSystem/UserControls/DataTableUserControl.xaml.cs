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
                if (dataArray.Length == 237)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int[] monitorDataArray = new int[7];

                        monitorDataArray[0] = (dataArray[53 + i * 56] << 8) + dataArray[54 + i * 56];
                        monitorDataArray[1] = (dataArray[55 + i * 56] << 8) + dataArray[56 + i * 56];
                        monitorDataArray[2] = (dataArray[57 + i * 56] << 8) + dataArray[58 + i * 56];
                        monitorDataArray[3] = (dataArray[59 + i * 56] << 8) + dataArray[60 + i * 56];
                        monitorDataArray[4] = (dataArray[61 + i * 56] << 8) + dataArray[62 + i * 56];
                        monitorDataArray[5] = (dataArray[63 + i * 56] << 8) + dataArray[64 + i * 56];
                        monitorDataArray[6] = (dataArray[65 + i * 56] << 8) + dataArray[66 + i * 56];

                        AddData(monitorDataArray);
                    }

                    //TableData tableDataAverage = new TableData();
                    //TableData tableDataSum = new TableData();
                    //foreach (var tableData in this.TableDatas)
                    //{
                    //    tableDataSum.SolenoidValveVoltage += tableData.SolenoidValveVoltage;
                    //    tableDataSum.PositivePowerMonitor += tableData.PositivePowerMonitor;
                    //    tableDataSum.NegativePowerMonitor += tableData.NegativePowerMonitor;
                    //    tableDataSum.Tool1TestValveDriveCurrent += tableData.Tool1TestValveDriveCurrent;
                    //    tableDataSum.Tool1CycleValveDriveCurrent += tableData.Tool1CycleValveDriveCurrent;
                    //    tableDataSum.Tool2TestValveDriveCurrent += tableData.Tool2TestValveDriveCurrent;
                    //    tableDataSum.Tool2CycleValveDriveCurrent += tableData.Tool2CycleValveDriveCurrent;
                    //}
                    //tableDataAverage.SolenoidValveVoltage = save2FractionalPart(tableDataSum.SolenoidValveVoltage / 8);
                    //tableDataAverage.PositivePowerMonitor = save2FractionalPart(tableDataSum.PositivePowerMonitor / 8);
                    //tableDataAverage.NegativePowerMonitor = save2FractionalPart(tableDataSum.NegativePowerMonitor / 8);
                    //tableDataAverage.Tool1TestValveDriveCurrent = tableDataSum.Tool1TestValveDriveCurrent / 8;
                    //tableDataAverage.Tool1CycleValveDriveCurrent = tableDataSum.Tool1CycleValveDriveCurrent / 8;
                    //tableDataAverage.Tool2TestValveDriveCurrent = tableDataSum.Tool2TestValveDriveCurrent / 8;
                    //tableDataAverage.Tool2CycleValveDriveCurrent = tableDataSum.Tool2CycleValveDriveCurrent / 8;

                    //this.TableDatas.Add(tableDataAverage);
                    // ScrollControl();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        public void AddData(int[] monitorDataArray)
        {
            //需要对原始数据进行，数值转换，乘或除对应的系数。
            TableData tableData = new TableData();
            tableData.SolenoidValveVoltage = (double)monitorDataArray[2] * 2 / 1000;
            tableData.NegativePowerMonitor = (double)monitorDataArray[1] * 8.5 / 1000;
            tableData.PositivePowerMonitor = (double)monitorDataArray[0] * 6 / 1000;
            tableData.Tool2CycleValveDriveCurrent = monitorDataArray[3];
            tableData.Tool2TestValveDriveCurrent = monitorDataArray[4];
            tableData.Tool1CycleValveDriveCurrent = monitorDataArray[5];
            tableData.Tool1TestValveDriveCurrent = monitorDataArray[6];

            tableData.SolenoidValveVoltage = save2FractionalPart(tableData.SolenoidValveVoltage);
            tableData.NegativePowerMonitor = save2FractionalPart(tableData.NegativePowerMonitor);
            tableData.PositivePowerMonitor = save2FractionalPart(tableData.PositivePowerMonitor);

            this.TableDatas.Add(tableData);
            ScrollControl();
        }

        private double save2FractionalPart(double input)
        {
            return (double)((int)(input * 100)) / 100;
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
            e.Row.Header = e.Row.GetIndex() + 1;

            DataGridRow dgr = e.Row;
            //if (e.Row.GetIndex() == 8)
            //{
            //    e.Row.Header = "平均";
            //    dgr.Background = new SolidColorBrush(Colors.LightYellow);
            //}

            ContextMenu cm = new ContextMenu();

            MenuItem copyCellMenu = new MenuItem();
            copyCellMenu.Header = "复制单元格";
            copyCellMenu.Click += CopyCellOnClick;
            copyCellMenu.InputGestureText = "Ctrl+D";
            cm.Items.Add(copyCellMenu);

            MenuItem copyLines = new MenuItem();
            copyLines.Header = "复制一行或多行";
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
                    Clipboard.SetDataObject(sColumnValue);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("复制单元格异常：" + ee.Message);
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
        public double SolenoidValveVoltage { get; set; }
        public double PositivePowerMonitor { get; set; }
        public double NegativePowerMonitor { get; set; }
        public int Tool1TestValveDriveCurrent { get; set; }
        public int Tool1CycleValveDriveCurrent { get; set; }
        public int Tool2TestValveDriveCurrent { get; set; }
        public int Tool2CycleValveDriveCurrent { get; set; }
    }

    public class Int2WordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int iValue = (int)value;
            switch (iValue)
            {
                case 1:
                    return "激活";
                case 0:
                    return "关闭";
                default:
                    return "错误";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
