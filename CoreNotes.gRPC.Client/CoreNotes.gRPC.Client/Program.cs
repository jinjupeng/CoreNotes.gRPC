using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CoreNotes.gRPC.Server.Protos;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Serilog;

namespace CoreNotes.gRPC.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            // 在根目录下打开命令行，输入：dotnet run 1/2/3/5，即可运行
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new EmployeeService.EmployeeServiceClient(channel);

            var option = int.Parse(args[0]);
            switch (option)
            {
                case 1:
                    await GetByNoAsync(client);
                    break;
                case 2:
                    await GetAllAsync(client);
                    break;
                case 3:
                    await AddPhotoAsync(client);
                    break;
                case 5:
                    await SaveAllAsync(client);
                    break;
            }

            
            Console.WriteLine("Press any key to exit");

            Console.ReadKey();

            Log.CloseAndFlush();
        }

        public static async Task GetByNoAsync(EmployeeService.EmployeeServiceClient client)
        {
            var md = new Metadata
            {
                {"username", "god"},
                {"role", "administrator"}
            };
            var response = await client.GetByNoAsync(new GetByNoRequest
            {
                No = 1994
            }, md);

            Console.WriteLine($"Response message: {response}");
        }

        public static async Task GetAllAsync(EmployeeService.EmployeeServiceClient client)
        {
            using var call = client.GetAll(new GetAllRequest());
            var responseStream = call.ResponseStream;
            while (await responseStream.MoveNext())
            {
                Console.WriteLine(responseStream.Current.Employee);
            }
        }

        public static async Task AddPhotoAsync(EmployeeService.EmployeeServiceClient client)
        {
            var md = new Metadata
            {
                {"username", "god"},
                {"role", "administrator"}
            };

            FileStream fs = File.OpenRead("logo.png");
            using var call = client.AddPtoto(md);

            var stream = call.RequestStream;

            while (true)
            {
                byte[] buffer = new byte[1024];
                int numRead = await fs.ReadAsync(buffer, 0, buffer.Length);
                if (numRead == 0)
                {
                    break;
                }

                if (numRead < buffer.Length)
                {
                    Array.Resize(ref buffer, numRead);
                }

                await stream.WriteAsync(new AddPhotoRequest()
                {
                    Data = ByteString.CopyFrom(buffer)
                });
            }

            await stream.CompleteAsync();

            var response = await call.ResponseAsync;
            Console.WriteLine(response.IsOk);
        }

        public static async Task SaveAllAsync(EmployeeService.EmployeeServiceClient client)
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    No = 111,
                    FirstName = "Monica",
                    LastName = "Geller",
                    Salary = 7890.1f
                },
                new Employee
                {
                    No = 222,
                    FirstName = "Joey",
                    LastName = "Tri",
                    Salary = 500
                }
            };

            using var call = client.SaveAll();
            var requestStream = call.RequestStream;
            var responseStream = call.ResponseStream;

            var responseTask = Task.Run(async () =>
            {
                while (await responseStream.MoveNext())
                {
                    Console.WriteLine($"Saved: {responseStream.Current.Employee}");
                }
            });

            foreach (var employee in employees)
            {
                await requestStream.WriteAsync(new EmployeeRequest
                {
                    Employee = employee
                });
            }

            await requestStream.CompleteAsync();
            await responseTask;

        }
    }
}
