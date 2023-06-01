using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RabbitMQManagement.Models
{

    // 消息实体类
    //public class Message
    //{
    //    public string MessageId { get; set; }
    //    public string Exchange { get; set; }
    //    public string RoutingKey { get; set; }
    //    public string Payload { get; set; }
    //    // 其他需要的属性
    //}
    public class Message
    {
        [JsonProperty("payload_bytes")]
        public int PayloadBytes { get; set; }
        [JsonProperty("redelivered")]
        public bool Redelivered { get; set; }
        [JsonProperty("exchange")]
        public string Exchange { get; set; }
        [JsonProperty("routing_key")]
        public string RoutingKey { get; set; }
        [JsonProperty("message_count")]
        public int MessageCount { get; set; }
       
        public MessageProperties Properties { get; set; }
   
        public string Payload { get; set; }
        [JsonProperty("payload_encoding")]
        public string PayloadEncoding { get; set; }
    }
}
