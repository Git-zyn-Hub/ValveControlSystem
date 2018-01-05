using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ValveControlSystem.UserControls
{
    /// <summary>
    /// Interaction logic for ValveStateUserControl.xaml
    /// </summary>
    public partial class ValveStateUserControl : UserControl, INotifyPropertyChanged
    {
        private bool? _state = null;
        public ValveStateUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        /// <summary>
        /// 开关状态，null为未知，对应灯变灰，字消失；true为开，绿灯亮；false为关，红灯亮。
        /// </summary>
        public bool? State
        {
            get
            {
                return _state;
            }

            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged("State");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    public class Bool2OpenColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? state = (bool?)value;
            switch (state)
            {
                case true:
                    return Colors.LightGreen;
                case null:
                case false:
                default:
                    return Colors.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Bool2CloseColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? state = (bool?)value;
            switch (state)
            {
                case false:
                    return Colors.Red;
                case null:
                case true:
                default:
                    return Colors.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Bool2OpenWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? state = (bool?)value;
            switch (state)
            {
                case true:
                    return "开";
                case null:
                case false:
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Bool2CloseWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? state = (bool?)value;
            switch (state)
            {
                case false:
                    return "关";
                case null:
                case true:
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
