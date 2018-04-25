using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ValveControlSystem.Classes;

namespace ValveControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for SetPowerOnTimeWindow.xaml
    /// </summary>
    public partial class SetPowerOnTimeWindow : Window
    {
        private DateTime _powerOnTime = new DateTime();
        private DateTimeXmlHelper _timeXmlHelper = new DateTimeXmlHelper();
        public DateTime PowerOnTime
        {
            get
            {
                return _powerOnTime;
            }
            set
            {
                _powerOnTime = value;
            }
        }
        public SetPowerOnTimeWindow()
        {
            InitializeComponent();
            try
            {
                _timeXmlHelper.LogDateTimeXmlInitial();

                DateTime? powerOnTime = _timeXmlHelper.GetPowerOnDateAndTime();
                if (powerOnTime.HasValue)
                {
                    this.dpEndDate.SelectedDate = powerOnTime.Value;
                    this.tpEndTime.Value = powerOnTime.Value;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("" + ee.Message);
            }
        }


        private void btnToNow_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = System.DateTime.Now;
            this.dpEndDate.SelectedDate = now;
            this.tpEndTime.Value = now;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            int year;
            int month;
            int day;
            int hour;
            int minute;
            int second;
            if (this.dpEndDate.SelectedDate.HasValue)
            {
                year = this.dpEndDate.SelectedDate.Value.Year;
                month = this.dpEndDate.SelectedDate.Value.Month;
                day = this.dpEndDate.SelectedDate.Value.Day;
            }
            else
            {
                MessageBox.Show("'上电日期'必须选择一个值！");
                return;
            }
            if (this.tpEndTime.Value.HasValue)
            {
                hour = this.tpEndTime.Value.Value.Hour;
                minute = this.tpEndTime.Value.Value.Minute;
                second = this.tpEndTime.Value.Value.Second;
            }
            else
            {
                MessageBox.Show("'上电时间'必须选择一个值！");
                return;
            }

            this.PowerOnTime = new DateTime(year, month, day, hour, minute, second);

            _timeXmlHelper.ModifyPowerOnDateTimeXml(PowerOnTime, PowerOnTime);
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
