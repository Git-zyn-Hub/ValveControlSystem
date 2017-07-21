using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
    /// Interaction logic for SelectMeterNoWindow.xaml
    /// </summary>
    public partial class SelectToolNoWindow : Window
    {
        private int _toolNo = 0;
        public int ToolNo
        {
            get
            {
                return _toolNo;
            }

            set
            {
                _toolNo = value;
            }
        }

        public SelectToolNoWindow()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (this.ToolNo == 0)
            {
                MessageBox.Show("请选择一支工具");
            }
            else
            {
                this.DialogResult = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void rbTool1_Checked(object sender, RoutedEventArgs e)
        {
            this.ToolNo = 1;
        }

        private void rbTool2_Checked(object sender, RoutedEventArgs e)
        {
            this.ToolNo = 2;
        }
    }
}
