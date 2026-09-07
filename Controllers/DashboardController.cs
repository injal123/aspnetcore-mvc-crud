using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;



namespace MyMvcApp.Controllers
{

    
    // This attribute ensures that only authenticated users can access the actions in this controller.
    [Authorize] 
    public class DashboardController() : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

    }
}