using Microsoft.AspNetCore.Mvc;
using Shared.DTO.WeatherForecast;

namespace HttpApi.Host.Controllers
{
    public class WeatherForecastController:BaseController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("GetWeatherForecast")]
        public IEnumerable<WeatherForecastRequest> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastRequest
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
