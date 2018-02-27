using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Data
{
    public class Timeseries
    {
        private List<DataPoint> dataPoints;
        private DataPoint max;
        private DataPoint min;
        private DataPoint amplitude;
        private double[] xValues;
        private double[] yValues;
        private bool outdated;

        public Timeseries()
        {
            this.Init();
        }

        public Timeseries(double[] timeSeries)
        {
            this.Init();

            foreach (double val in timeSeries)
            {
                AddDataPoint(val);
            }
        }

        public string Name { get; set; }

        public List<DataPoint> DataPoints => this.dataPoints;

        public DataPoint Max
        {
            get
            {
                if (this.outdated)
                {
                    UpdateProperties();
                }

                return this.max;
            }
        }

        public DataPoint Min
        {
            get
            {
                if (this.outdated)
                {
                    UpdateProperties();
                }

                return this.min;
            }
        }
        
        public DataPoint Amplitude
        {
            get
            {
                if (this.outdated)
                {
                    UpdateProperties();
                }

                return this.amplitude;
            }
        }

        public int Length => this.dataPoints.Count;
        
        private double[] XValues
        {
            get
            {
                if (this.outdated)
                {
                    this.xValues = (from dp in this.DataPoints select dp.X).ToArray();
                }
                return xValues;
            }
        }
        
        public double[] YValues
        {
            get
            {
                if (this.outdated)
                {
                    this.yValues = (from dp in this.DataPoints select dp.Y).ToArray();
                }
                return yValues;
            }
        }

        public void AddDataPoint(double x, double y)
        {
            this.dataPoints.Add(new DataPoint(x, y));
            this.outdated = true;
        }

        public void AddDataPoint(double y)
        {
            this.dataPoints.Add(new DataPoint(this.dataPoints.Count + 1, y));
            this.outdated = true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (DataPoint dp in this.dataPoints)
            {
                sb.AppendFormat("{0:G14}\t{1:G14}\n", dp.X, dp.Y);
            }
                
            return sb.ToString();
        }

        private void UpdateProperties()
        {
            this.min.X = this.XValues.Min();
            this.min.Y = this.YValues.Min();
            this.max.X = this.XValues.Max();
            this.max.Y = this.YValues.Max();
            this.amplitude.X = this.max.X - this.min.X;
            this.amplitude.Y = this.max.Y - this.min.Y;
            this.outdated = false;
        }

        private void Init()
        {
            this.dataPoints = new List<DataPoint>();
            this.min = new DataPoint(0, 0);
            this.max = new DataPoint(0, 0);
            this.amplitude = new DataPoint(0, 0);
            this.outdated = true;
        }
    }
}
