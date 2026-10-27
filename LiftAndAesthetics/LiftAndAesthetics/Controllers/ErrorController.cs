using Microsoft.AspNetCore.Mvc;

namespace LiftAndAesthetics.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "WRONG_LINK_EXCEPTION";
                    ViewBag.Description = "BRO, YOU LIFTED THE WRONG LINK. THIS PAGE HAS BEEN WIPED FROM THE SYSTEM.";
                    break;
                case 500:
                    ViewBag.ErrorMessage = "SYSTEM_FAILURE_CORE_CRITICAL";
                    ViewBag.Description = "CORE ERROR detected. Our engineers  are looking into it. Hold the gains.";
                    break;
                default:
                    ViewBag.ErrorMessage = $"ERROR_CODE_{statusCode}";
                    ViewBag.Description = "Something went wrong in the simulation.";
                    break;
            }

            return View("NotFound");
        }
    }
}