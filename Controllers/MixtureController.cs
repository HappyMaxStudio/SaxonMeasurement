using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SaxonSO2.Models;
using SaxonSO2.Controllers;

namespace SaxonSO2.Controllers
{
    public class MixtureController : IEnumerable
    {
        public  List<Mixture> Mixtures = new List<Mixture>();
        public Mixture this[int index]
        {
            get => Mixtures[index];
        }
        public  void CreateMixtureList(string[] numbersArray, int mixturesCount, int cyclesCount)
        {
            for (int i = 0; i < mixturesCount; i++)
            {
                Mixtures.Add(new Mixture(numbersArray[i], new double[cyclesCount]));
            }
            MainMenuForm.box.Items.AddRange(ReturnMixtureNames());
        }
        public  string[] ReturnMixtureNames()
        {
            return Mixtures.Select(x => x.cylinderNumber).ToArray();
        }

        //сохранение текущего результата в конкретный цикл конкретной смеси
        public  void SetMixtureConcentration(int mixture, int cycle, double concentration, MeasurementController.MixtureOrNullGas mixtureOrN2)
        {
            Mixtures[mixture].measurementResults[cycle] = mixtureOrN2 == MeasurementController.MixtureOrNullGas.Mixture ? concentration : Mixtures[mixture].measurementResults[cycle] - concentration;
        }
        public  void UpdateAllAverages()
        {
            Mixtures.ForEach(mixture => mixture.average = mixture.measurementResults.Average());
        }
        public  void ResetMixtures()
        {
            Mixtures.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            for(int i = 0;i < Mixtures.Count; i++)
            {
                yield return Mixtures[i];
            }
        }
    }
}
