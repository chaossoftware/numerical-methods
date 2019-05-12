using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MathLib.DrawEngine
{
    partial class MathChart
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            var chartArea = new ChartArea();
            chartArea.Name = "ChartArea";
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisX.IsLabelAutoFit = true;
            chartArea.AxisX.LabelStyle.Format = "G5";
            chartArea.AxisX.LabelStyle.TruncatedLabels = true;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsLabelAutoFit = true;
            //schartArea.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;

            chartArea.AxisX.LabelAutoFitStyle = 
                LabelAutoFitStyles.IncreaseFont |
                LabelAutoFitStyles.DecreaseFont |
                LabelAutoFitStyles.StaggeredLabels;

            chartArea.AxisY.LabelAutoFitStyle = 
                LabelAutoFitStyles.IncreaseFont |
                LabelAutoFitStyles.DecreaseFont |
                LabelAutoFitStyles.StaggeredLabels;
            
            this.ChartAreas.Add(chartArea);

            this.Cursor = Cursors.Cross;
            this.Palette = ChartColorPalette.SeaGreen;
        }

        #endregion
    }
}
