using System.ComponentModel.DataAnnotations;

namespace AgroMaintenance.Api.Models;

// Завдання 3. Модель виду технічного обслуговування.
public sealed class WorkType
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 2000)]
    public int IntervalHours { get; set; }

    [Range(0.25, 48)]
    public decimal EstimatedHours { get; set; }
}