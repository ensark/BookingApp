using System;
using System.Collections.Generic;

namespace Booking.Common.RecurrenceProcessor
{
    public class RecurrenceValues
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DateTime> Values { get; }

        public RecurrenceValues()
        {
            Values = new List<DateTime>();
        }

        public DateTime LastDate
        {
            get
            {
                if (Values.Count > 0)
                    return Values[Values.Count - 1];
                else
                    return DateTime.MaxValue;
            }
        }

        internal void SetStartDate(DateTime startingDate)
        {
            StartDate = startingDate;
        }

        internal void SetEndDate(DateTime endingDate)
        {
            EndDate = endingDate;
        }

        internal void AddDateValue(DateTime recurrenceDate)
        {
            Values.Add(recurrenceDate);
        }

        internal void AddDateValue(DateTime recurrenceDate, int adjustedValue)
        {
            Values.Add(recurrenceDate.AddDays(adjustedValue));
        }
    }
}
