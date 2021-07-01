
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IOTTrainingProject_3
{
    class Program
    {
        // TODO: set your DPS info here:
        private const string dpsGlobalDeviceEndpoint = "global.azure-devices-provisioning.net";
        private const string dpsIdScope = "0ne002F095E";

        // TODO: set the keys for the symmetric key enrollment group here:
        private const string enrollmentGroupPrimaryKey = "/LwFGOVJJ2xfp3uJyv2DZ2ugAsDwgBZUB1UB6WeyKxJ5cjJty0cnnx4MbCqhYLS2iWtwVMIPrm5lx5DTlzczwg==";
        private const string enrollmentGroupSecondaryKey = "gVNBcdltNCZxcQ1m2JG9KQ7XnXFqobvxXytYgjB2Co4cOEP0dz2ePAugKV3Rk5zvEEZJZZKI/dgMEfA4oC5FCA==";

        private static readonly string[] deviceUsages = new[] { "car", "building", "plane" };
        private static readonly Random random = new Random();

        static async Task Main(string[] args)
        {
            Console.WriteLine("*** Press ENTER to start device registrations ***");
            Console.ReadLine();

            await RegisterDevices(10);

            Console.WriteLine("*** Press ENTER to quit ***");
            Console.ReadLine();
        }

        private static async Task RegisterDevices(int deviceCount)
        {
            // register given number of devices
            for (int i = 1; i <= deviceCount; i++)
            {
                var deviceUsage = deviceUsages[random.Next(0, deviceUsages.Length)];

                var deviceRegistrationId = $"{deviceUsage}-device-{System.Guid.NewGuid().ToString().ToLower()}";

                Console.WriteLine($"Will register device {i}/{deviceCount}: {deviceRegistrationId}...");

                await RegisterDevice(deviceRegistrationId);
            }
        }

        private static async Task RegisterDevice(string deviceRegistrationId)
        {
            // using symmetric keys
            using var securityProvider = new SecurityProviderSymmetricKey(
              registrationId: deviceRegistrationId,
              primaryKey: ComputeKeyHash(enrollmentGroupPrimaryKey, deviceRegistrationId),
              secondaryKey: ComputeKeyHash(enrollmentGroupSecondaryKey, deviceRegistrationId));

            // Amqp transport
            using var transportHandler = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly);

            // set up provisioning client for given device
            var provisioningDeviceClient = ProvisioningDeviceClient.Create(
              globalDeviceEndpoint: dpsGlobalDeviceEndpoint,
              idScope: dpsIdScope,
              securityProvider: securityProvider,
              transport: transportHandler);

            // register device
            var deviceRegistrationResult = await provisioningDeviceClient.RegisterAsync();

            Console.WriteLine($"   Device registration result: {deviceRegistrationResult.Status}");
            if (!string.IsNullOrEmpty(deviceRegistrationResult.AssignedHub))
            {
                Console.WriteLine($"   Assigned to hub '{deviceRegistrationResult.AssignedHub}'");
            }
            Console.WriteLine();
        }

        private static string ComputeKeyHash(string enrollmentKey, string deviceId)
        {
          //  using var hmac = new HMACSHA256(Convert.FromBase64String(key));

            using var hmac = new HMACSHA256(Convert.FromBase64String(enrollmentKey));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(deviceId)));
        }
    }
}
