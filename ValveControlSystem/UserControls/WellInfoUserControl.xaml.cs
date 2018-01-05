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
using ValveControlSystem.Classes;

namespace ValveControlSystem.UserControls
{
    /// <summary>
    /// Interaction logic for WellInfoUserControl.xaml
    /// </summary>
    public partial class WellInfoUserControl : UserControl, INotifyPropertyChanged
    {
        private WellInfoMode _wellInfoUserCtrlMode;
        private WellInfomation _wellInfo;

        public WellInfoMode WellInfoUserCtrlMode
        {
            get
            {
                return _wellInfoUserCtrlMode;
            }

            set
            {
                if (_wellInfoUserCtrlMode != value)
                {
                    _wellInfoUserCtrlMode = value;
                    OnPropertyChanged("WellInfoUserCtrlMode");
                }
            }
        }

        public WellInfomation WellInfo
        {
            get
            {
                return _wellInfo;
            }

            set
            {
                if (_wellInfo != value)
                {
                    _wellInfo = value;
                    OnPropertyChanged("WellInfo");
                }
            }
        }

        public WellInfoUserControl()
        {
            InitializeComponent();
            this.DataContext = this;
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

    public enum WellInfoMode
    {
        主页,
        地面预设
    }

    public class Mode2TextBlockVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            WellInfoMode mode = (WellInfoMode)value;
            switch (mode)
            {
                case WellInfoMode.主页:
                    return Visibility.Visible;
                case WellInfoMode.地面预设:
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Mode2TextBoxVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            WellInfoMode mode = (WellInfoMode)value;
            switch (mode)
            {
                case WellInfoMode.地面预设:
                    return Visibility.Visible;
                case WellInfoMode.主页:
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
