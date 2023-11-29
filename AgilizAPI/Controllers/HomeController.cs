#region

using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[ApiController]
[Route("/")]
public class HomeController : ControllerBase
{
    public string Get
    {
        [HttpGet] get => "Bem Vindo ao Backend do AgilizApp!";
    }
}