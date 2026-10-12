using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AgroMaintenance.Api.Models;

namespace AgroMaintenance.Api.Data;

// Завдання 5. Завантаження тестових записів із src/data.
public static class SeedData
{
    public static void Initialize(
        AgroDbContext db,
        string contentRoot)
    {
        if (db.Devices.Any())
            return;

        var devices = Read<Device>("devices.json", contentRoot);

        var workTypes = Read<WorkType>(
            "worktypes.json",
            contentRoot);

        var maintenances = Read<Maintenance>(
            "maintenances.json",
            contentRoot);

        if (devices.Count < 5
            || workTypes.Count < 5
            || maintenances.Count < 20)
        {
            throw new InvalidOperationException(
                "Недостатньо початкових даних.");
        }

        if (maintenances.Any(item =>
            !devices.Any(device => device.Id == item.DeviceId)
            || !workTypes.Any(work => work.Id == item.WorkTypeId)))
        {
            throw new InvalidOperationException(
                "Початковий графік містить відсутні довідники.");
        }

        db.Devices.AddRange(devices);
        db.WorkTypes.AddRange(workTypes);
        db.Maintenances.AddRange(maintenances);

        db.SaveChanges();

        Console.WriteLine(
            $"Дані: техніка — {devices.Count}; " +
            $"роботи — {workTypes.Count}; " +
            $"графік — {maintenances.Count}.");
    }

    private static List<T> Read<T>(string name, string root)
    {
        var path = Path.Combine(root, "src", "data", name);

        var items = JsonSerializer.Deserialize<List<T>>(
            File.ReadAllText(path),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })
            ?? throw new InvalidOperationException(
                $"Порожній файл {name}.");

        foreach (var item in items)
        {
            Validator.ValidateObject(
                item!,
                new ValidationContext(item!),
                true);
        }

        return items;
    }
}