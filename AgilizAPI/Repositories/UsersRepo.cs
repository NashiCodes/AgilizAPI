#region

using AgilizAPI.Data;
using AgilizAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AgilizAPI.Repositories.IUsersRepo;

// ReSharper disable All

#endregion

namespace AgilizAPI.Repositories;

public class UsersRepo(AgilizApiContext context) : IUsersRepo
{
    public async Task<IActionResult> CadastrarUser(UserRegister requestUser)
    {
        try
        {
            await ValidateProperties(requestUser);

            if (UserExists(requestUser.Email)) return new BadRequestObjectResult("Email já cadastrado");
            return await RegisterUser(requestUser);
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    public async Task<IActionResult> Login(UserLogin user)
    {
        try
        {
            await ValidateEmail(user.Email);

            var userDb = await context.User.FindAsync(user.Email);

            if (userDb == null) return new NotFoundObjectResult("Usuario não encontrado");
            if (!userDb.VerifyPassword(user.Password)) return new BadRequestObjectResult("Senha incorreta");

            if (userDb.Role != "Entrepreneur")
            {
                var scheduler = await new SchedulerRepo(context).GetSchedulerByUser(userDb.Email);
                return new OkObjectResult(userDb.ToDto(userDb.GenerateToken(), scheduler));
            }

            var existeEstab = await context.Establishment.Select(e => e.Email == user.Email).FirstOrDefaultAsync();
            if (!existeEstab) return new NotFoundObjectResult("Estabelecimento não encontrado, ou Não cadastrado");

            var estab = await context.Establishment.Where(e => e.Email == user.Email && e.VerifyPassword(userDb
                                                                        .Password))
                            .FirstOrDefaultAsync();

            if (estab != null) return await new EstabRepo(context).loginEstab(estab.Id, userDb.GenerateToken());
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }

        return new BadRequestObjectResult("Erro ao logar");
    }

    private async Task<IActionResult> RegisterUser(UserRegister requestUser)
    {
        var userDb = context.User.Where(u => u.UserCpf == requestUser.UserCpf).FirstOrDefault();
        if (userDb is not null) return new BadRequestObjectResult("CPF já cadastrado");

        var user = requestUser.toUser();
        user.CreatePasswordHash(requestUser.Password);
        await context.User.AddAsync(user);
        await context.SaveChangesAsync();

        return new CreatedAtActionResult("Get", "Users", new { email = user.Email },
                                         user.ToDto(user.GenerateToken(), new List<Scheduler>()));
    }

    private bool UserExists(string Email)
    {
        return context.Establishment.Any(e => e.Email == Email);
    }
}