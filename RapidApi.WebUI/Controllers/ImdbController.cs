using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RapidApi.WebUI.Models;

namespace RapidApi.WebUI.Controllers
{
    public class ImdbController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://imdb236.p.rapidapi.com/imdb/top250-movies"),
                Headers =
    {
        { "x-rapidapi-key", "9f6b62567cmshd12b6ac6c91e098p10e2d1jsneca27d197ab2" },
        { "x-rapidapi-host", "imdb236.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var Jsonbody = await response.Content.ReadAsStringAsync();
                var values= JsonConvert.DeserializeObject<List<IMDBViewModel.Movie>>(Jsonbody);
                return View(values);
            }
        
        }
    }
}
