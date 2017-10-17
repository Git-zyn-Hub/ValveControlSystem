using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ValveControlSystem.Classes
{
    public class SurfacePreset : INotifyPropertyChanged
    {
        private int _automaticClosureValve;
        private int _automaticClosurePressure;
        private int _AVS_A_Option;
        private int _AVS_TriggerPressure;
        private int _AVS_B_Option;
        private int _AVS4TimeLimit;
        private int _AVS4UnderPressureLimit;
        private int _AVS4OverPressureLimit;
        private int _SUD_Setting;
        private int _ToolNumber;
        private int _circleValveState;
        private int _testValveState;
        public int AutomaticClosureValve
        {
            get
            {
                return _automaticClosureValve;
            }
            set
            {
                if (_automaticClosureValve != value)
                {
                    _automaticClosureValve = value;
                    OnPropertyChanged("AutomaticClosureValve");
                }
            }
        }

        public int AutomaticClosurePressure
        {
            get
            {
                return _automaticClosurePressure;
            }

            set
            {
                if (_automaticClosurePressure != value)
                {
                    _automaticClosurePressure = value;
                    OnPropertyChanged("AutomaticClosurePressure");
                }
            }
        }

        public int AVS_A_Option
        {
            get
            {
                return _AVS_A_Option;
            }

            set
            {
                if (_AVS_A_Option != value)
                {
                    _AVS_A_Option = value;
                    OnPropertyChanged("AVS_A_Option");
                }
            }
        }

        public int AVS_TriggerPressure
        {
            get
            {
                return _AVS_TriggerPressure;
            }

            set
            {
                if (_AVS_TriggerPressure != value)
                {
                    _AVS_TriggerPressure = value;
                    OnPropertyChanged("AVS_TriggerPressure");
                }
            }
        }

        public int AVS_B_Option
        {
            get
            {
                return _AVS_B_Option;
            }

            set
            {
                if (_AVS_B_Option != value)
                {
                    _AVS_B_Option = value;
                    OnPropertyChanged("AVS_B_Option");
                }
            }
        }

        public int AVS4TimeLimit
        {
            get
            {
                return _AVS4TimeLimit;
            }

            set
            {
                if (_AVS4TimeLimit != value)
                {
                    _AVS4TimeLimit = value;
                    OnPropertyChanged("AVS4TimeLimit");
                }
            }
        }

        public int AVS4UnderPressureLimit
        {
            get
            {
                return _AVS4UnderPressureLimit;
            }

            set
            {
                if (_AVS4UnderPressureLimit != value)
                {
                    _AVS4UnderPressureLimit = value;
                    OnPropertyChanged("AVS4UnderPressureLimit");
                }
            }
        }

        public int AVS4OverPressureLimit
        {
            get
            {
                return _AVS4OverPressureLimit;
            }

            set
            {
                if (_AVS4OverPressureLimit != value)
                {
                    _AVS4OverPressureLimit = value;
                    OnPropertyChanged("AVS4OverPressureLimit");
                }
            }
        }

        public int SUD_Setting
        {
            get
            {
                return _SUD_Setting;
            }

            set
            {
                if (_SUD_Setting != value)
                {
                    _SUD_Setting = value;
                    OnPropertyChanged("SUD_Setting");
                }
            }
        }

        public int ToolNumber
        {
            get
            {
                return _ToolNumber;
            }

            set
            {
                if (_ToolNumber != value)
                {
                    _ToolNumber = value;
                    OnPropertyChanged("ToolNumber");
                }
            }
        }

        public int CircleValveState
        {
            get
            {
                return _circleValveState;
            }

            set
            {
                if (_circleValveState != value)
                {
                    _circleValveState = value;
                    OnPropertyChanged("CircleValveState");
                }
            }
        }

        public int TestValveState
        {
            get
            {
                return _testValveState;
            }

            set
            {
                if (_testValveState != value)
                {
                    _testValveState = value;
                    OnPropertyChanged("TestValveState");
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
}
