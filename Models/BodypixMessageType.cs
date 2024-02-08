namespace TimestampLogger.Models
{
    public class BodypixMessageType
    {
        public int TriggerTimestamp { get; set; }
        public int deviceId { get; set; }
        public int deviceType { get; set; }

        public int arucoId { get; set; }

        public int targetPersonIndex { get; set; }

        public int numOfPerson { get; set; }

        public int label { get; set; }

    }
}