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

            var existEstab = await context.Establishment.Where(e => e.Email.Equals(userDb.Email)).ToListAsync();

            if (existEstab.Count == 0)
                return new NotFoundObjectResult(new {
                    Message = "Erro ao logar",
                    Reason  = "Estabelecimento não encontrado, ou não cadastrado",
                    Token   = userDb.GenerateToken()
                });

            var estab = existEstab.Find(e => e.VerifyPassword(userDb.Password));


            return estab is not null
                       ? await new EstabRepo(context).LoginEstab(estab.Id, userDb.GenerateToken())
                       : new OkObjectResult(userDb.ToDto(userDb.GenerateToken(), new List<Scheduler>()));
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
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