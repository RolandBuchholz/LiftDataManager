namespace LiftDataManager.Helpers
{
    public class DateRange
    {
        public DateOnly Start { get; private set; }
        public DateOnly End { get; private set; }
        public DateOnly NullDate { get; private set; }

        public DateRange(DateOnly start, DateOnly end)
        {
            Start = start;
            End = end;
        }

        public bool WithInRange(DateOnly value)
        {
            if (Start == DateOnly.MinValue && End == DateOnly.MinValue)
                return true;
            if (Start == DateOnly.MinValue)
                return value <= End;
            if (End == DateOnly.MinValue)
                return Start <= value;
            return (Start <= value) && (value <= End);
        }

        public bool WithInRange(DateTime value)
        {
            if (Start == DateOnly.MinValue && End == DateOnly.MinValue)
                return true;

            var valueDateOnly = DateOnly.FromDateTime(value);

            if (valueDateOnly == DateOnly.MinValue)
                return true;
            if (Start == DateOnly.MinValue)
                return valueDateOnly <= End;
            if (End == DateOnly.MinValue)
                return Start <= valueDateOnly;
            return (Start <= valueDateOnly) && (valueDateOnly <= End);
        }
    }
}
