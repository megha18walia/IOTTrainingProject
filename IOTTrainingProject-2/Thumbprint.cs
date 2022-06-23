using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IOTTrainingProject_2
{
    public class Thumbprint
    {
        public async Task AuthenticateCertificate()
        {
            var certificate = new X509Certificate2(@"primary.pfx", "password@1");
            var authentication = new DeviceAuthenticationWithX509Certificate("device-04", certificate);

            DeviceClient Client = DeviceClient.Create("iothubdemomegha.azure-devices.net",
                                                        authentication,
                                                        Microsoft.Azure.Devices.Client.TransportType.Mqtt);

            while (true)
            {
                Console.WriteLine("Enter Message to Send (Empty Message to exit)");
                var messageToSend = Console.ReadLine();

                Message message = new Message(Encoding.ASCII.GetBytes(messageToSend));
                Console.WriteLine("Sending Message {0}", messageToSend);
                await Client.SendEventAsync(message);
            }
        }
    }
}
