using System.Collections.Generic;
using System.Text;

namespace MathLib.DrawEngine
{
    public class DataSeries
    {
        public string Name;

        public List<DataPoint> ListDataPoints;


        private DataPoint _PointMax = null;
        public DataPoint PointMax
        {
            get
            {
                if (_PointMax == null)
                    _PointMax = new DataPoint(Ext.countMax(ValX), Ext.countMax(ValY));

                return _PointMax;
            }
        }


        private DataPoint _PointMin = null;
        public DataPoint PointMin
        {
            get
            {
                if (_PointMin == null)
                    _PointMin = new DataPoint(Ext.countMin(_ValX), Ext.countMin(_ValY));

                return _PointMin;
            }
        }


        private DataPoint _Amplitude = null;
        public DataPoint Amplitude
        {
            get
            {
                if (_Amplitude == null)
                    _Amplitude = new DataPoint(PointMax.X - PointMin.X, PointMax.Y - PointMin.Y);

                return _Amplitude;
            }
        }


        public int Length
        {
            get
            {
                return ListDataPoints.Count;
            }
        }


        private double[] _ValX = null;
        private double[] ValX
        {
            get
            {
                if (_ValX == null || _ValX.Length != ListDataPoints.Count)
                {
                    int ind = 0;
                    _ValX = new double[ListDataPoints.Count];
                    foreach (DataPoint dp in ListDataPoints)
                        _ValX[ind++] = dp.X;
                }
                return _ValX;
            }
        }

        private double[] _ValY = null;
        public double[] ValY
        {
            get
            {
                if (_ValY == null || _ValY.Length != ListDataPoints.Count)
                {
                    int ind = 0;
                    _ValY = new double[ListDataPoints.Count];
                    foreach (DataPoint dp in ListDataPoints)
                        _ValY[ind++] = dp.Y;
                }
                return _ValY;
            }
        }

        public DataSeries()
        {
            ListDataPoints = new List<DataPoint>();
        }

        public DataSeries(double[] timeSeries)
        {
            ListDataPoints = new List<DataPoint>();
            foreach (double val in timeSeries)
                AddDataPoint(val);
        }

        public void AddDataPoint(double x, double y)
        {
            ListDataPoints.Add(new DataPoint(x, y));
        }

        public void AddDataPoint(double y)
        {
            ListDataPoints.Add(new DataPoint(ListDataPoints.Count + 1, y));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataPoint dp in ListDataPoints)
                sb.AppendFormat("{0:F5}\t{1:F5}\n", dp.X, dp.Y);
            return sb.ToString();
        }
    }
}
