using System.ComponentModel.DataAnnotations;

namespace AgroMaintenance.Api.Validation;

// Завдання 3,7. Відсутня дата та нелогічні роки не допускаються.
[AttributeUsage(AttributeTargets.Property)]
public sealed class ModernDateAttribute : ValidationAttribute
{
    public ModernDateAttribute()
        : base("Дата має бути в межах 2000–2035 років.")
    {
    }

    public override bool IsValid(object? value) =>
        value is DateOnly date
        && date >= new DateOnly(2000, 1, 1)
        && date <= new DateOnly(2035, 12, 31);
}