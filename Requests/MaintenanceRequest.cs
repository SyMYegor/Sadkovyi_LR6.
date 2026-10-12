using System.ComponentModel.DataAnnotations;
using AgroMaintenance.Api.Validation;

namespace AgroMaintenance.Api.Requests;

// Завдання 2,3,7. Вхідний DTO обслуговування.
public sealed class MaintenanceRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int DeviceId { get; set; }

    [Range(1, int.MaxValue)]
    public int WorkTypeId { get; set; }

    [Required, StringLength(80, MinimumLength = 2)]
    public string AssignedTo { get; set; } = string.Empty;

    [Required, ModernDate]
    public DateOnly PlannedDate { get; set; }

    [Range(1, 5)]
    public int Priority { get; set; }

    [Required, RegularExpression("^(Planned|Completed)$")]
    public string Status { get; set; } = string.Empty;
}