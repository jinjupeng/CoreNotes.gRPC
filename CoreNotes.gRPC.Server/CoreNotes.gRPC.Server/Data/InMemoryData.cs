using System.Collections.Generic;
using CoreNotes.gRPC.Server.Protos;

namespace CoreNotes.gRPC.Server.Data
{
    public class InMemoryData
    {
        public static List<Employee> Employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                No = 1994,
                FirstName = "Chandler",
                LastName = "Bing",
                Salary = 2200
            },
            new Employee
            {
                Id = 2,
                No = 1999,
                FirstName = "Rachel",
                LastName = "Green",
                Salary = 2400
            },
            new Employee
            {
                Id = 3,
                No = 1996,
                FirstName = "hello",
                LastName = "world",
                Salary = 3000
            }
        };

    }
}