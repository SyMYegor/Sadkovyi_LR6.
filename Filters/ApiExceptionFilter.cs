using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgroMaintenance.Api.Filters;

// Завдання 7. Спільна обробка винятків контролерів.
public sealed class ApiExceptionFilter(
    ILogger<ApiExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        IResult result;

        if (context.Exception is ArgumentException argument)
        {
            logger.LogWarning(
                "Помилка: {Message}",
                argument.Message);

            result = Results.BadRequest(new
            {
                message = argument.Message
            });
        }
        else if (context.Exception is InvalidOperationException operation)
        {
            logger.LogError(
                operation,
                "Помилка: {Message}",
                operation.Message);

            result = Results.Problem(
                statusCode: 500,
                title: "Помилка роботи з даними",
                detail:
                    "Не вдалося виконати операцію. " +
                    "Перевірте журнал сервера.");
        }
        else
        {
            return;
        }

        context.Result = new HttpResultActionResult(result);
        context.ExceptionHandled = true;
    }

    // Адаптер для виконання Results у MVC exception filter.
    private sealed class HttpResultActionResult(
        IResult result) : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            return result.ExecuteAsync(context.HttpContext);
        }
    }
}