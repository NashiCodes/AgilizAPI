#region

using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[ApiController]
[Route("/[controller]")]
public class UsersController(IUsersRepo repo) : ControllerBase
{
    // GET: /<UserController>/email=string&password=string
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] UserLogin requestUser) // Endpoint para login
    {
        return
            await repo.Login(requestUser)
                .ConfigureAwait(false); // Metodo de login, retorna o usuario e um token ou um estabelecimento e um token
    }

    // POST /<UserController>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserRegister requesUser)
    {
        return await repo.CadastrarUser(requesUser)
                   .ConfigureAwait(false); // Metodo de cadastro, retorna o usuario e um token
    }
}