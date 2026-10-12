using AgroMaintenance.Api.Models;
using AgroMaintenance.Api.Requests;
using AgroMaintenance.Api.Responses;

namespace AgroMaintenance.Api.Mapping;

// Завдання 2. Перетворення DTO без дублювання в контролерах.
public static class DtoMapping
{
    public static DeviceResponse ToResponse(this Device entity) => new(
        entity.Id,
        entity.Title,
        entity.Manufacturer,
        entity.CommissionedOn,
        entity.ConditionRating,
        entity.Category,
        entity.InventoryNumber);

    public static Device ToEntity(this DeviceRequest request)
    {
        var entity = new Device();
        entity.Apply(request);
        return entity;
    }

    public static void Apply(this Device entity, DeviceRequest request)
    {
        entity.Title = request.Title.Trim();
        entity.Manufacturer = request.Manufacturer.Trim();
        entity.CommissionedOn = request.CommissionedOn;
        entity.ConditionRating = request.ConditionRating;
        entity.Category = request.Category.Trim();
        entity.InventoryNumber = request.InventoryNumber.Trim();
    }

    public static WorkTypeResponse ToResponse(this WorkType entity) => new(
        entity.Id,
        entity.Title,
        entity.IntervalHours,
        entity.EstimatedHours);

    public static WorkType ToEntity(this WorkTypeRequest request)
    {
        var entity = new WorkType();
        entity.Apply(request);
        return entity;
    }

    public static void Apply(this WorkType entity, WorkTypeRequest request)
    {
        entity.Title = request.Title.Trim();
        entity.IntervalHours = request.IntervalHours;
        entity.EstimatedHours = request.EstimatedHours;
    }

    public static MaintenanceResponse ToResponse(this Maintenance entity) => new(
        entity.Id,
        entity.Title,
        entity.DeviceId,
        entity.WorkTypeId,
        entity.AssignedTo,
        entity.PlannedDate,
        entity.Priority,
        entity.Status);

    public static Maintenance ToEntity(this MaintenanceRequest request)
    {
        var entity = new Maintenance();
        entity.Apply(request);
        return entity;
    }

    public static void Apply(
        this Maintenance entity,
        MaintenanceRequest request)
    {
        entity.Title = request.Title.Trim();
        entity.DeviceId = request.DeviceId;
        entity.WorkTypeId = request.WorkTypeId;
        entity.AssignedTo = request.AssignedTo.Trim();
        entity.PlannedDate = request.PlannedDate;
        entity.Priority = request.Priority;
        entity.Status = request.Status.Trim();
    }
}