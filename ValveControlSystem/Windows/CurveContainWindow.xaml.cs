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

namespace ValveControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for CurveContainWindow.xaml
    /// </summary>
    public partial class CurveContainWindow : Window
    {
        private bool _isShow = false;
        //public delegate void ClosedEventHandler();
        //public event ClosedEventHandler Closed;

        public bool IsShow
        {
            get
            {
                return _isShow;
            }

            set
            {
                _isShow = value;
            }
        }

        public CurveContainWindow()
        {
            InitializeComponent();
        }

        public void AddContent(UserControl userCtrl)
        {
            this.gridMain.Children.Add(userCtrl);
        }

        public void ClearContent()
        {
            this.gridMain.Children.Clear();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.IsShow = false;
            ClearContent();
        }
    }
}
