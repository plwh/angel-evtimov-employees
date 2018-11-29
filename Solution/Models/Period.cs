using System;

namespace Solution.Models
{
    public class Period
    {
        public Period(DateTime dateFrom, DateTime dateTo)
        {
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
        }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}
