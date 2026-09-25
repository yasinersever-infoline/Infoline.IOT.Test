using IotDashboard.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IotDashboard.Pages
{
    public class IndexModel : PageModel
    {
        public IotData[] Measurements { get; private set; } = Array.Empty<IotData>();

        public void OnGet()
        {
            Measurements = IotDataStore.GetHistory();
        }
    }
}