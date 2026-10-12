namespace AgroMaintenance.Api.Responses;

// Завдання 2,6. Вихідний DTO техніки.
public sealed record DeviceResponse(
    int Id,
    string Title,
    string Manufacturer,
    DateOnly CommissionedOn,
    int ConditionRating,
    string Category,
    string InventoryNumber);