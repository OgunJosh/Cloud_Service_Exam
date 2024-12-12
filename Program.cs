using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace ElevatorSimulator
{
    class Program
    {
        // Replace with your IoT Hub device connection string
        private const string DeviceConnectionString = "HostName=MCTIoTHub1.azure-devices.net;DeviceId=pcjosh;SharedAccessKey=2P5zhIT+YvtvtyvHhs28Ol6BnRQLqkjy2Q5xf+svGlU=";
        private static DeviceClient? deviceClient;

        static async Task Main(string[] args)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);

            Console.WriteLine("Elevator Simulator");
            Console.WriteLine("==================");

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Send Trip Message");
                Console.WriteLine("2. Send Maintenance Message");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await SendTripMessage();
                        break;
                    case "2":
                        await SendMaintenanceMessage();
                        break;
                    case "3":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static async Task SendTripMessage()
        {
            Console.Write("Enter Elevator Name: ");
            string elevatorName = Console.ReadLine();

            Console.Write("Enter Number of People: ");
            int numberOfPeople = int.Parse(Console.ReadLine());

            Console.Write("Enter Departure Floor: ");
            int departureFloor = int.Parse(Console.ReadLine());

            Console.Write("Enter Destination Floor: ");
            int destinationFloor = int.Parse(Console.ReadLine());

            var messageData = new
            {
                ElevatorName = elevatorName,
                Timestamp = DateTime.UtcNow,
                NumberOfPeople = numberOfPeople,
                DepartureFloor = departureFloor,
                DestinationFloor = destinationFloor,
                IsMaintenance = false
            };

            await SendMessageToIoTHub(messageData);
            Console.WriteLine("Trip message sent to IoT Hub.");
        }

        private static async Task SendMaintenanceMessage()
        {
            Console.Write("Enter Elevator Name: ");
            string elevatorName = Console.ReadLine();

            Console.WriteLine("Choose Maintenance Type:");
            Console.WriteLine("1. Monthly check");
            Console.WriteLine("2. Yearly check");
            Console.WriteLine("3. Repair");
            Console.WriteLine("4. Broken");
            Console.Write("Select an option: ");
            string maintenanceType = Console.ReadLine() switch
            {
                "1" => "Monthly check",
                "2" => "Yearly check",
                "3" => "Repair",
                "4" => "Broken",
                _ => "Unknown"
            };

            var messageData = new
            {
                ElevatorName = elevatorName,
                Timestamp = DateTime.UtcNow,
                IsMaintenance = true,
                MaintenanceType = maintenanceType
            };

            await SendMessageToIoTHub(messageData);
            Console.WriteLine("Maintenance message sent to IoT Hub.");
        }

        private static async Task SendMessageToIoTHub(object messageData)
        {
            string messageJson = JsonConvert.SerializeObject(messageData);
            Message message = new Message(Encoding.UTF8.GetBytes(messageJson));
            await deviceClient.SendEventAsync(message);
        }
    }
}
