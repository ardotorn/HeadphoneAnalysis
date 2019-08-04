using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace HeadphoneGraphAnalysis
{
    public class FreqAndDb
    {
        public int frequency;
        public double db;
    }

    public class NameAndDif
    {
        public string name;
        public double difference;
    }

    static public class Tools
    {
        static public void LoadCsv(string path, ref FreqAndDb[] frequencyPlot)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                csvParser.ReadLine();

                int arrayIndex = 0;
                while (!csvParser.EndOfData)
                {

                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    string frequency = fields[0];
                    string db = fields[1];

                    //add the items to the freq and db object
                    double temp = Convert.ToDouble(frequency);
                    frequencyPlot[arrayIndex].frequency = (int)temp;
                    frequencyPlot[arrayIndex].db = Convert.ToDouble(db);
                    //move to the next item in the array
                    arrayIndex++;


                }
            }
        }

        static public void Normalize(ref FreqAndDb[] frequencyPlot)
        {
            //total offset from 0
            double dbTotal = 0;
            for (int i = 0; i < 613; i++)
                dbTotal = dbTotal + frequencyPlot[i].db;

            //average offset from 0
            double dbAverage;
            dbAverage = dbTotal / 613;

            //change offset to 0
            for (int i = 0; i < 613; i++)
                frequencyPlot[i].db = frequencyPlot[i].db - dbAverage;
        }

        static public double Compare(FreqAndDb[] frequencyPlot, FreqAndDb[] frequencyPlot2)
        {
            //Total difference of the two charts
            double totalDifference=0;
            for (int i = 0; i < 613; i++)
            {
                totalDifference = totalDifference + Math.Abs(frequencyPlot[i].db - frequencyPlot2[i].db);
            }

            //Average difference
            double averageDifference = totalDifference / 613;

            return averageDifference;
        }
        static public FreqAndDb[] AverageProcess(FreqAndDb[] frequencyPlot, FreqAndDb[] frequencyPlot2, FreqAndDb[] frequencyPlot3, FreqAndDb[] frequencyPlot4)
        {
            //Create new Plot
            FreqAndDb[] outputFrequencyPlot = new FreqAndDb[613];
            //Total difference of the two charts
            Tools.newChart(ref outputFrequencyPlot);

            for (int i = 0; i < 613; i++)
            {
                outputFrequencyPlot[i].frequency = frequencyPlot[i].frequency;
                outputFrequencyPlot[i].db = (frequencyPlot[i].db + frequencyPlot2[i].db + frequencyPlot3[i].db + frequencyPlot4[i].db) / 4;
            }

            return outputFrequencyPlot;
            
        }

        static public void newChart(ref FreqAndDb[] frequencyPlot)
        {
            for (int i = 0; i < 613; i++)
            {
                frequencyPlot[i] = new FreqAndDb();
            }
        }
        
        static public void Process(string path, ref FreqAndDb[] frequencyPlot)
        {
            //Lets make the freq and db object ready
            Tools.newChart(ref frequencyPlot);
            //Load data into the freq and db object
            Tools.LoadCsv(path, ref frequencyPlot);
            Tools.Normalize(ref frequencyPlot);
        }

        static public FreqAndDb[] AverageResult()
        {
            //Path of our csv 1
            var path = @"C:\Users\Marika\Documents\Visual Studio 2013\Projects\HeadphoneGraphAnalysis\data\Stax SR-009.csv"; 

            //Lets make the freq and db object ready
            FreqAndDb[] frequencyPlot = new FreqAndDb[613];
            Tools.Process(path, ref frequencyPlot);

            //Path of our csv 2
            var path2 = @"C:\Users\Marika\Documents\Visual Studio 2013\Projects\HeadphoneGraphAnalysis\data\Sennheiser HD 598.csv"; 

            //Lets make the freq and db object ready
            FreqAndDb[] frequencyPlot2 = new FreqAndDb[613];
            Tools.Process(path2, ref frequencyPlot2);

            //Path of our csv 3
            var path3 = @"C:\Users\Marika\Documents\Visual Studio 2013\Projects\HeadphoneGraphAnalysis\data\Sennheiser HD 600.csv";

            //Lets make the freq and db object ready
            FreqAndDb[] frequencyPlot3 = new FreqAndDb[613];
            Tools.Process(path3, ref frequencyPlot3);

            //Path of our csv 4
            var path4 = @"C:\Users\Marika\Documents\Visual Studio 2013\Projects\HeadphoneGraphAnalysis\data\Audio Technica ATH-M50x.csv";

            //Lets make the freq and db object ready
            FreqAndDb[] frequencyPlot4 = new FreqAndDb[613];
            Tools.Process(path4, ref frequencyPlot4);

            //Create average chart from 2 charts
            FreqAndDb[] averageFrequencyPlot = new FreqAndDb[613];
            averageFrequencyPlot = Tools.AverageProcess(frequencyPlot, frequencyPlot2, frequencyPlot3, frequencyPlot4);

            return averageFrequencyPlot;
        }
    }


    class Program
    {

        static void Main(string[] args)
        {

            //Create average chart from 4 charts
            FreqAndDb[] averageFrequencyPlot = new FreqAndDb[613];
            averageFrequencyPlot = Tools.AverageResult();


            //Create output information object
            NameAndDif[] differences = new NameAndDif[956];

            for (int i = 0; i < 956; i++)
            {
                differences[i] = new NameAndDif();

            }

            DirectoryInfo d = new DirectoryInfo(@"C:\Users\Marika\Documents\Visual Studio 2013\Projects\HeadphoneGraphAnalysis\data");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.csv"); //Getting Text files
            string str = "";

            int j = 0;

            foreach (FileInfo file in Files)
            {
                //Loop for analysing all the files
                str = file.FullName;
 
                differences[j].name = file.Name;

                var path = file.FullName;

                //Lets make the freq and db object ready
                FreqAndDb[] frequencyPlot = new FreqAndDb[613];
                Tools.Process(path, ref frequencyPlot);

                double difference = 0;
                difference = Tools.Compare(frequencyPlot, averageFrequencyPlot);
                differences[j].difference = difference;
                j++;
                Console.WriteLine(file.Name + "," + difference.ToString());
            }
            Console.ReadLine();
            string a = "wefawefa";


            //compare to target chart
            //double difference = 0;
            //difference = Tools.Compare(frequencyPlot, averageFrequencyPlot);



            





        }
    }
}
