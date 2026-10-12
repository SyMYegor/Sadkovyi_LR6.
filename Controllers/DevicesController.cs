using AgroMaintenance.Api.Data;
using AgroMaintenance.Api.Filters;
using AgroMaintenance.Api.Mapping;
using AgroMaintenance.Api.Requests;
using AgroMaintenance.Api.Responses;
using AgroMaintenance.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroMaintenance.Api.Controllers;

// Завдання 6,7. CRUD техніки, типи повернення та HTTP-коди.
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[ServiceFilter(typeof(ApiExceptionFilter))]
[ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status500InternalServerError)]
public sealed class DevicesController(
    AgroDbContext db,
    RequestRules rules) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        typeof(List<DeviceResponse>),
        StatusCodes.Status200OK)]
    public List<DeviceResponse> GetAll()
    {
        rules.CheckDatabase();

        return db.Devices.AsNoTracking()
            .OrderBy(item => item.Id)
            .AsEnumerable()
            .Select(item => item.ToResponse())
            .ToList();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(DeviceResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DeviceResponse> GetById(int id)
    {
        RequestRules.CheckId(id);

        var entity = db.Devices.Find(id);

        if (entity is null)
            return NotFound();

        return entity.ToResponse();
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(DeviceResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DeviceResponse> Create(DeviceRequest request)
    {
        rules.CheckInventory(request.InventoryNumber);

        var entity = request.ToEntity();

        db.Devices.Add(entity);
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
    public IActionResult Update(int id, DeviceRequest request)
    {
        RequestRules.CheckId(id);

        var entity = db.Devices.Find(id);

        if (entity is null)
            return NotFound();

        rules.CheckInventory(request.InventoryNumber, id);

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

        var entity = db.Devices.Find(id);

        if (entity is null)
            return Results.NotFound();

        if (db.Maintenances.Any(item => item.DeviceId == id))
        {
            return Results.Conflict(new
            {
                message = "Запис використовується у графіку."
            });
        }

        db.Devices.Remove(entity);
        db.SaveChanges();

        return Results.NoContent();
    }
}