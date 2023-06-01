using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQManagement.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var Configuration=builder.Configuration;
// 从 appsettings.json 文件中读取 RabbitMQ 的连接配置
IConfigurationSection rabbitMqConfig = Configuration.GetSection("RabbitMQ");
builder.Services.Configure<RabbitMqOptions>(rabbitMqConfig);

// 注册 ConnectionFactory 并使用 IOptions<RabbitMqOptions> 进行注入
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

    //guest用户只是被容许从localhost访问,要在rabbitmq服务端创建新用户
    return new ConnectionFactory
    {
        HostName = options.HostName,
        UserName = options.UserName,
        Password = options.Password,
        Port = options.Port
    };
});
// 注入HttpClient
builder.Services.AddHttpClient("RabbitMQClient", config =>  //这里指定的name=RabbitMQClient，可以方便我们后期服用该实例
{
    config.BaseAddress = new Uri(rabbitMqConfig["ManagementApiUrl"]);
    config.Timeout = TimeSpan.FromSeconds(30);
    config.DefaultRequestHeaders.Add("Authorization", $"Basic {GetAuthHeaderValue(rabbitMqConfig["UserName"], rabbitMqConfig["Password"])}");

});
//// 注册 HttpClient，并配置基础地址和超时时间
//builder.Services.AddHttpClient("RabbitMQClient", client =>
//{
//    client.BaseAddress = new Uri(rabbitMqConfig["ManagementApiUrl"]);
//    client.Timeout = TimeSpan.FromSeconds(30);
//});

// 使用 Newtonsoft.Json 进行序列化和反序列化
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    };
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});




static string GetAuthHeaderValue(string username, string password)
{
    var authBytes = Encoding.ASCII.GetBytes($"{username}:{password}");
    return Convert.ToBase64String(authBytes);
}



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
