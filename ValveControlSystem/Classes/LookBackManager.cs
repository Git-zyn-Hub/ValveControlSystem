using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visifire.Charts;

namespace ValveControlSystem.Classes
{
    public class LookBackManager
    {
        private static LookBackManager _uniqueInstance;
        //常规采集压力
        private List<List<DataPoint>> _listCurveNorPres = new List<List<DataPoint>>();
        //常规采集温度
        private List<List<DataPoint>> _listCurveNorTemp = new List<List<DataPoint>>();
        //指令采集压力
        private List<List<DataPoint>> _listCurveCmdPres = new List<List<DataPoint>>();
        //指令采集温度
        private List<List<DataPoint>> _listCurveCmdTemp = new List<List<DataPoint>>();

        //最后一天的压力温度曲线数据点。
        private List<DataPoint> _listNorPresLastDay;
        private List<DataPoint> _listNorTempLastDay;
        private List<DataPoint> _listCmdPresLastDay;
        private List<DataPoint> _listCmdTempLastDay;

        private LightDataPoint[] _points;
        private int _pageCurrent = 0;
        private int _pagesBack = 0;
        private int _pagesForward = 0;
        private int _pagesTotal = 0;
        private int _dayCount1Page = 0;
        private CurveSetXmlHelper _helper = new CurveSetXmlHelper();

        public int PageCurrent
        {
            get
            {
                return _pageCurrent;
            }

            set
            {
                _pageCurrent = value;
            }
        }

        public int PagesBack
        {
            get
            {
                return _pagesBack;
            }

            set
            {
                _pagesBack = value;
            }
        }

        public int PagesForward
        {
            get
            {
                return _pagesForward;
            }

            set
            {
                _pagesForward = value;
            }
        }

        public int PagesTotal
        {
            get
            {
                return _pagesTotal;
            }

            set
            {
                _pagesTotal = value;
            }
        }
        public int DayCount1Page
        {
            get
            {
                return _dayCount1Page;
            }

            set
            {
                _dayCount1Page = value;
            }
        }

        /// <summary>
        /// 常规采集压力
        /// </summary>
        public List<List<DataPoint>> ListCurveNorPres
        {
            get
            {
                return _listCurveNorPres;
            }

            set
            {
                _listCurveNorPres = value;
            }
        }

        /// <summary>
        /// 常规采集温度,每天一个List，有多少天，就有多少Count
        /// </summary>
        public List<List<DataPoint>> ListCurveNorTemp
        {
            get
            {
                return _listCurveNorTemp;
            }

            set
            {
                _listCurveNorTemp = value;
            }
        }

        /// <summary>
        /// 指令采集压力
        /// </summary>
        public List<List<DataPoint>> ListCurveCmdPres
        {
            get
            {
                return _listCurveCmdPres;
            }

            set
            {
                _listCurveCmdPres = value;
            }
        }

        /// <summary>
        /// 指令采集温度
        /// </summary>
        public List<List<DataPoint>> ListCurveCmdTemp
        {
            get
            {
                return _listCurveCmdTemp;
            }

            set
            {
                _listCurveCmdTemp = value;
            }
        }


        private LookBackManager()
        {
            _helper.CurveSettingXmlInitial();
            GetAndRefreshDayCount1Page();
        }
        public static LookBackManager GetInstance()
        {
            if (_uniqueInstance == null)
            {
                _uniqueInstance = new LookBackManager();
            }
            return _uniqueInstance;
        }

        public static void CloseInstance()
        {
            _uniqueInstance = null;
        }

        public void SavePointsNorPres(DataPoint p)
        {
            int countDay = _listCurveNorPres.Count;
            if (countDay == 0)
            {
                _listNorPresLastDay = new List<DataPoint>();
                _listNorPresLastDay.Add(p);
                _listCurveNorPres.Add(_listNorPresLastDay);
            }
            else
            {
                int countPointOfLastDay = _listCurveNorPres[countDay - 1].Count;
                DateTime timeNewPoint = (DateTime)p.XValue;
                DateTime timeLastPoint = (DateTime)_listCurveNorPres[countDay - 1][countPointOfLastDay - 1].XValue;
                if (timeNewPoint.Day != timeLastPoint.Day)
                {
                    _listNorPresLastDay = null;
                    _listNorPresLastDay = new List<DataPoint>();
                    _listCurveNorPres.Add(_listNorPresLastDay);
                    countDay++;
                }
                _listCurveNorPres[countDay - 1].Add(p);
            }
        }


