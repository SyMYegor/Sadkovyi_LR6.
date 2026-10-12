using System.ComponentModel.DataAnnotations;
using AgroMaintenance.Api.Validation;

namespace AgroMaintenance.Api.Models;

// Завдання 3. Модель запису графіка обслуговування.
public sealed class Maintenance
{
    [Key]
    public int Id { get; set; }

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