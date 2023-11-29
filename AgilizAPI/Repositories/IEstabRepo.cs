#region

using AgilizAPI.Models;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Repositories;

public interface IEstabRepo
{
    public Task<ActionResult<IEnumerable<EstablishmentDto>>> GetAll();
    public Task<ActionResult<EstablishmentDto>> GetEstabServices(Guid id);

    public Task<IActionResult> CadastrarEstab(Establishment estab);

    public Task<IActionResult> EditarEstab(Guid id, Establishment estab);
}