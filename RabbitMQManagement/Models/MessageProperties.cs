namespace RabbitMQManagement.Models
{
    public class MessageProperties
    {
        public int DeliveryMode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string ContentType { get; set; }
    }
}
