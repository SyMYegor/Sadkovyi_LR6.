namespace AgroMaintenance.Api.Responses;

// Завдання 2,6. Вихідний DTO обслуговування.
public sealed record MaintenanceResponse(
    int Id,
    string Title,
    int DeviceId,
    int WorkTypeId,
    string AssignedTo,
    DateOnly PlannedDate,
    int Priority,
    string Status);