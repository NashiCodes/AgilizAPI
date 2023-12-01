#region

using AgilizAPI.Data;
using AgilizAPI.Models;

#endregion

namespace AgilizAPI.Repositories;

public class UsersRepo(AgilizApiContext context) : IUsersRepo
{
    public async Task<IResult> CadastrarUser(UserRegister requestUser)
    {
        try
        {
            await IUsersRepo.ValidateEmail(requestUser.Email);
            var user = requestUser.toUser();
            user.CreatePasswordHash(requestUser.Password);
            await context.User.AddAsync(user);
            await context.SaveChangesAsync();
            return Results.Ok("Usuario Cadastrado com sucesso");
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    public async Task<IResult> Login(UserLogin user)
    {
        try
        {
            await IUsersRepo.ValidateEmail(user.Email);
            var userDb = await context.User.FindAsync(user.Email);

            if (userDb == null) return Results.BadRequest("Usuario não cadastrado");
            if (!userDb.VerifyPassword(user.Password)) return Results.BadRequest("Senha incorreta");

            return userDb.Role == UserClaims.CommonUser ? LoadEstablishment(userDb) : Results.Ok(userDb.ToDto(""));
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    private IResult LoadEstablishment(User dbUser)
    {
        var estabishment =
            context.Establishment.Where(e => e.Email.Equals(dbUser.Email) && e.VerifyPassword(dbUser.Password));

        return estabishment.Any()
                   ? Results.Ok(estabishment.First())
                   : Results.BadRequest("Estabelecimento Não encontrado \n, contate o suporte para mais informações");
    }
}