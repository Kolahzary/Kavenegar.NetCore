namespace Kavenegar.Core.Models
{
    public class DateResult
    {
        public string DateTime { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public long UnixTime { get; set; }
    }
}