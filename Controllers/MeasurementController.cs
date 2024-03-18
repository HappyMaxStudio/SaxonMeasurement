using SaxonSO2.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SaxonSO2.Controllers
{
    public class MeasurementController
    {
        int CurrentCycle {  get; set; }
        public int CurrentMixture { get;private set; }
        int CyclesAmount { get; set; }
        int MixturesAmount { get; set; }
        double CurrentConcentration { get; set; }
        List<double> ConcentrationsList { get; set; }
        MixtureUI MixtureUI { get; set; }
        Font SmallChartFont { get; set; }
        Chart CurrentMeasurementChart { get; set; }
        Chart AutoMeasurementChart { get; set; }
        Random Random { get; set; }
        Timer MeasurementTimer { get; set; }
        Timer ResultsTimer { get; set; }
        MixtureController MixtureController { get; set; }

        public enum MeasurementType
        {
            Manual, Auto, Stop
        }
        public enum MixtureOrNullGas
        {
            Mixture, NullGas
        }
        public MeasurementType measurementType {  get; set; }
        public MixtureOrNullGas mixtureOrNullGas {  get; set; }
        public MeasurementController(Chart currentMeasurementChart, Chart autoMeasurementChart, MixtureUI mixtureUI)
        {
            CurrentMeasurementChart = currentMeasurementChart;
            AutoMeasurementChart = autoMeasurementChart;
            SmallChartFont = new Font("Micrisoft Sans Serif", 7, FontStyle.Regular);
            Random = new Random();
            MixtureUI = mixtureUI;
            MixtureUI.ManualBtn.Click += manualBtn_Click;
            MixtureUI.StartSavingBtn.Click += startSavingBtn_Click;
            MixtureUI.StopBtn.Click += stopBtn_Click;
        }
        public void StartAutoMeasurement()
        {
            MixtureUI.StartSavingBtn.Enabled = true;
            MixtureUI.StopBtn.Enabled = true;
            MeasurementTimer.Start();
            SetCharts();
            MixtureUI.ManualBtn.Enabled = false;
            mixtureOrNullGas = MixtureOrNullGas.Mixture;
        }
        public void StartManualMeasurement()
        {
            MeasurementTimer.Start();
            measurementType = MeasurementType.Manual;
        }
        public void StartAccumulation()
        {
            MixtureUI.ProgressBar.Value = 400;
            ResultsTimer.Start();
            MixtureUI.StartSavingBtn.Enabled = false;
            MixtureUI.StopBtn.Enabled = false;
        }
        public void StartSavingConcentration()
        {
            ConcentrationsList.Add(CurrentConcentration);
            if (ConcentrationsList.Count == 10)
            {
                ResultsTimer.Stop();
                measurementType = MeasurementType.Stop;
                MixtureUI.ProgressBar.Value = 0;
                MixtureUI.StartSavingBtn.Enabled = true;
                MixtureUI.StopBtn.Enabled = true;
                MixtureController.SetMixtureConcentration(CurrentMixture, CurrentCycle, ConcentrationsList.Average(), mixtureOrNullGas);
                MixtureController.UpdateAllAverages();
                if (mixtureOrNullGas == MixtureOrNullGas.NullGas)
                {
                    mixtureOrNullGas = MixtureOrNullGas.Mixture;
                    if (CurrentMixture < MixturesAmount - 1)
                    {
                        CurrentMixture++;
                        NextMixtureForm nextMixture = new NextMixtureForm(this, MixtureController);
                        nextMixture.ShowDialog();
                    }
                    else
                    {
                        CurrentMixture = 0;
                        if (CurrentCycle < CyclesAmount - 1)
                        {
                            CurrentCycle++;
                            NextMixtureForm nextMixture = new NextMixtureForm(this, MixtureController);
                            nextMixture.ShowDialog();
                        }
                        else
                        {
                            StopMeasurement();
                        }
                    }
                }
                else
                {
                    mixtureOrNullGas = MixtureOrNullGas.NullGas;
                    NextMixtureForm nextMixture = new NextMixtureForm(this, MixtureController);
                    nextMixture.ShowDialog();

                }
                ConcentrationsList.Clear();
            }
        }
        public void ProceedMeasurement()
        {
            CurrentConcentration = mixtureOrNullGas == MixtureOrNullGas.Mixture ? Random.NextDouble() * (15) + 950 : Random.Next(10);
            MixtureUI.CurrentConcentrationLabel.Text = $"{CurrentConcentration.ToString("#.##")}, mV";
            CurrentMeasurementChart.ChartAreas[0].AxisY.Minimum = (int)CurrentConcentration - 20;
            CurrentMeasurementChart.ChartAreas[0].AxisY.Maximum = (int)CurrentConcentration + 20;
            CurrentMeasurementChart.Series[0].Points.AddXY(DateTime.Now, (int)CurrentConcentration);
            if (DateTime.Now.ToOADate() > CurrentMeasurementChart.ChartAreas[0].AxisX.Maximum)
            {
                CurrentMeasurementChart.ChartAreas[0].AxisX.Minimum = DateTime.Now.AddSeconds(-45).ToOADate();
                CurrentMeasurementChart.ChartAreas[0].AxisX.Maximum = DateTime.Now.AddSeconds(45).ToOADate();
            }
            if (measurementType == MeasurementType.Auto)
            {
                MixtureUI.CurrentMixtureLabel.Text = mixtureOrNullGas == MixtureOrNullGas.Mixture ? MixtureController[CurrentMixture].cylinderNumber : "Нулевой газ";
                MixtureUI.ProgressBar.PerformStep();
                AutoMeasurementChart.Series[0].Points.AddXY(DateTime.Now, CurrentConcentration);
                AutoMeasurementChart.ChartAreas[0].AxisX.Maximum = DateTime.Now.AddSeconds(1).ToOADate();
                if (MixtureUI.ProgressBar.Value == 400)
                {
                    ResultsTimer.Start();
                }
            }
        }
        public void SetCharts()
        {
            CurrentMeasurementChart.ChartAreas[0].AxisY.Minimum = (int)CurrentConcentration - 30;
            CurrentMeasurementChart.ChartAreas[0].AxisY.Maximum = (int)CurrentConcentration + 30;
            CurrentMeasurementChart.ChartAreas[0].AxisX.LabelStyle.Format = "H.mm.ss";
            CurrentMeasurementChart.ChartAreas[0].AxisX.LabelStyle.Font = SmallChartFont;
            CurrentMeasurementChart.Series[0].XValueType = ChartValueType.DateTime;
            CurrentMeasurementChart.ChartAreas[0].AxisX.Minimum = DateTime.Now.ToOADate();
            CurrentMeasurementChart.ChartAreas[0].AxisX.Maximum = DateTime.Now.AddMinutes(1).ToOADate();
            CurrentMeasurementChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            CurrentMeasurementChart.ChartAreas[0].AxisX.Interval = 5;
            AutoMeasurementChart.ChartAreas[0].AxisY.Minimum = 0;
            AutoMeasurementChart.ChartAreas[0].AxisY.Maximum = 1500;
            AutoMeasurementChart.ChartAreas[0].AxisX.LabelStyle.Format = "H.mm.ss";
            AutoMeasurementChart.Series[0].XValueType = ChartValueType.DateTime;
            AutoMeasurementChart.ChartAreas[0].AxisX.Minimum = DateTime.Now.ToOADate();
            AutoMeasurementChart.ChartAreas[0].AxisX.Maximum = DateTime.Now.AddMinutes(1).ToOADate();
            AutoMeasurementChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            AutoMeasurementChart.ChartAreas[0].AxisX.Interval = 5;
        }
        public void StopMeasurement()
        {
            MixtureUI.ProgressBar.Value = 400;
            MixtureUI.MixturesBox.Items.Clear();
            measurementType = MeasurementType.Manual;
            CurrentCycle = 0;
            CurrentMixture = 0;
            MixtureUI.StopBtn.Enabled = false;
            MixtureUI.StartSavingBtn.Enabled = false;
            MixtureUI.ManualBtn.Enabled = true;
            DataController.SaveResults(MixtureController);
        }

        private void manualBtn_Click(object sender, EventArgs e)
        {
            StartManualMeasurement();
        }
        private void startSavingBtn_Click(object sender, EventArgs e)//обработчки кнопки накопления
        {
            StartAccumulation();
        }
        private void stopBtn_Click(object sender, EventArgs e)
        {
            StopMeasurement();
        }
        private void resultsTimer_Tick(object sender, EventArgs e)
        {
            StartSavingConcentration();
        }
        private void measurementTimer_Tick(object sender, EventArgs e)//каждая секунда измерений
        {
            ProceedMeasurement();
        }
    }
}
