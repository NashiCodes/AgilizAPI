#region

using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[ApiController]
[Route("/[controller]")]
public class UsersController(UsersRepo repo) : ControllerBase
{
    // GET: /<UserController>/email=string&password=string
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] UserLogin requestUser)
    {
        return await repo.Login(requestUser);
    }

    // POST /<UserController>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserRegister requesUser)
    {
        return await repo.CadastrarUser(requesUser);
    }
}