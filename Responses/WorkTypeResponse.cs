namespace AgroMaintenance.Api.Responses;

// Завдання 2,6. Вихідний DTO виду робіт.
public sealed record WorkTypeResponse(
    int Id,
    string Title,
    int IntervalHours,
    decimal EstimatedHours);