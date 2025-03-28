namespace ReadingService.Services
{
    public class TimeService
    {
        public DateTime GetTaiwanNow()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }
    }
}
