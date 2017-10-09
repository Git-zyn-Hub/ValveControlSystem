using System;
using System.Collections.Generic;
using System.IO;
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
using ValveControlSystem.Classes;

namespace ValveControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for SurfacePresetWindow.xaml
    /// </summary>
    public partial class SurfacePresetWindow : Window
    {
        private SurfacePreset _surfacePrs = new SurfacePreset();
        private SurfacePresetXmlHelper _presetXmlHelper = new SurfacePresetXmlHelper();
        private ToolNo _toolNoSet = ToolNo.Undefined;

        public SurfacePreset SurfacePrs
        {
            get
            {
                return _surfacePrs;
            }

            set
            {
                _surfacePrs = value;
            }
        }

        public Button ButtonOK
        {
            get { return this.btnOK; }
        }

        public ToolNo ToolNoSet
        {
            get
            {
                return _toolNoSet;
            }

            set
            {
                _toolNoSet = value;
            }
        }

        public SurfacePresetWindow()
        {
            InitializeComponent();
            _presetXmlHelper.SurfacePresetXmlInitial();
            SurfacePrs = _presetXmlHelper.GetSurfacePreset();
            this.DataContext = SurfacePrs;
            ButtonOK.Click += ButtonOK_Click;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            checkErrorState();

            if (SurfacePrs.ToolNumber == 1)
            {
                ToolNoSet = ToolNo.Tool_1;
            }
            else if (SurfacePrs.ToolNumber == 2)
            {
                ToolNoSet = ToolNo.Tool_2;
            }
            else
            {
                ToolNoSet = ToolNo.Undefined;
            }

            _presetXmlHelper.ModifyXmlSurfacePresetElement(SurfacePrs);
            this.DialogResult = true;
            this.Close();
        }

        private bool checkErrorState()
        {
            FindChild fc = new FindChild();
            Border border = fc.FindVisualChild<Border>(this.txtAutomaticClosurePressure);
            return true;
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(_presetXmlHelper.XmlPath);
            _presetXmlHelper.SurfacePresetXmlInitial();
            SurfacePrs = _presetXmlHelper.GetSurfacePreset();
            this.DataContext = SurfacePrs;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
