using System.Text.Json.Serialization;

namespace RabbitMQManagement.Models
{
    // 实体类，用于表示 RabbitMQ 消息
    //public class RabbitMQMessage
    //{
    //    public string MessageId { get; set; }
    //    public string Exchange { get; set; }
    //    public string RoutingKey { get; set; }
    //    public string Payload { get; set; }
    //    // 其他需要展示的字段
    //}

    public class RabbitMQMessage
    {
        [JsonPropertyName("payload_bytes")]
        public int PayloadBytes { get; set; }
        [JsonPropertyName("redelivered")]
        public bool Redelivered { get; set; }
        [JsonPropertyName("exchange")]
        public string Exchange { get; set; }
        [JsonPropertyName("routing_key")]
        public string RoutingKey { get; set; }
        [JsonPropertyName("message_count")]
        public int MessageCount { get; set; }

        public MessageProperties Properties { get; set; }

        public string Payload { get; set; }
        [JsonPropertyName("payload_encoding")]
        public string PayloadEncoding { get; set; }
    }

}
