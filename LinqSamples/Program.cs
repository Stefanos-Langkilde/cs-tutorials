using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Features.Linq;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {

            var developers = new Employee[]
            {
                new Employee { Id = 1, Name = "Scott" },
                new Employee { Id = 2, Name = "Chris" }
            };

            var sales = new List<Employee>()
            {
                new Employee { Id = 3, Name = "Alex" }
            };

            var query = developers.Where(e => e.Name.Length == 5)
                .OrderBy(e => e.Name);

            foreach (var employee in query)
            {
                Console.WriteLine(employee.Name);
            }
            static bool NameStartsWithS(Employee employee)
            {
                return employee.Name.StartsWith("S");
            }
        }
    }
}

