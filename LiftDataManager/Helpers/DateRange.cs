﻿namespace LiftDataManager.Helpers
{
    public class DateRange
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public DateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public bool WithInRange(DateTime value)
        {
            if (Start == DateTime.MinValue && End == DateTime.MinValue)
                return true;
            if (value == DateTime.MinValue)
                return true;
            if (Start == DateTime.MinValue)
                return value <= End;
            if (End == DateTime.MinValue)
                return Start <= value;
            return (Start <= value) && (value <= End);
        }
    }
}
