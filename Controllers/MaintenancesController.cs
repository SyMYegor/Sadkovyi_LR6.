using AgroMaintenance.Api.Data;
using AgroMaintenance.Api.Filters;
using AgroMaintenance.Api.Mapping;
using AgroMaintenance.Api.Requests;
using AgroMaintenance.Api.Responses;
using AgroMaintenance.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroMaintenance.Api.Controllers;

// Завдання 6,7. CRUD, пошук, bulk і планові роботи.
[ApiController]
[Route("api/[controller]")]
[Consumes("application/json")]
[ServiceFilter(typeof(ApiExceptionFilter))]
[ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status500InternalServerError)]
public sealed class MaintenancesController(
    AgroDbContext db,
    RequestRules rules) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        typeof(List<MaintenanceResponse>),
        StatusCodes.Status200OK)]
    public ActionResult<List<MaintenanceResponse>> GetAll()
    {
        rules.CheckDatabase();

        return db.Maintenances.AsNoTracking()
            .OrderBy(item => item.Id)
            .AsEnumerable()
            .Select(item => item.ToResponse())
            .ToList();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(MaintenanceResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<MaintenanceResponse> GetById(int id)
    {
        RequestRules.CheckId(id);

        var entity = db.Maintenances.Find(id);

        if (entity is null)
            return NotFound();

        return entity.ToResponse();
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(MaintenanceResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<MaintenanceResponse> Create(
        MaintenanceRequest request)
    {
        rules.CheckMaintenance(request);

        var entity = request.ToEntity();

        db.Maintenances.Add(entity);
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
        MaintenanceRequest request)
    {
        RequestRules.CheckId(id);

        var entity = db.Maintenances.Find(id);

        if (entity is null)
            return NotFound();

        rules.CheckMaintenance(request);

        entity.Apply(request);
        db.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult Delete(int id)
    {
        RequestRules.CheckId(id);

        var entity = db.Maintenances.Find(id);

        if (entity is null)
            return Results.NotFound();

        db.Maintenances.Remove(entity);
        db.SaveChanges();

        return Results.NoContent();
    }

    // Завдання 6. ПОШУК за кількома Id: ids=1,2,3.
    [HttpGet("search")]
    [ProducesResponseType(
        typeof(List<MaintenanceResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<List<MaintenanceResponse>> Search(
        [FromQuery] string? ids)
    {
        var selected = rules.ParseIds(ids);

        return db.Maintenances.AsNoTracking()
            .Where(item => selected.Contains(item.Id))
            .OrderBy(item => item.Id)
            .AsEnumerable()
            .Select(item => item.ToResponse())
            .ToList();
    }

    // Завдання 6. Масове створення.
    [HttpPost("bulk")]
    [ProducesResponseType(
        typeof(List<MaintenanceResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<List<MaintenanceResponse>> CreateBulk(
        List<MaintenanceRequest> requests)
    {
        rules.CheckBulk(requests.Count);

        // ПЕРЕВІРКА весь масив до першого запису.
        foreach (var request in requests)
        {
            rules.CheckMaintenance(request);
        }

        var entities = requests
            .Select(request => request.ToEntity())
            .ToList();

        db.Maintenances.AddRange(entities);
        db.SaveChanges();

        return CreatedAtAction(
            nameof(Search),
            new
            {
                ids = string.Join(
                    ",",
                    entities.Select(item => item.Id))
            },
            entities.Select(item => item.ToResponse()).ToList());
    }

    // Завдання 6. Планові роботи за діапазоном дат.
    [HttpGet("upcoming")]
    [ProducesResponseType(
        typeof(List<MaintenanceResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<List<MaintenanceResponse>> Upcoming(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        if (from < new DateOnly(2000, 1, 1)
            || to > new DateOnly(2035, 12, 31)
            || from > to)
        {
            throw new ArgumentException(
                "Вкажіть правильний діапазон дат 2000–2035 років.");
        }

        return db.Maintenances.AsNoTracking()
            .Where(item =>
                item.Status == "Planned"
                && item.PlannedDate >= from
                && item.PlannedDate <= to)
            .OrderBy(item => item.PlannedDate)
            .ThenBy(item => item.Id)
            .AsEnumerable()
            .Select(item => item.ToResponse())
            .ToList();
    }
}