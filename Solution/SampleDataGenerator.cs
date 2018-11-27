namespace Solution
{
    using System;
    using System.Globalization;
    using System.IO;

    public class SampleDataGenerator
    {
        public static void GenerateSampleData(int numOfRecords)
        {
            var rnd = new Random();
            var outputFilePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\", "SampleData.txt");
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                for (var i = 0; i < numOfRecords; i++)
                {
                    // Generate employee id
                    var employeeId = rnd.Next(1, 300);

                    // Generate project id
                    var projectId = rnd.Next(1, 20);

                    // Generate first date
                    var firstDate = GetRandomDate("2009-01-01", "2013-11-01");

                    // Generate second date based on first. It can also be NULL as well

                    var dateNow = DateTime.Now.ToShortDateString();
                    var secondDateBasedOnFirst = GetRandomDate(firstDate, dateNow);

                    string[] options = new string[] { secondDateBasedOnFirst, "NULL" };

                    var secondDate = options[rnd.Next(0, 2)];

                    // Make sure that we don't write an extra line at the end
                    if (i == numOfRecords - 1)
                    {
                        writer.Write($"{employeeId}, {projectId}, {firstDate}, {secondDate}");
                        break;
                    }

                    writer.WriteLine($"{employeeId}, {projectId}, {firstDate}, {secondDate}");
                }
            }
        }

        public static string GetRandomDate(string startDate, string endDate)
        {
            var gen = new Random();
            var start = DateTime.Parse(startDate, CultureInfo.InvariantCulture);
            var end = DateTime.Parse(endDate, CultureInfo.InvariantCulture);
            var range = (end - start).Days;
            var result = start.AddDays(gen.Next(range));

            return result.ToShortDateString();
        }
    }
}
