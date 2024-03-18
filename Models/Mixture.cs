using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaxonSO2.Models
{
    public class Mixture : IEnumerable
    {
        public string cylinderNumber;
        public double[] measurementResults;
        public double average = 0;
        public double sko = 0; //пока не рассчитывается
        public double this[int index]
        {
            get => measurementResults[index];
        }
        public Mixture(string number, double[] array)
        {
            cylinderNumber = number;
            measurementResults = array;
        }

        public IEnumerator GetEnumerator()
        {
            for(int i = 0; i < measurementResults.Length; i++)
            {
                yield return measurementResults[i];
            }
        }
    }
}
