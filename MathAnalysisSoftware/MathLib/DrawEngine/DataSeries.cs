using System.Collections.Generic;
using System.Text;

namespace MathLib.DrawEngine
{
    public class DataSeries
    {
        private List<DataPoint> listDataPoints;
        private DataPoint pointMax = null;
        private DataPoint pointMin = null;
        private DataPoint amplitude = null;
        private double[] valX = null;
        private double[] valY = null;

        public DataSeries()
        {
            this.listDataPoints = new List<DataPoint>();
        }

        public DataSeries(double[] timeSeries)
        {
            this.listDataPoints = new List<DataPoint>();
            foreach (double val in timeSeries)
                AddDataPoint(val);
        }

        public string Name { get; set; }

        public List<DataPoint> ListDataPoints => this.listDataPoints;

        public DataPoint PointMax
        {
            get
            {
                if (pointMax == null)
                {
                    pointMax = new DataPoint(Ext.countMax(ValX), Ext.countMax(ValY));
                }

                return pointMax;
            }
        }

        public DataPoint PointMin
        {
            get
            {
                if (pointMin == null)
                {
                    pointMin = new DataPoint(Ext.countMin(valX), Ext.countMin(valY));
                }

                return pointMin;
            }
        }
        
        public DataPoint Amplitude
        {
            get
            {
                if (amplitude == null)
                {
                    amplitude = new DataPoint(PointMax.X - PointMin.X, PointMax.Y - PointMin.Y);
                }

                return amplitude;
            }
        }

        public int Length => this.listDataPoints.Count;
        
        private double[] ValX
        {
            get
            {
                if (valX == null || valX.Length != this.listDataPoints.Count)
                {
                    int ind = 0;
                    valX = new double[this.listDataPoints.Count];
                    foreach (DataPoint dp in this.listDataPoints)
                    {
                        valX[ind++] = dp.X;
                    }
                }
                return valX;
            }
        }
        
        public double[] ValY
        {
            get
            {
                if (valY == null || valY.Length != this.listDataPoints.Count)
                {
                    int ind = 0;
                    valY = new double[this.listDataPoints.Count];
                    foreach (DataPoint dp in this.listDataPoints)
                    {
                        valY[ind++] = dp.Y;
                    }
                }
                return valY;
            }
        }

        public void AddDataPoint(double x, double y)
        {
            this.listDataPoints.Add(new DataPoint(x, y));
        }

        public void AddDataPoint(double y)
        {
            this.listDataPoints.Add(new DataPoint(this.listDataPoints.Count + 1, y));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (DataPoint dp in this.listDataPoints)
            {
                sb.AppendFormat("{0:F5}\t{1:F5}\n", dp.X, dp.Y);
            }
                
            return sb.ToString();
        }
    }
}
