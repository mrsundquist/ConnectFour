using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ConnectFour
{
    class Writer
    {
        static StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
        static Stream output;
        static Stream input;
        static StreamWriter outputStream;
        static StreamReader inputStream;

        static public async void Write(string[] theData)
        {
            Writer.output = await Writer.folder.OpenStreamForWriteAsync("ConnectFourData.txt", CreationCollisionOption.OpenIfExists);
            Writer.outputStream = new StreamWriter(output);

            foreach (string record in theData)
            {
                Writer.outputStream.Write(record);
                Writer.outputStream.WriteLine();
            }

            //Writer.outputStream.Dispose();  // causes input string in not correct format
            Writer.output.Dispose();
        }

        static public async void Read(Dictionary<string, stateData> historicalData)
        {
            string dataLine;
            string[] dataParts = new string[3];
            string state;
            stateData record = new stateData();

            Writer.input = await Writer.folder.OpenStreamForReadAsync("ConnectFourData.txt");
            Writer.inputStream = new StreamReader(input);

            while ((dataLine = inputStream.ReadLine()) != null)
            {
                dataParts = dataLine.Split(' ');
                state = dataParts[0];
                record.numPlays = Convert.ToInt32(dataParts[1]);
                record.numWins = Convert.ToInt32(dataParts[2]);

                if (historicalData.ContainsKey(dataParts[0]))
                {
                    record.numPlays += historicalData[state].numPlays;
                    record.numWins += historicalData[state].numWins;
                }
                historicalData[state] = record;
            }
            
            inputStream.Dispose();
            input.Dispose();
        }
    }
}