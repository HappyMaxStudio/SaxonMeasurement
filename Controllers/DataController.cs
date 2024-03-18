using SaxonSO2.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaxonSO2.Controllers
{
    internal class DataController
    {
        static string FileName { get; set; }
        static StreamWriter StreamWriterToFile { get; set; }
        public DataController(string fileName)
        {
            FileName = fileName;
            StreamWriterToFile = new StreamWriter(fileName, true);
        }
        //для построения и отображения таблицы результатов
        public static void SetResultsTable(TableLayoutPanel table, Mixture[] mixtures, int cyclesAmount, int mixturesAmount)
        {
            try
            {
                Font headerFont = new Font("Micrisoft Sans Serif", 9, FontStyle.Regular);
                table.RowCount = cyclesAmount + 3; //задаём необходимый размер таблицы и создаём Label в каждой ячейке
                table.ColumnCount = mixturesAmount;
                table.RowStyles.Clear();
                table.ColumnStyles.Clear();
                for (int i = 0; i < table.RowCount; i++)
                {
                    table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                }
                for (int i = 0; i < table.RowCount; i++)
                {
                    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
                }
                for (int i = 0; i < (table.RowCount * table.ColumnCount); i++)
                {
                    table.Controls.Add(new Label());
                }

                for (int i = 0; i < table.ColumnCount; i++) //заполняем итоговые данные и результаты смесей в таблицу
                {
                    table.GetControlFromPosition(i, 0).Text = mixtures[i].cylinderNumber;
                    table.GetControlFromPosition(i, 0).Font = headerFont;
                    table.GetControlFromPosition(i, table.RowCount - 1).Text = String.Concat("СКО", ":", mixtures[i].sko.ToString());
                    table.GetControlFromPosition(i, table.RowCount - 1).Font = headerFont;
                    table.GetControlFromPosition(i, table.RowCount - 2).Text = String.Concat("Сред.", ":", mixtures[i].average.ToString());
                    table.GetControlFromPosition(i, table.RowCount - 2).Font = headerFont;
                    for (int j = 1; j <= mixturesAmount; j++)
                    {
                        table.GetControlFromPosition(i, j).Text = mixtures[i].measurementResults[j - 1].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void SaveResults(MixtureController mixtureController)
        {
            try
            {
                using (StreamWriterToFile)
                {
                    StreamWriterToFile.WriteLine("Результаты измерений:");

                    foreach (Mixture gas in mixtureController.Mixtures)
                    {
                        StreamWriterToFile.WriteLine("Смесь:" + gas.cylinderNumber);
                        for (int i = 0; i < gas.measurementResults.Length; i++)
                        {
                            StreamWriterToFile.WriteLine("Цикл " + (i + 1).ToString() + " =" + gas.measurementResults[i].ToString());
                        }
                        StreamWriterToFile.WriteLine("\n");
                    }
                    MessageBox.Show("Результаты измерений сохранены в файл: " + FileName, "Результаты завершены", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);//поставить более конкретный тип исключения
            }
            finally
            {
                ResultsForm resultForm = new ResultsForm();
                SetResultsTable(resultForm.resultTable);
                resultForm.ShowDialog();
                mixtureController.ResetMixtures();
            }
        }
    }
}