        public void SavePointsNorTemp(DataPoint p)
        {
            int countDay = _listCurveNorTemp.Count;
            if (countDay == 0)
            {
                _listNorTempLastDay = new List<DataPoint>();
                _listNorTempLastDay.Add(p);
                _listCurveNorTemp.Add(_listNorTempLastDay);
            }
            else
            {
                int countPointOfLastDay = _listCurveNorTemp[countDay - 1].Count;
                DateTime timeNewPoint = (DateTime)p.XValue;
                DateTime timeLastPoint = (DateTime)_listCurveNorTemp[countDay - 1][countPointOfLastDay - 1].XValue;
                if (timeNewPoint.Day != timeLastPoint.Day)
                {
                    _listNorTempLastDay = null;
                    _listNorTempLastDay = new List<DataPoint>();
                    _listCurveNorTemp.Add(_listNorTempLastDay);
                    countDay++;
                }
                _listCurveNorTemp[countDay - 1].Add(p);
            }
        }
        public void SavePointsCmdPres(DataPoint p)
        {
            int countDay = _listCurveCmdPres.Count;
            if (countDay == 0)
            {
                _listCmdPresLastDay = new List<DataPoint>();
                _listCmdPresLastDay.Add(p);
                _listCurveCmdPres.Add(_listCmdPresLastDay);
            }
            else
            {
                int countPointOfLastDay = _listCurveCmdPres[countDay - 1].Count;
                DateTime timeNewPoint = (DateTime)p.XValue;
                DateTime timeLastPoint = (DateTime)_listCurveCmdPres[countDay - 1][countPointOfLastDay - 1].XValue;
                if (timeNewPoint.Day != timeLastPoint.Day)
                {
                    _listCmdPresLastDay = null;
                    _listCmdPresLastDay = new List<DataPoint>();
                    _listCurveCmdPres.Add(_listCmdPresLastDay);
                    countDay++;
                }
                _listCurveCmdPres[countDay - 1].Add(p);
            }
        }
        public void SavePointsCmdTemp(DataPoint p)
        {
            int countDay = _listCurveCmdTemp.Count;
            if (countDay == 0)
            {
                _listCmdTempLastDay = new List<DataPoint>();
                _listCmdTempLastDay.Add(p);
                _listCurveCmdTemp.Add(_listCmdTempLastDay);
            }
            else
            {
                int countPointOfLastDay = _listCurveCmdTemp[countDay - 1].Count;
                DateTime timeNewPoint = (DateTime)p.XValue;
                DateTime timeLastPoint = (DateTime)_listCurveCmdTemp[countDay - 1][countPointOfLastDay - 1].XValue;
                if (timeNewPoint.Day != timeLastPoint.Day)
                {
                    _listCmdTempLastDay = null;
                    _listCmdTempLastDay = new List<DataPoint>();
                    _listCurveCmdTemp.Add(_listCmdTempLastDay);
                    countDay++;
                }
                _listCurveCmdTemp[countDay - 1].Add(p);
            }
        }

        public void InitialParameters()
        {
            int count = _listCurveNorPres.Count;
            GetAndRefreshDayCount1Page();
            PageCurrent = PagesTotal;
            PagesBack = PageCurrent - 1;
            PagesBack = PagesBack < 0 ? 0 : PagesBack;
            PagesForward = 0;
        }

        public void ClearDataPoints()
        {
            foreach (var oneday in _listCurveCmdPres)
            {
                oneday.Clear();
            }
            foreach (var oneday in _listCurveCmdTemp)
            {
                oneday.Clear();
            }
            foreach (var oneday in _listCurveNorPres)
            {
                oneday.Clear();
            }
            foreach (var oneday in _listCurveNorTemp)
            {
                oneday.Clear();
            }
            _listCurveCmdPres.Clear();
            _listCurveCmdTemp.Clear();
            _listCurveNorPres.Clear();
            _listCurveNorTemp.Clear();
            this.PageCurrent = 0;
            this.PagesBack = 0;
            this.PagesForward = 0;
            GC.Collect();
        }

        public void Back()
        {
            int count = _listCurveNorPres.Count;
            GetAndRefreshDayCount1Page();
            if (PageCurrent > 1)
            {
                PageCurrent--;
            }
            if (PagesBack > 0)
            {
                PagesBack--;
            }
            PagesForward = PagesTotal - PageCurrent;
        }

        public void Forward()
        {
            int count = _listCurveNorPres.Count;
            GetAndRefreshDayCount1Page();
            if (PageCurrent < PagesTotal)
            {
                PageCurrent++;
            }
            if (PagesForward > 0)
            {
                PagesForward = PagesTotal - PageCurrent;
            }
            PagesBack = PageCurrent - 1 < 0 ? 0 : PageCurrent - 1;
        }

        public bool HasDataPoints()
        {
            return _listCurveNorPres.Count > 0;
        }

        public int GetAndRefreshDayCount1Page()
        {
            CurveGeneralSetting cgs = _helper.GetCurveGeneralSetting();
            string strCount = cgs.DayCount1Page;
            if (strCount == "全部")
            {
                DayCount1Page = 0;
                PagesTotal = 1;
            }
            else
            {
                string strNum = strCount.Remove(strCount.IndexOf("天"));
                if (!int.TryParse(strNum, out _dayCount1Page))
                {
                    throw new Exception("参数错误");
                }
                else
                {
                    int dayTotal = _listCurveNorTemp.Count;
                    int temp = dayTotal / DayCount1Page;
                    PagesTotal = dayTotal % DayCount1Page == 0 ? temp : temp + 1;
                }
            }
            return DayCount1Page;
        }
    }
}
