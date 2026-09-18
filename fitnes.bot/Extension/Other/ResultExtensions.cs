using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Models.Common;
using Telegram.Bot.Types.ReplyMarkups;

namespace fitnes.bot.Extension.Other;

public static class ResultExtensions
{
    public static string GetTextOrErrors(this Result<WorkFlowResponse> result)
    {
        if (result.IsSuccess)
        {
            return result.Value!.Text;
        }

        return string.Join(", ", result.ErrorsList);
    }

    public static InlineKeyboardMarkup? ToTelegramMarkup(this Result<WorkFlowResponse> result)
    {
        if (!result.IsSuccess || result.Value?.ButtonRows is null)
        {
            return null;
        }

        var keyboardButtons = result.Value.ButtonRows
            .Select(row => row.Buttons
                .Select(b => InlineKeyboardButton.WithCallbackData(b.Text, b.CallbackData)))
            .ToList();

        return new InlineKeyboardMarkup(keyboardButtons);
    }
}
