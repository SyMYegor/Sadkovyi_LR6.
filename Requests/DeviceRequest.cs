using System.ComponentModel.DataAnnotations;
using AgroMaintenance.Api.Validation;

namespace AgroMaintenance.Api.Requests;

// Завдання 2,3,7. Вхідний DTO техніки без серверного Id.
public sealed class DeviceRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Manufacturer { get; set; } = string.Empty;

    [Required, ModernDate]
    public DateOnly CommissionedOn { get; set; }

    [Range(1, 5)]
    public int ConditionRating { get; set; }

    [Required, RegularExpression("^(Tractor|Combine|Sprayer|Seeder|Loader)$")]
    public string Category { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string InventoryNumber { get; set; } = string.Empty;
}