using Microsoft.Azure.Devices.Client;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IOTTrainingProject_2
{
    class Program
    {

        static async Task Main(string[] args)
        {
            //await TraverseMessages.Initiallize();
            await FileUpload.UploadFile();
        }
       

    }
}
