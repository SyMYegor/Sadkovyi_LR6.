using System.ComponentModel.DataAnnotations;

namespace AgroMaintenance.Api.Requests;

// Завдання 2,3,7. Вхідний DTO виду робіт.
public sealed class WorkTypeRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 2000)]
    public int IntervalHours { get; set; }

    [Range(0.25, 48)]
    public decimal EstimatedHours { get; set; }
}