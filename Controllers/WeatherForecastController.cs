using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;

namespace dynamodb_sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"};

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDynamoDBContext _dynamoDbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDynamoDBContext dynamoDbContext)
        {
            _logger = logger;
            _dynamoDbContext = dynamoDbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> GetAsync(string city="Bangalore")
        {
            return await _dynamoDbContext
                .QueryAsync<WeatherForecast>(city, QueryOperator.Between, new object[] { DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date.AddDays(1) })
                .GetRemainingAsync();
            
        }

        [HttpPost]
        public async Task Post(string  city= "Bangalore")
        {
            var data = GenerateDummyForecastData(city);
            foreach (var item in data)
            {
                await _dynamoDbContext.SaveAsync(item);
            }
        }
        private IEnumerable<WeatherForecast> GenerateDummyForecastData(string city)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    City = city,
                    Date = DateTime.UtcNow.Date,
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}