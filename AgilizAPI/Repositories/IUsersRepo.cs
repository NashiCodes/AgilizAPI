using AgilizaAppAPI;
using AgilizAPI.Models;

namespace AgilizAPI.Repositories;

public interface IUsersRepo
{
    public Task<IResult> CadastrarUser(User user);
    public IResult login(userDto user);
}