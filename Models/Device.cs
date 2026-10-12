using System.ComponentModel.DataAnnotations;
using AgroMaintenance.Api.Validation;

namespace AgroMaintenance.Api.Models;

// Завдання 3. Модель сільськогосподарської техніки.
public sealed class Device
{
    [Key]
    public int Id { get; set; }

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