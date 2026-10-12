using AgroMaintenance.Api.Configuration;
using AgroMaintenance.Api.Data;
using AgroMaintenance.Api.Requests;
using Microsoft.Extensions.Options;

namespace AgroMaintenance.Api.Services;

// Завдання 2,7. Спільні перевірки та логіка предметної області.
public sealed class RequestRules(
    AgroDbContext db,
    IOptions<ApiSettings> settings,
    IWebHostEnvironment environment)
{
    public void CheckDatabase()
    {
        if (environment.IsDevelopment()
            && settings.Value.SimulateDatabaseFailure)
        {
            throw new InvalidOperationException(
                "Навчальна імітація недоступності БД.");
        }
    }

    public static void CheckId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "Id має бути додатним числом.");
        }
    }

    public int[] ParseIds(string? ids)
    {
        if (string.IsNullOrWhiteSpace(ids))
        {
            throw new ArgumentException("Список Id порожній.");
        }

        var parts = ids.Split(',');

        if (parts.Length > settings.Value.MaxBulkSize)
        {
            throw new ArgumentException("Забагато Id у запиті.");
        }

        var result = new List<int>();

        foreach (var part in parts)
        {
            if (!int.TryParse(part.Trim(), out var id))
            {
                throw new ArgumentException(
                    "Id повинні бути цілими числами через кому.");
            }

            CheckId(id);
            result.Add(id);
        }

        return result.Distinct().ToArray();
    }

    public void CheckMaintenance(MaintenanceRequest? request)
    {
        if (request is null)
        {
            throw new ArgumentException(
                "Елемент масиву не може бути null.");
        }

        if (!db.Devices.Any(item => item.Id == request.DeviceId))
        {
            throw new ArgumentException(
                "Техніка з таким DeviceId відсутня.");
        }

        if (!db.WorkTypes.Any(item => item.Id == request.WorkTypeId))
        {
            throw new ArgumentException(
                "Вид робіт із таким WorkTypeId відсутній.");
        }
    }

    public void CheckBulk(int count)
    {
        if (count == 0 || count > settings.Value.MaxBulkSize)
        {
            throw new ArgumentException(
                $"Передайте від 1 до {settings.Value.MaxBulkSize} об'єктів.");
        }
    }

    public void CheckInventory(
        string number,
        int? exceptId = null)
    {
        if (db.Devices.Any(item =>
            item.InventoryNumber == number.Trim()
            && item.Id != exceptId))
        {
            throw new ArgumentException(
                "Інвентарний номер уже використано.");
        }
    }
}