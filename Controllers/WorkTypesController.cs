using AgroMaintenance.Api.Data;
using AgroMaintenance.Api.Filters;
using AgroMaintenance.Api.Mapping;
using AgroMaintenance.Api.Requests;
using AgroMaintenance.Api.Responses;
using AgroMaintenance.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroMaintenance.Api.Controllers;

// Завдання 6,7. CRUD видів технічного обслуговування.
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[ServiceFilter(typeof(ApiExceptionFilter))]
[ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status500InternalServerError)]
public sealed class WorkTypesController(
    AgroDbContext db,
    RequestRules rules) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        typeof(List<WorkTypeResponse>),
        StatusCodes.Status200OK)]
    public ActionResult<List<WorkTypeResponse>> GetAll()
    {
        rules.CheckDatabase();

        return db.WorkTypes.AsNoTracking()
            .OrderBy(item => item.Id)
            .AsEnumerable()
            .Select(item => item.ToResponse())
            .ToList();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(WorkTypeResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WorkTypeResponse> GetById(int id)
    {
        RequestRules.CheckId(id);

        var entity = db.WorkTypes.Find(id);

        if (entity is null)
            return NotFound();

        return entity.ToResponse();
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(WorkTypeResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<WorkTypeResponse> Create(
        WorkTypeRequest request)
    {
        var entity = request.ToEntity();

        db.WorkTypes.Add(entity);
        db.SaveChanges();

        return CreatedAtAction(
            nameof(GetById),
            new { id = entity.Id },
            entity.ToResponse());
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(
        int id,
        WorkTypeRequest request)
    {
        RequestRules.CheckId(id);

        var entity = db.WorkTypes.Find(id);

        if (entity is null)
            return NotFound();

        entity.Apply(request);
        db.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IResult Delete(int id)
    {
        RequestRules.CheckId(id);

        var entity = db.WorkTypes.Find(id);

        if (entity is null)
            return Results.NotFound();

        if (db.Maintenances.Any(item => item.WorkTypeId == id))
        {
            return Results.Conflict(new
            {
                message = "Запис використовується у графіку."
            });
        }

        db.WorkTypes.Remove(entity);
        db.SaveChanges();

        return Results.NoContent();
    }
}