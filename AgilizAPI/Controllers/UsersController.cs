#region

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
    public async Task<IResult> Get([FromQuery] UserLogin requestUser)
    {
        return await repo.Login(requestUser);
    }

    // POST /<UserController>
    [HttpPost]
    public async Task<IResult> Post([FromBody] UserRegister requesUser)
    {
        return await repo.CadastrarUser(requesUser);
    }
}