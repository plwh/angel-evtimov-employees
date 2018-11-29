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
            // SampleDataGenerator.GenerateSampleData(1500);

            var employeesData = new List<Employee>();

            Console.WriteLine("Please enter file path (including file name) to file with sample\ndata, or just press Enter to run program with generated data file:");

            var filePath = Console.ReadLine();

            if (filePath == "")
            {
                filePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\", "SampleData.txt");
            }

            // Read input data

            StreamReader reader;

            try
            {
                reader = new StreamReader(filePath);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found. Please restart program and try again.");
                return;
            }

            using (reader)
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

                    // Initialize new period containg the two dates
                    var period = new Period(dateFrom, dateTo);

                    var currentEmployee = employeesData.FirstOrDefault(e => e.EmpId == employeeId);

                    if (currentEmployee == null)
                    {
                        currentEmployee = new Employee(employeeId);
                        employeesData.Add(currentEmployee);
                    }

                    var currentProject = currentEmployee.ProjectsWorkedOn.FirstOrDefault(e => e.ProjectId == projectId);

                    if (currentProject == null)
                    {
                        currentProject = new Project(projectId);
                        currentProject.PeriodsWorkedOn.Add(period);

                        currentEmployee.ProjectsWorkedOn.Add(currentProject);
                    }
                    else
                    {
                        currentProject.PeriodsWorkedOn.Add(period);
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
                            // Get project working periods of current employee project

                            var currentProjectPeriods = currentProject.PeriodsWorkedOn;

                            // Get project working periods of next employee matching project

                            var matchingProjectPeriods = matchingProject.PeriodsWorkedOn;

                            foreach (var currentPeriod in currentProjectPeriods)
                            {
                                var currentPeriodDateFrom = currentPeriod.DateFrom;
                                var currentPeriodDateTo = currentPeriod.DateTo;

                                foreach (var nextPeriod in matchingProjectPeriods)
                                {
                                    var nextPeriodDateFrom = nextPeriod.DateFrom;
                                    var nextPeriodDateTo = nextPeriod.DateTo;

                                    // If have worked at the same time (check if dates overlap)
                                    if (currentPeriodDateFrom < nextPeriodDateTo && nextPeriodDateFrom < currentPeriodDateTo)
                                    {
                                        // Find overlapping days
                                        double overlappingDays = GetOverlappingDays(currentPeriodDateFrom, currentPeriodDateTo, nextPeriodDateFrom, nextPeriodDateTo);

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
                Console.WriteLine($"Pair of employees, who have worked on the same projects together\n" +
                                  $"(including multiple time periods) the longest(in days): {firstEmpId} {secondEmpId} {printDayOrDays}");
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
