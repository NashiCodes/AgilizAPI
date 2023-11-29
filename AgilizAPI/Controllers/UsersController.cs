#region

using AgilizAPI.Models;
using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[Route("/[controller]")]
[ApiController]
public class UsersController(IUsersRepo repo) : ControllerBase
{
    // GET: /<UserController>/email=string&password=string
    [HttpGet]
    public async Task<IResult> Get([FromQuery] UserDto user)
    {
        return await repo.Login(user);
    }

    // POST /<UserController>
    [HttpPost]
    public async Task<IResult> Post([FromBody] User user)
    {
        return await repo.CadastrarUser(user);
    }
}