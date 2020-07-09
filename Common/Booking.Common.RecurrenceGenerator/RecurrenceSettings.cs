using System;
using Booking.Common.RecurrenceProcessor.Enums;

namespace Booking.Common.RecurrenceProcessor
{
    public abstract class RecurrenceSettings
    {
        public RecurrenceSettings(DateTime startDate)
        {
            StartDate = startDate;
            EndDateType = EndDateType.NoEndDate;
        }

        public RecurrenceSettings(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            this.endDate = endDate;
            EndDateType = EndDateType.SpecificDate;
        }

        public RecurrenceSettings(DateTime startDate, int numberOfOccurrences)
        {
            StartDate = startDate;
            NumberOfOccurrences = numberOfOccurrences;
            EndDateType = EndDateType.NumberOfOccurrences;
        }

        protected EndDateType EndDateType = EndDateType.NotDefined;

        internal abstract DateTime GetNextDate(DateTime currentDate);
        internal abstract RecurrenceValues GetValues();
        internal abstract RecurrenceValues GetValues(DateTime startDate, int numberOfOccurrences);
        internal abstract RecurrenceValues GetValues(DateTime startDate, DateTime endDate);

        DateTime? endDate; // Nullable date because there may or may not be an end date.
        public DateTime StartDate { get; set; }
        public int RecurrenceInterval { get; set; } = 1;
        public int RegenerationAfterCompletedInterval { get; set; } = 0;
        public int NumberOfOccurrences { get; set; } = 0;

        public EndDateType TypeOfEndDate
        {
            get
            {
                return EndDateType;
            }
            set
            {
                EndDateType = value;
            }
        }

        public bool HasEndDate
        {
            get
            {
                return endDate.HasValue;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                if (endDate.HasValue)
                    return endDate.Value;
                else
                    return null;
            }
            set
            {
                endDate = value;
            }
        }
    }
}
