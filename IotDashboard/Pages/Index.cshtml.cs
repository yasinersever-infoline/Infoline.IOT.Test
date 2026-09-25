using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IotDashboard.Models;

namespace IotDashboard.Pages
{
    public class IndexModel : PageModel
    {
        public IotData DisplayData { get; set; } = null!;
        public void OnGet()
        {
            DisplayData = IotDataStore.CurrentData;
        }

        // JavaScript'in her 3 saniyede bir "Short Polling" ile çağıracağı metot
        public JsonResult OnGetLatestIotData()
        {
            /* // IoT cihazı varmış gibi değerleri her istekte ufakça rastgele değiştirelim (Simülasyon)
            var random = new Random();
            IotDataStore.CurrentData.Temperature += (random.NextDouble() - 0.5); // -0.5 ile +0.5 arası değişim
            IotDataStore.CurrentData.Humidity += (random.NextDouble() - 0.5);
            IotDataStore.CurrentData.Timestamp = DateTime.Now;

            // Veriyi tarayıcıya JSON formatında fırlatıyoruz
            return new JsonResult(IotDataStore.CurrentData); */

            // Gerçek IoT cihazından gelen veriyi tarayıcıya JSON formatında fırlatıyoruz
            return new JsonResult(IotDataStore.CurrentData);
        }
    }
}
