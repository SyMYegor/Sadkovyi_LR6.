using System.ComponentModel.DataAnnotations;

namespace AgroMaintenance.Api.Configuration;

// Завдання 4. Конфігурація та валідація параметрів.
public sealed class ApiSettings
{
    [Required, StringLength(80, MinimumLength = 4)]
    public string DatabaseName { get; set; } = string.Empty;

    [Range(1, 500)]
    public int MaxBulkSize { get; set; } = 200;

    public bool SimulateDatabaseFailure { get; set; }
}