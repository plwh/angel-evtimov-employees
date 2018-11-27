namespace Solution.Models
{
    using System;

    public class Project
    {
        public Project(int projectId, DateTime dateFrom, DateTime dateTo)
        {
            this.ProjectId = projectId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
        }

        public int ProjectId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}
