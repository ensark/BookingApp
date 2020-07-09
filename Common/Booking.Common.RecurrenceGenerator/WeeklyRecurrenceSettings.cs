using System;
using Booking.Common.RecurrenceProcessor.Enums;

namespace Booking.Common.RecurrenceProcessor
{
    public class WeeklyRecurrenceSettings : RecurrenceSettings
    {
        public WeeklyRecurrenceSettings(DateTime startDate) : base(startDate) { }

        public WeeklyRecurrenceSettings(DateTime startDate, DateTime endDate) : base(startDate, endDate) { }

        public WeeklyRecurrenceSettings(DateTime startDate, int numberOfOccurrences) : base(startDate, numberOfOccurrences) { }

        public WeeklyRegenType RegenType { get; private set; } = WeeklyRegenType.OnEveryXWeeks;
        public SelectedDayOfWeekValues SelectedDays { get; set; }
        private int regenEveryXWeeks;
        private bool getNextDateValue;
        private DateTime nextDateValue;

        public RecurrenceValues GetValues(int regenEveryXWeeks, SelectedDayOfWeekValues selectedDays)
        {
            this.regenEveryXWeeks = regenEveryXWeeks;
            RegenType = WeeklyRegenType.OnEveryXWeeks;
            SelectedDays = selectedDays;

            return GetValues();
        }

        internal override RecurrenceValues GetValues()
        {
            return GetRecurrenceValues();
        }

        internal override RecurrenceValues GetValues(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
            EndDateType = EndDateType.SpecificDate;

            return GetRecurrenceValues();
        }

        internal override RecurrenceValues GetValues(DateTime startDate, int numberOfOccurrences)
        {
            NumberOfOccurrences = numberOfOccurrences;
            StartDate = startDate;
            EndDateType = EndDateType.NumberOfOccurrences;

            return GetRecurrenceValues();
        }

        private RecurrenceValues GetRecurrenceValues()
        {
            RecurrenceValues values = null;
            switch (RegenType)
            {
                case WeeklyRegenType.OnEveryXWeeks:
                    values = GetEveryXWeeksValues();
                    break;

            }
            if (values.Values.Count > 0)
            {
                values.SetStartDate(values.Values[0]);

                if (TypeOfEndDate != EndDateType.NoEndDate)
                    values.SetEndDate(values.Values[values.Values.Count - 1]);
            }

            return values;
        }

        internal override DateTime GetNextDate(DateTime currentDate)
        {
            getNextDateValue = true;
            nextDateValue = currentDate;

            RecurrenceValues values = GetValues();

            return values.EndDate;
        }

        private RecurrenceValues GetEveryXWeeksValues()
        {
            RecurrenceValues values = new RecurrenceValues();
            DateTime dt = base.StartDate.AddDays(-1);

            if (getNextDateValue)
            {
                do
                {
                    dt = GetNextDay(dt);
                    values.AddDateValue(dt);
                    if (values.Values[values.Values.Count - 1] > nextDateValue)
                        break;
                } while (dt <= nextDateValue.AddDays((regenEveryXWeeks * 7) + 7));
            }
            else
            {
                switch (base.TypeOfEndDate)
                {
                    case EndDateType.NoEndDate:
                        throw new Exception("The ability to create recurring dates with no End date is not currently available.");

                    case EndDateType.NumberOfOccurrences:

                        for (int i = 0; i < base.NumberOfOccurrences; i++)
                        {
                            dt = GetNextDay(dt);
                            values.AddDateValue(dt);
                        }
                        break;

                    case EndDateType.SpecificDate:
                        do
                        {
                            dt = GetNextDay(dt);

                            if (dt > base.EndDate)
                                break;

                            values.AddDateValue(dt);
                        } while (dt <= base.EndDate);
                        break;

                    default:
                        throw new ArgumentNullException("TypeOfEndDate", "The TypeOfEndDate property has not been set.");
                }
            }

            return values;
        }

        private DateTime GetNextDay(DateTime input)
        {
            DateTime? returnDate = null;

            do
            {
                input = input.AddDays(1);
                switch (input.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (SelectedDays.Sunday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Monday:
                        if (SelectedDays.Monday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Tuesday:
                        if (SelectedDays.Tuesday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Wednesday:
                        if (SelectedDays.Wednesday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Thursday:
                        if (SelectedDays.Thursday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Friday:
                        if (SelectedDays.Friday)
                            returnDate = input;
                        break;
                    case DayOfWeek.Saturday:
                        if (SelectedDays.Saturday)
                            returnDate = input;
                        else
                        {
                            if (regenEveryXWeeks > 1)
                                input = input.AddDays((regenEveryXWeeks - 1) * 7);
                        }
                        break;
                }
            } while (!returnDate.HasValue);

            return returnDate.Value;
        }
    }
}
