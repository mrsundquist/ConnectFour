using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ConnectFour
{
    public partial class Board
    {
        static StorageFolder folder;
        static StreamWriter outputStream;
        static StreamReader inputStream;
        static Stream output;
        static Stream input;


        private async void getStreams()
        {
            
            Board.output = await Board.folder.OpenStreamForWriteAsync("ConnectFourData.txt", CreationCollisionOption.OpenIfExists);
            Board.input = await Board.folder.OpenStreamForReadAsync("ConnectFourData.txt");
        }

        public void Write(string[] theData)
        {
            foreach (string record in theData)
            {
                Board.outputStream.Write(record);
                Board.outputStream.WriteLine();
            }

            //outputStream.Dispose();
            //output.Dispose();
        }

        private void Read(Dictionary<string, stateData> historicalData)
        {
            string dataLine;
            string[] dataParts = new string[3];
            string state;
            stateData record = new stateData();

            while ((dataLine = Board.inputStream.ReadLine()) != null)
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
            //inputStream.Dispose();
            //input.Dispose();
        }
    }
}
