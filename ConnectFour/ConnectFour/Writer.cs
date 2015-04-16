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
                
        static public async void Write(string[] theData)
        {
            StorageFile outputFile = await
             folder.CreateFileAsync("ConnectFourData.txt", CreationCollisionOption.OpenIfExists);

           await Windows.Storage.FileIO.AppendLinesAsync(outputFile, theData);
        }

        static public async void Read(Dictionary<string, stateData> historicalData)
        {
            string dataLine;
            string[] dataParts = new string[3];
            string state;
            stateData record = new stateData();

            System.IO.Stream input = await folder.OpenStreamForReadAsync("ConnectFourData.txt");
            System.IO.StreamReader inputStream = new StreamReader(input);

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
