using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using SaxonSO2.Controllers;
using SaxonSO2.Models;

using static SaxonSO2.Controllers.MeasurementController.MixtureOrNullGas;

namespace SaxonSO2
{
    public partial class NextMixtureForm : Form
    {
        MeasurementController measurementController1;
        public NextMixtureForm(MeasurementController measurementController, MixtureController mixtureController)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            mixtureNameLbl.Text = measurementController.mixtureOrNullGas == MeasurementController.MixtureOrNullGas.Mixture ? "Нулевой газ" : mixtureController.Mixtures[measurementController.CurrentMixture].cylinderNumber;
            SystemSounds.Exclamation.Play();
            measurementController1 = measurementController;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            measurementController1.StopMeasurement();
            this.Close();
        }
    }
}
