using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace IOTTrainingProject
{
	class Program
	{

		static string IoTHubConnectionString = "";
		static string DeviceConnectionString = "";
		static string DeviceID = "";
		static string desiredProperty = "";
		static string reportedProperty = "";
		static DeviceClient Client = null;

		static int desiredPropertyValue = 30;
		static int ReportedPropertyValue = 0;

		static async Task Main(string[] args)
		{
			Console.WriteLine("***********************************************");
			Console.WriteLine("Welcome to the Azure IoT Hub Device Twin Tester");
			Console.WriteLine();
			Console.WriteLine("Author: Pete Gallagher");
			Console.WriteLine("Twitter: @pete_codes");
			Console.WriteLine("Date: 16th November 2020");
			Console.WriteLine();
			Console.WriteLine("***********************************************");
			Console.WriteLine();

			try
			{

				Console.WriteLine("Enter the IoT Hub Connection String");
				IoTHubConnectionString = "HostName=iothubdemomegha.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=wrJbOYxSDvN1eevbrulcwJ6Qd+OWMg9FGJZDVif9Lm0=";

				Console.WriteLine("Enter the Device Connection String");
				DeviceConnectionString = "HostName=iothubdemomegha.azure-devices.net;DeviceId=device-02;SharedAccessKey=UL2otj689VYuZedik4WFLhhLzXhygs1ECrMvZvXKkhM=";

				Console.WriteLine("Enter the Device ID");
				DeviceID = "device-02";

				Console.WriteLine("Enter the Desired Property Key");
				desiredProperty = "tempLevel";

				Console.WriteLine("Enter the Reported Property Key");
				reportedProperty = "coolerOn";

				InitClient();

				GetDeviceTwinAsync().Wait();
				Client.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, null).Wait();

				var reportedPropertyValueString = "";

				while (true)
				{
					Console.WriteLine($"Set value for {reportedProperty} Reported Property (Enter to exit)");
					reportedPropertyValueString = Console.ReadLine();

					if (reportedPropertyValueString != null && reportedPropertyValueString != "")
					{
						ReportedPropertyValue = Int32.Parse(reportedPropertyValueString);
						await ReportProperty();
					}
					else
					{
						return;
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("Error in sample: {0}", ex.Message);
			}

		}

		private static async Task GetDeviceTwinAsync()
		{
			RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IoTHubConnectionString);

			try
			{
				var twin = await registryManager.GetTwinAsync(DeviceID);
				Console.WriteLine(twin.ToJson());
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : {0}", ex);
			}

		}

		public static async void InitClient()
		{
			try
			{
				Console.WriteLine("Connecting to hub");
				Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
				Console.WriteLine("Retrieving twin");
				await Client.GetTwinAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("Error in sample: {0}", ex.Message);
			}
		}

		public static async Task ReportProperty()
		{
			try
			{
				Console.WriteLine($"Sending reported property {reportedProperty} value");

				TwinCollection reportedProperties;
				reportedProperties = new TwinCollection();
				reportedProperties["coolerOn"] = ReportedPropertyValue;
				await Client.UpdateReportedPropertiesAsync(reportedProperties);
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("Errors: {0}", ex.Message);
			}
		}

		private static async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
		{
			try
			{
				Console.WriteLine("Desired property change:");
				Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));
				desiredPropertyValue = desiredProperties[desiredProperty];
				Console.WriteLine($"Desired Property {desiredProperty} is now: {desiredPropertyValue}");
			}
			catch (AggregateException ex)
			{
				foreach (Exception exception in ex.InnerExceptions)
				{
					Console.WriteLine();
					Console.WriteLine("Error in sample: {0}", exception);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("Error in sample: {0}", ex.Message);
			}
		}

	}
}