namespace Solution.Models
{
    using System.Collections.Generic;

    public class Employee
    {
        public Employee(int empId)
        {
            this.EmpId = empId;
            this.ProjectsWorkedOn = new List<Project>();
        }

        public int EmpId { get; set; }

        public List<Project> ProjectsWorkedOn { get; set; }
    }
}
