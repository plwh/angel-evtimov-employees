namespace Solution
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class Solution
    {
        static void Main()
        {
            /*
             * Put task sample data in "SampleData.txt" first, or just run program with already generated data
             */

            // SampleDataGenerator.GenerateSampleData(1500);

            var employeesData = new List<Employee>();

            var filePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\", "SampleData.txt");

            // Read input data

            using (var reader = new StreamReader(filePath))
            {
                var input = "";
                while ((input = reader.ReadLine()) != null)
                {
                    var tokens = input.Split(", ", StringSplitOptions.RemoveEmptyEntries);

                    var employeeId = int.Parse(tokens[0]);

                    var projectId = int.Parse(tokens[1]);

                    var dateFrom = DateTime.Parse(tokens[2], CultureInfo.InvariantCulture);

                    var dateTo = new DateTime();

                    if (!DateTime.TryParse(tokens[3], out dateTo))
                    {
                        dateTo = DateTime.Now;
                    }

                    var currentEmployee = employeesData.FirstOrDefault(e => e.EmpId == employeeId);

                    if (currentEmployee == null)
                    {
                        currentEmployee = new Employee(employeeId);
                        employeesData.Add(currentEmployee);
                    }

                    // Based on the three lines sample data in the task description it looks like one person can not
                    // work twice on the same project within different time periods so same project ids are omitted

                    var currentProject = currentEmployee.ProjectsWorkedOn.FirstOrDefault(e => e.ProjectId == projectId);

                    if (currentProject == null)
                    {
                        currentProject = new Project(projectId, dateFrom, dateTo);
                        currentEmployee.ProjectsWorkedOn.Add(currentProject);
                    }
                }
            }

            // Core logic
            var aggregatedData = new Dictionary<int, Dictionary<int, double>>();

            // Iterate employees data
            foreach (var employeeInfo in employeesData)
            {
                var currentEmployeeId = employeeInfo.EmpId;
                var currentEmployeeProjects = employeeInfo.ProjectsWorkedOn;

                // Get the id and projects of the rest of the employees
                var restOfEmployees = employeesData
                    .Where(e => e.EmpId != currentEmployeeId)
                    .Select(p => new
                    {
                        p.EmpId,
                        p.ProjectsWorkedOn
                    });

                // Iterate current employee projects
                foreach (var currentProject in currentEmployeeProjects)
                {
                    // Iterate rest of employees
                    foreach (var employee in restOfEmployees)
                    {
                        var nextEmployeeId = employee.EmpId;
                        var nextEmployeeProjects = employee.ProjectsWorkedOn;

                        // Iterate next employee projects and get project matching the id of the current project, if any
                        var matchingProject = nextEmployeeProjects.Where(p => p.ProjectId == currentProject.ProjectId).FirstOrDefault();

                        if (matchingProject != null)
                        {
                            var firstProjectDateFrom = currentProject.DateFrom;
                            var firstProjectDateTo = currentProject.DateTo;

                            var secondProjectDateFrom = matchingProject.DateFrom;
                            var secondProjectDateTo = matchingProject.DateTo;

                            // If have worked at the same time (check if dates overlap)
                            if (firstProjectDateFrom < secondProjectDateTo && secondProjectDateFrom < firstProjectDateTo)
                            {
                                // Find overlapping days
                                double overlappingDays = GetOverlappingDays(firstProjectDateFrom, firstProjectDateTo, secondProjectDateFrom, secondProjectDateTo);

                                // Add current employee to aggregated data dictionary if not present
                                if (!aggregatedData.ContainsKey(currentEmployeeId))
                                {
                                    aggregatedData[currentEmployeeId] = new Dictionary<int, double>();
                                }

                                // Add next employee to inner dictionary if not present
                                if (!aggregatedData[currentEmployeeId].ContainsKey(nextEmployeeId))
                                {
                                    aggregatedData[currentEmployeeId].Add(nextEmployeeId, 0);
                                }

                                aggregatedData[currentEmployeeId][nextEmployeeId] += overlappingDays;
                            }
                        }
                    }
                }
            }

            var firstTargetEmployeeId = 0;
            var secondTargetEmployeeId = 0;
            double maxDays = 0;

            // Iterate aggregated data dictionary
            foreach (var data in aggregatedData)
            {
                // We take the inner dict entry (id and worked days) of the colleague who has worked most with the current person
                var topColleagueInfo = data.Value.OrderByDescending(e => e.Value).First();

                var currentEmployeeId = data.Key;
                var topColleagueId = topColleagueInfo.Key;
                var workedDays = topColleagueInfo.Value;

                if (workedDays > maxDays)
                {
                    firstTargetEmployeeId = currentEmployeeId;
                    secondTargetEmployeeId = topColleagueId;
                    maxDays = workedDays;
                }
            }

            // Print result

            PrintResult(firstTargetEmployeeId, secondTargetEmployeeId, maxDays);
        }

        private static void PrintResult(int firstEmpId, int secondEmpId, double maxDays)
        {
            if (maxDays > 0)
            {
                var roundedResult = Math.Round(maxDays);
                var printDayOrDays = (roundedResult == 1) ? $"{roundedResult} day" : $"{roundedResult} days";
                Console.WriteLine($"Pair of employees, who have worked on the same projects together \n" +
                                  $"the longest(in days): {firstEmpId} {secondEmpId} {printDayOrDays}");
            }
            else
            {
                Console.WriteLine("No such pair.");
            }
        }

        private static double GetOverlappingDays(DateTime firstStart, DateTime firstEnd, DateTime secondStart, DateTime secondEnd)
        {
            var maxStart = firstStart > secondStart ? firstStart : secondStart;
            var minEnd = firstEnd < secondEnd ? firstEnd : secondEnd;
            var interval = minEnd - maxStart;
            var returnValue = interval > TimeSpan.FromSeconds(0) ? interval.TotalDays : 0;
            return returnValue;
        }
    }
}
