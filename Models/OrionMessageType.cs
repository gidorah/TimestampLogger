namespace TimestampLogger.Models
{
    public class OrionMessageType
    {
        public string id { get; set; }
        public string weaponTrigger { get; set; }
        public string vegaReceive { get; set; }
        public string vegaSend { get; set; }

        public string orionReceive { get; set; }
        public string ledToArduino { get; set; }

        public string vibrationToArduino { get; set; }
    }
}
