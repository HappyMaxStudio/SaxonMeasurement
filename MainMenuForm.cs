using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Windows.Forms.DataVisualization.Charting;
using SaxonSO2.Controllers;
using SaxonSO2.Models;



namespace SaxonSO2
{
    public partial class MainMenuForm : Form
    {
        TextBox[] results = new TextBox[7];
        TextBox[] serieses = new TextBox[7];

        MixtureUI mixtureUI;
        DataController dataController;
        MeasurementController measurementController;
        MixtureController mixtureController;
        SerialPortController serialController;

        public static ComboBox box;

        public MainMenuForm()
        {
            InitializeComponent();
            InitializeTBoxArrays();
            dataController = new DataController($"{dateLabel.Text}.txt");
            measurementController = new MeasurementController(currentMeasurementChart, autoMeasurementChart, mixtureUI);
            mixtureController = new MixtureController();
            serialController = new SerialPortController("COM1");
            dataController = new DataController("SaveLog.txt");
            measurementController.SetCharts();
            dateAndTimeTimer.Start();
            box = mixturesCBox;
        }   
        private void InitializeTBoxArrays()
        {
            results[0] = result1TBox;
            results[1] = result2TBox;
            results[2] = result3TBox;
            results[3] = result4TBox;
            results[4] = result5TBox;
            results[5] = resultSkoTBox;
            results[6] = resultAvgTBox;
            serieses[0] = series1TBox;
            serieses[1] = series2TBox;
            serieses[2] = series3TBox;
            serieses[3] = series4TBox;
            serieses[4] = series5TBox;
            serieses[5] = skoTBox;
            serieses[6] = averageTBox;
        }
        private void FillTBoxes()//заполнение таблички результатов измерений(не итоговой)
        {
            foreach(Mixture mixture in mixtureController.Mixtures)
            {

                if (mixture.cylinderNumber == mixturesCBox.Text)
                {
                    for(int i = 0; i < 5; i++)
                    {
                        serieses[i].Text = i < mixture.measurementResults.Length ? $"Серия {i + 1}" : string.Empty;
                        results[i].Text = i < mixture.measurementResults.Length ? mixture.measurementResults[i].ToString() : string.Empty;
                    }
                    results[5].Text = mixture.sko.ToString();
                    results[6].Text = mixture.average.ToString();
                    break;
                }
            }
        }
        private void quitBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void mixturesCBox_TextChanged(object sender, EventArgs e)
        {
            FillTBoxes();
        }
        private void dateAndTimeTimer_Tick(object sender, EventArgs e)
        {
            dateLabel.Text = DateTime.Now.ToLongDateString();
            timeLabel.Text = DateTime.Now.ToLongTimeString();
            temperatureLbl.Text = string.Concat(new Random().Next(30, 40), "'С");
            pressureLbl.Text = string.Concat(new Random().Next(10), "атм.");
        }

        //----Кнопки----

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Saxon Monitoring. Версия 0.9", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SystemSounds.Question.Play();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialController.ClosePort();
        }
        private void autoMeasurementBtn_Click(object sender, EventArgs e)
        {
            setAutoMeasurementForm form2 = new setAutoMeasurementForm();
            form2.ShowDialog();
            measurementController.StartAutoMeasurement();
        }
        
    }
}




