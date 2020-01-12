using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Data
{
    public class Timeseries
    {
        private DataPoint max;
        private DataPoint min;
        private DataPoint amplitude;
        private double[] xValues;
        private double[] yValues;
        private bool outdated;

        public Timeseries()
        {
            DataPoints = new List<DataPoint>();
            min = new DataPoint(0, 0);
            max = new DataPoint(0, 0);
            amplitude = new DataPoint(0, 0);
            outdated = true;
        }

        public Timeseries(double[] timeSeries) : this()
        {
            foreach (double val in timeSeries)
            {
                AddDataPoint(val);
            }
        }

        public string Name { get; set; }

        public List<DataPoint> DataPoints { get; protected set; }

        public DataPoint Max
        {
            get
            {
                if (outdated)
                {
                    UpdateProperties();
                }

                return max;
            }
        }

        public DataPoint Min
        {
            get
            {
                if (outdated)
                {
                    UpdateProperties();
                }

                return min;
            }
        }
        
        public DataPoint Amplitude
        {
            get
            {
                if (outdated)
                {
                    UpdateProperties();
                }

                return amplitude;
            }
        }

        public int Length => DataPoints.Count;
        
        private double[] XValues
        {
            get
            {
                if (outdated)
                {
                    xValues = (from dp in DataPoints select dp.X).ToArray();
                }

                return xValues;
            }
        }
        
        public double[] YValues
        {
            get
            {
                if (outdated)
                {
                    yValues = (from dp in DataPoints select dp.Y).ToArray();
                }

                return yValues;
            }
        }

        public void AddDataPoint(double x, double y)
        {
            DataPoints.Add(new DataPoint(x, y));
            outdated = true;
        }

        public void AddDataPoint(double y)
        {
            DataPoints.Add(new DataPoint(DataPoints.Count + 1, y));
            outdated = true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (DataPoint dp in DataPoints)
            {
                sb.AppendFormat("{0:G14}\t{1:G14}\n", dp.X, dp.Y);
            }
                
            return sb.ToString();
        }

        private void UpdateProperties()
        {
            min.X = XValues.Min();
            min.Y = YValues.Min();
            max.X = XValues.Max();
            max.Y = YValues.Max();
            amplitude.X = max.X - min.X;
            amplitude.Y = max.Y - min.Y;
            outdated = false;
        }
    }
}
