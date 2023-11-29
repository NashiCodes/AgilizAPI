#region

using AgilizAPI.Models;

#endregion

namespace AgilizAPI.Repositories;

public interface IUsersRepo
{
    public Task<IResult> CadastrarUser(User user);
    public Task<IResult> Login(UserDto user);
}