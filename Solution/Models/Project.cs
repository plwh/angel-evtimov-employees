namespace Solution.Models
{
    using System;
    using System.Collections.Generic;

    public class Project
    {
        public Project(int projectId)
        {
            this.ProjectId = projectId;
            this.PeriodsWorkedOn = new List<Period>();
        }

        public int ProjectId { get; set; }

        public List<Period> PeriodsWorkedOn { get; set; }
    }
}
