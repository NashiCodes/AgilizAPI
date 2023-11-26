using AgilizaAppAPI;
using AgilizAPI.Models;
using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AgilizAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUsersRepo repo) : ControllerBase
{
    private readonly IUsersRepo _repo = repo;

    // GET: api/<UserController>/email=string&password=string
    [HttpGet]
    public IResult Get([FromQuery] userDto user)
    {
        return _repo.login(user);
    }

    // POST api/<UserController>
    [HttpPost]
    public async Task<IResult> Post([FromBody] User user)
    {
        return await _repo.CadastrarUser(user);
    }
}