using System;
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
    }
}
