using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformsLabel = System.Windows.Forms.Label;

namespace SaxonSO2.Models
{
    public class MixtureUI
    {
        public Button StopBtn {  get; set; }
        public Button StartSavingBtn { get; set; }
        public ToolStripButton ManualBtn { get; set; }
        public ComboBox MixturesBox { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public WinformsLabel CurrentMixtureLabel {  get; set; }
        public WinformsLabel CurrentConcentrationLabel { get; set; }

        public MixtureUI(Button stopBtn, Button startSavingBtn, ToolStripButton manualBtn, ComboBox mixtureBox, ProgressBar progressBar, WinformsLabel mixtureLabel, WinformsLabel currentConcentrationLabel)
        {
            StopBtn = stopBtn;
            StartSavingBtn = startSavingBtn;
            ManualBtn = manualBtn;
            MixturesBox = mixtureBox;
            ProgressBar = progressBar;
            CurrentMixtureLabel = mixtureLabel;
            CurrentConcentrationLabel = currentConcentrationLabel;
        }

    }
}
