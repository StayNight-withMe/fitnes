namespace fitnes.Application.DTOs.WorkFlowResponse;

public class ButtonData
{
    public string Text { get; set; } = default!;
    public string CallbackData { get; set; } = default!;

    public ButtonData(string text, string callbackData)
    {
        Text = text;
        CallbackData = callbackData;
    }
}
