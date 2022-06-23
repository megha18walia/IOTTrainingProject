using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IOTTrainingProject_2
{
     class TraverseMessages
    {
         string DeviceConnectionString = "";
         RegistryManager Client = null;
        Channel messageToSend = new Channel();

        public  async Task Initiallize()
        {
            Console.WriteLine("****************************************************");
            Console.WriteLine("Welcome to the Azure IoT Hub Device Messaging Tester");
            Console.WriteLine();
            Console.WriteLine("Author: Pete Gallagher");
            Console.WriteLine("Twitter: @pete_codes");
            Console.WriteLine("Date: 19th December 2020");
            Console.WriteLine();
            Console.WriteLine("****************************************************");
            Console.WriteLine();

            try
            {
                Console.WriteLine("Enter the Device Connection String");
                DeviceConnectionString = "HostName=iothubdemomegha.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=qPZAc7EaH7mXczCsoBFD0b5dJccxPuli4wzj6gyyFv0=";

                InitClient();

                SendDeviceToCloudMessageAsync();
               

            }
            catch (System.Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        private void SendDeviceToCloudMessageAsync()
        {

            try
            {
                Task t = Task.Run(async () =>
                {
                    var twin = await Client.GetTwinAsync("Airtel_109088");
                    int channel = twin.Properties.Reported["channel"];
                    if (channel == 111)
                    {
                        twin.Properties.Desired["channel_status"] = "Off";
                    }
                    else
                    {
                        twin.Properties.Desired["channel_status"] = "On";
                    }
                    await Client.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);


                });
                Task.WaitAll(t);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

                    Thread.Sleep(2000);
                }

        public  void InitClient()
        {
            try
            {
                Console.WriteLine("Connecting to hub");
                Client = RegistryManager.CreateFromConnectionString(DeviceConnectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

       

       
    }
}
