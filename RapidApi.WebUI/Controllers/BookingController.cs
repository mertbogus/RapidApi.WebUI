using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RapidApi.WebUI.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace RapidApi.WebUI.Controllers
{
    public class BookingController : Controller
    {
        private readonly string _apiKey = "9f6b62567cmshd12b6ac6c91e098p10e2d1jsneca27d197ab2"; // RapidAPI anahtarınız
        private readonly string _apiHost = "booking-com15.p.rapidapi.com"; // API host
        private readonly HttpClient _httpClient;

        public BookingController()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", _apiHost);
        }

        // Ana sayfa (otel arama formu)
        public IActionResult Index()
        {
            return View(new BookingHotelModel.Rootobject());
           
        }

        // Otel arama işlemi
        [HttpPost]
        public async Task<IActionResult> Search(string cityName, string checkInDate, string checkOutDate, int roomQty, int adults, string childrenAge)
        {
            // Şehir adı boşsa hata döndür
            if (string.IsNullOrEmpty(cityName))
            {
                return View("Index", new BookingHotelModel.Rootobject());
            }

            // Tarihlerin geçerli olup olmadığını kontrol et
            if (!DateTime.TryParse(checkInDate, out DateTime parsedCheckInDate) || !DateTime.TryParse(checkOutDate, out DateTime parsedCheckOutDate))
            {
                return View("Index", new BookingHotelModel.Rootobject());
            }

            // Tarihleri doğru formata çevir
            string formattedCheckInDate = parsedCheckInDate.ToString("yyyy-MM-dd");
            string formattedCheckOutDate = parsedCheckOutDate.ToString("yyyy-MM-dd");

            // Şehir adına göre destinasyon ID'sini al
            string destinationId = await GetDestinationIdAsync(cityName);
            if (string.IsNullOrEmpty(destinationId))
            {
                return View("Index", new BookingHotelModel.Data { hotels = new List<BookingHotelModel.Hotel>() });
            }

            // Destinasyon ID'si ile otel listesini al
            var hotels = await GetHotelsAsync(destinationId, formattedCheckInDate, formattedCheckOutDate, roomQty, adults, childrenAge);

            // Veriyi View'a gönder
            return View("Index", hotels);
        }

        // Şehir adına göre destinasyon ID'sini al
        private async Task<string> GetDestinationIdAsync(string cityName)
        {
            var requestUrl = $"https://{_apiHost}/api/v1/hotels/searchDestination?query={cityName}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                // JSON verisini Newtonsoft.Json ile deserialize ediyoruz
                var result = JsonConvert.DeserializeObject<BookingDestineationIdViewModel.Rootobject>(jsonResponse);

                if (result != null && result.data != null && result.data.Count > 0)
                {
                    return result.data[0].dest_id; // İlk sonucun destinasyon ID'sini döndür
                }
            }

            return null; // Destinasyon ID'si bulunamazsa null döndür
        }

        // Destinasyon ID'si ile otel listesini al
        private async Task<BookingHotelModel.Rootobject> GetHotelsAsync(
            string destinationId,
            string checkInDate,
            string checkOutDate,
            int roomQty,
            int adults,
            string childrenAge)
        {
            var requestUrl = $"https://{_apiHost}/api/v1/hotels/searchHotels?dest_id={destinationId}&search_type=CITY&arrival_date={checkInDate}&departure_date={checkOutDate}&adults={adults}&room_qty={roomQty}&page_number=1&units=metric&temperature_unit=c&languagecode=en-us&currency_code=TRY";

            if (!string.IsNullOrEmpty(childrenAge))
            {
                requestUrl += $"&children_age={childrenAge}";
            }

            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Yanıtı: " + jsonResponse); // Debug için ekledik

                // JSON verisini Newtonsoft.Json ile deserialize ediyoruz
                var result = JsonConvert.DeserializeObject<BookingHotelModel.Rootobject>(jsonResponse);

                if (result != null && result.data.hotels != null && result.data.hotels.Any())
                {
                    return result;
                }
            }
            else
            {
                Console.WriteLine("Hata Kodu: " + response.StatusCode);
            }
            var model = new BookingHotelModel.Rootobject();
            return model;
        }
    }
}
