using System;
using System.Threading.Tasks;
using CoreNotes.gRPC.Server.Protos;
using Grpc.Core;
using Grpc.Net.Client;

namespace CoreNotes.gRPC.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var md = new Metadata
            {
                {"username", "god"},
                {"role", "administrator"}
            };

            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new EmployeeService.EmployeeServiceClient(channel);

            var response = await client.GetByNoAsync(new GetByNoRequest
            {
                No = 1994
            }, md);

            Console.WriteLine($"Response message: {response}");
            Console.WriteLine("Press any key to exit");

            Console.ReadKey();
        }
    }
}
