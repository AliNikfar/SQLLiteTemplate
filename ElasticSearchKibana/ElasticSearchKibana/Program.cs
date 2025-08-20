using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
ConfigureLogging();
builder.Host.UseSerilog();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
void ConfigureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json"
        , optional: false /* this option means file should be exiest */
        , reloadOnChange: true /* this option means any changes on appsetting file sense and update during even runtime */)
        .AddJsonFile($"appsettings.{environment}.json",optional:true)
        .Build();

        Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration,environment))
        .Enrich.WithProperty("Environment",environment).
        ReadFrom.Configuration(configuration).CreateLogger();
}
ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration,string environment)
{
    //create and configure the Index
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".","-") /*this replace used because Elastic does not accept "." */}-{environment.ToLower()}-{DateTime.UtcNow:yyy-MM} ",
        NumberOfReplicas = 1 ,
        NumberOfShards = 9 ,
    };
}
