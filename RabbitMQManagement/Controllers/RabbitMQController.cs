using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQManagement.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using IConnectionFactory = RabbitMQ.Client.IConnectionFactory;

namespace RabbitMQManagement.Controllers
{
    public class RabbitMQController : Controller
    {
        //private readonly ConnectionFactory _connectionFactoryInstance;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        public RabbitMQController(IConnectionFactory connectionFactory, ILogger<RabbitMQController> logger, IHttpClientFactory httpClientFactory)
        {
            //_connectionFactoryInstance = new ConnectionFactory
            //{
            //    // 设置 RabbitMQ 连接参数
            //    HostName = "localhost",
            //    Port = 5672,
            //    UserName = "guest",
            //    Password = "guest",
            //    VirtualHost = "/"
            //};
            _connectionFactory = connectionFactory;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient("RabbitMQClient");
        }

        [HttpGet]
        public IActionResult Index()
        {
            
            return View();
        }

        // 显示所有交换机
        public IActionResult Exchanges()
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // 获取交换机列表
                var exchanges = ""; //channel.ExchangeDeclareNoWait("");
                return View(exchanges);
            }
        }

        // 显示所有队列
        public IActionResult Queues()
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // 获取队列列表
                var queues = channel.QueueDeclarePassive("");
                return View(queues);
            }
        }

        // 发送消息
        [HttpPost]
        public IActionResult SendMessage(string exchange, string routingKey, string message)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // 发布消息
                var body = System.Text.Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange, routingKey, null, body);
            }

            return RedirectToAction("Exchanges");
        }

        // 接收消息
        public IActionResult ReceiveMessage(string queue)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // 消费消息
                var result = channel.BasicGet(queue, autoAck: true);

                if (result != null)
                {
                    var message = System.Text.Encoding.UTF8.GetString(result.Body.ToArray());
                    return Content($"Received message: {message}");
                }

                return Content("No message in the queue.");
            }
        }

        // 删除队列
        public IActionResult DeleteQueue(string queue)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // 删除队列
                channel.QueueDelete(queue);
            }

            return RedirectToAction("Queues");
        }


        /// <summary>
        /// 读取一条消息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // 使用 _connectionFactory 进行 RabbitMQ 连接和操作
                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        // 定义队列名称
                        string queueName = "orderqueues";

                        // 声明队列
                        channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                        // 获取队列中的消息
                        var result = channel.BasicGet(queueName, autoAck: true);
                        if (result != null)
                        {
                            // 将消息内容转换为字符串
                            var message = Encoding.UTF8.GetString(result.Body.ToArray());
                            return Ok(message);
                        }
                        else
                        {
                            return NoContent(); // 没有消息时返回 No Content 状态码
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                return BadRequest(ex.Message);
            }
        }

        // 获取 RabbitMQ 队列中的消息列表
        [HttpGet]
        public async Task<IActionResult> GetMlist()
        {
            try
            {
                // 创建 HttpClient
                //var client = _httpClientFactory.CreateClient();

                // 构建请求内容
                
                //count控制要获取的最大消息数。如果队列不能立即提供它们，您可能会收到比这更少的消息。
                //ackmode确定消息是否将从队列中删除。如果 ackmode 是 ack_requeue_true 或 reject_requeue_true 它们将被重新排队 - 如果 ackmode 是 ack_requeue_false 或 reject_requeue_false 它们将被删除。
                //encoding必须是“auto”（在这种情况下，如果负载是有效的 UTF-8，则负载将作为字符串返回，否则为 base64 编码）或“base64”（在这种情况下，负载将始终为 base64 编码）。
                //如果truncate存在，如果它大于给定的大小（以字节为单位），它将截断消息有效负载。
                var requestData = new
                {
                    count = 5,
                    ackmode = "ack_requeue_true",
                    encoding = "auto",
                    truncate = 50000
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                //var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
               
                // 发起 POST 请求
                var response = await _httpClient.PostAsync("queues/%2F/orderqueues/get", jsonContent);
                //// 发起 GET 请求获取队列中的消息
                //var response = await _httpClient.GetAsync("queues/%2F/orderqueues/get");

                if (response.IsSuccessStatusCode)
                {
                    // 将返回的内容序列化为实体类
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var messages = JsonConvert.DeserializeObject<List<Message>>(jsonString);

                    // 返回消息列表给前端
                    return Ok(new { code = 0, msg = "success", count = messages.Count, data = messages });
                }
                else
                {
                    // 处理请求失败的情况
                    var errorMessage = $"Failed to get messages from RabbitMQ. StatusCode: {response.StatusCode}";
                    _logger.LogError(errorMessage);
                    return StatusCode((int)response.StatusCode, errorMessage);
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                _logger.LogError(ex, "Failed to get messages from RabbitMQ.");
                return BadRequest(ex.Message);
            }
        }


        // 获取 RabbitMQ 消息列表
        [HttpGet("Get")]
        public async Task<IActionResult> GetList()
        {
            var response = await _httpClient.GetAsync("http://192.168.0.111:15672/api/queues/%2F/orderqueues/get");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get RabbitMQ messages: {StatusCode}", response.StatusCode);
                return BadRequest();
            }

            var json = await response.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<RabbitMQMessage>>(json);
            return Ok(new { code = 0, msg = "success", count = messages.Count, data = messages });
        }

        // 发布 RabbitMQ 消息
        [HttpPost("Publish")]
        public async Task<IActionResult> Publish([FromBody] RabbitMQMessage message)
        {
            var json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://localhost:15672/api/exchanges/%2F/orderexchanges/publish", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to publish RabbitMQ message: {StatusCode}", response.StatusCode);
                return BadRequest();
            }

            return Ok(new { code = 0, msg = "success" });
        }

        // 删除 RabbitMQ 消息
        [HttpDelete("Delete/{messageId}")]
        public async Task<IActionResult> Delete(string messageId)
        {
            var response = await _httpClient.DeleteAsync($"http://localhost:15672/api/queues/%2F/orderqueues/get/{messageId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to delete RabbitMQ message: {StatusCode}", response.StatusCode);
                return BadRequest();
            }

            return Ok(new { code = 0, msg = "success" });
        }

        //// 更新 RabbitMQ 消息
        //[HttpPut("Update")]
        //public async Task<IActionResult> Update([FromBody] RabbitMQMessage message)
        //{
        //    var json = JsonConvert.SerializeObject(message);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");
        //    var response = await _httpClient.PutAsync($"http://localhost:15672/api/queues/%2F/orderqueues/get/{message.MessageId}", content);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        _logger.LogError("Failed to update RabbitMQ message: {StatusCode}", response.StatusCode);
        //        return BadRequest();
        //    }

        //    return Ok(new { code = 0, msg = "success" });
        //}
    }

}
