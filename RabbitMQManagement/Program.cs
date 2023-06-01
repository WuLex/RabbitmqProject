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
// �� appsettings.json �ļ��ж�ȡ RabbitMQ ����������
IConfigurationSection rabbitMqConfig = Configuration.GetSection("RabbitMQ");
builder.Services.Configure<RabbitMqOptions>(rabbitMqConfig);

// ע�� ConnectionFactory ��ʹ�� IOptions<RabbitMqOptions> ����ע��
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

    //guest�û�ֻ�Ǳ������localhost����,Ҫ��rabbitmq����˴������û�
    return new ConnectionFactory
    {
        HostName = options.HostName,
        UserName = options.UserName,
        Password = options.Password,
        Port = options.Port
    };
});
// ע��HttpClient
builder.Services.AddHttpClient("RabbitMQClient", config =>  //����ָ����name=RabbitMQClient�����Է������Ǻ��ڷ��ø�ʵ��
{
    config.BaseAddress = new Uri(rabbitMqConfig["ManagementApiUrl"]);
    config.Timeout = TimeSpan.FromSeconds(30);
    config.DefaultRequestHeaders.Add("Authorization", $"Basic {GetAuthHeaderValue(rabbitMqConfig["UserName"], rabbitMqConfig["Password"])}");

});
//// ע�� HttpClient�������û�����ַ�ͳ�ʱʱ��
//builder.Services.AddHttpClient("RabbitMQClient", client =>
//{
//    client.BaseAddress = new Uri(rabbitMqConfig["ManagementApiUrl"]);
//    client.Timeout = TimeSpan.FromSeconds(30);
//});

// ʹ�� Newtonsoft.Json �������л��ͷ����л�
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
