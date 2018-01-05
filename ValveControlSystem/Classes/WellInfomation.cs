using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValveControlSystem.Classes
{
    public class WellInfomation : INotifyPropertyChanged
    {
        private string _time;
        private string _engineer;
        private string _client;
        private string _wellNo;
        private string _wellLocation;
        private string _serviceOrderNo;
        private string _runNumber;
        private string _note;
        private bool? _circleValveState;
        private bool? _testValveState;

        public string Time
        {
            get
            {
                return _time;
            }

            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged("Time");
                }
            }
        }

        public string Engineer
        {
            get
            {
                return _engineer;
            }

            set
            {
                if (_engineer != value)
                {
                    _engineer = value;
                    OnPropertyChanged("Engineer");
                }
            }
        }

        public string Client
        {
            get
            {
                return _client;
            }

            set
            {
                if (_client != value)
                {
                    _client = value;
                    OnPropertyChanged("Client");
                }
            }
        }

        public string WellNo
        {
            get
            {
                return _wellNo;
            }

            set
            {
                if (_wellNo != value)
                {
                    _wellNo = value;
                    OnPropertyChanged("WellNo");
                }
            }
        }

        public string WellLocation
        {
            get
            {
                return _wellLocation;
            }

            set
            {
                if (_wellLocation != value)
                {
                    _wellLocation = value;
                    OnPropertyChanged("WellLocation");
                }
            }
        }

        public string ServiceOrderNo
        {
            get
            {
                return _serviceOrderNo;
            }

            set
            {
                if (_serviceOrderNo != value)
                {
                    _serviceOrderNo = value;
                    OnPropertyChanged("ServiceOrderNo");
                }
            }
        }

        public string RunNumber
        {
            get
            {
                return _runNumber;
            }

            set
            {
                if (_runNumber != value)
                {
                    _runNumber = value;
                    OnPropertyChanged("RunNumber");
                }
            }
        }

        public string Note
        {
            get
            {
                return _note;
            }

            set
            {
                _note = value;
            }
        }

        public bool? CircleValveState
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

        public bool? TestValveState
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
