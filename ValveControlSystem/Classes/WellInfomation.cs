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
                _time = value;
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
                _engineer = value;
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
                _client = value;
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
                _wellNo = value;
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
                _wellLocation = value;
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
                _serviceOrderNo = value;
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
                _runNumber = value;
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
                _circleValveState = value;
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
                _testValveState = value;
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
