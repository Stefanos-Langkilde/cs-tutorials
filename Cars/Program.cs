using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace Cars
{
     class Program
     {
          static void Main(string[] args)
          {
               var manufacturers = ProcessManufacturers("manufacturers.csv");
               CreateXml();
               QueryXml();



               /*
               var query =
                    from manufacturer in manufacturers
                    join car in cars on manufacturer.Name equals car.Manufacturer
                         into carGroup
                    orderby manufacturer.Name
                    select new
                    {
                         Manufacturer = manufacturer,
                         Cars = carGroup
                    } into result
                    group result by result.Manufacturer.Headquarters;

               var query2 =
                    cars.GroupBy(c => c.Manufacturer)
                              .Select(g =>
                              {
                                   var results = g.Aggregate(new CarStatistics(),
                                             (acc, c) => acc.Accumulate(c),
                                             acc => acc.Compute());
                                   return new
                                   {
                                        Name = g.Key,
                                        Avg = results.Average,
                                        Min = results.Min,
                                        Max = results.Max,
                                   };
                              })
                              .OrderByDescending(r => r.Max);

               foreach (var result in query2)
               {
                    Console.WriteLine($"{result.Name}");
                    Console.WriteLine($"\t Max: {result.Max}");
                    Console.WriteLine($"\t Min: {result.Min}");
                    Console.WriteLine($"\t Avg: {result.Avg}");
               }*/


               /*
               var query =
                    from car in cars
                    join manufacturer in manufacturers on new { car.Manufacturer, car.Year }
                         equals 
                         new { Manufacturer = manufacturer.Name, manufacturer.Year }
                    orderby car.Combined descending, car.Name ascending
                    select new
                    {
                         manufacturer.Headquarters,
                         car.Name,
                         car.Combined
                    };

               var query2 =
                    cars.Join(manufacturers,
                              c => new { c.Manufacturer, c.Year },
                              m => new { Manufacturer = m.Name, m.Year }, 
                              (c, m) => new
                              {
                                   m.Headquarters,
                                   c.Name,
                                   c.Combined
                              })
                    .OrderByDescending(c => c.Combined)
                    .ThenBy(c => c.Name);

                var top =
                    cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
                         .OrderByDescending(c => c.Combined)
                         .ThenByDescending(c => c.Name)
                         .Select(c => c)
                         .First();

               var result = cars.Any(cars => cars.Manufacturer == "Ford");

               Console.WriteLine(top.Name); 

               foreach (var car in query2.Take(10))
               {
                    Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
               }
               */
          }

          private static void QueryXml()
          {
               var document = XDocument.Load("fuel.xml");

               var query = 
                    from element in document.Element("Cars")?.Elements("Car")
                    where element.Attribute("Manufacturer")?.Value == "BMW"
                    select element.Attribute("Name")?.Value;
               foreach (var name in query)
               {
                    Console.WriteLine(name);
               }
          }

          private static void CreateXml()
          {
               var records = ProcessFile("Fuel.csv");
               
               var document = new XDocument();
               var cars = new XElement("Cars",

                         from record in records
                         select new XElement("Car",
                                                  new XAttribute("Name", record.Name),
                                                  new XAttribute("Combined", record.Combined),
                                                  new XAttribute("Manufacturer", record.Manufacturer))
               );

               document.Add(cars);
               document.Save("fuel.xml");
          }

          private static List<Car> ProcessFile(string path)
          {
               var query =

                  File.ReadAllLines(path)
                       .Skip(1)
                       .Where(l => l.Length > 1)
                       .ToCar();

               return query.ToList();
          }

          private static List<Manufacturer> ProcessManufacturers(string path)
          {
               var query =
                    File.ReadAllLines(path)
                          .Where(l => l.Length == 1)
                          .Select(l =>
                          {
                               var columns = l.Split(',');
                               return new Manufacturer
                               {
                                    Name = columns[0],
                                    Headquarters = columns[1],
                                    Year = int.Parse(columns[2]),
                               };
                          });
               return query.ToList();
          }
     }

     public class CarStatistics
     {
          public CarStatistics()
          {
               Max = Int32.MinValue;
               Min = Int32.MaxValue;
          }
          public CarStatistics Accumulate(Car car)
          {
               Count += 1;
               Total += car.Combined;
               Max = Math.Max(Max, car.Combined);
               Min = Math.Min(Min, car.Combined);

               return this;
          }

          public CarStatistics Compute()
          {
               Average = Total / Count;
               return this;
          }

          public int Max { get; set; }
          public int Min { get; set; }
          public int Total { get; set; }
          public int Count { get; set; }
          public double Average { get; set; }
     }

     public static class CarExtensions
     {
          public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
          {

               foreach (var line in source)
               {
                    var columns = line.Split(',');

                    yield return new Car
                    {
                         Year = int.Parse(columns[0]),
                         Manufacturer = columns[1],
                         Name = columns[2],
                         Displacement = double.Parse(columns[3]),
                         Cylinders = int.Parse(columns[4]),
                         City = int.Parse(columns[5]),
                         Highway = int.Parse(columns[6]),
                         Combined = int.Parse(columns[7]),
                    };
               }
          }
     }


}
