namespace fitnes.Application.DTOs.WorkFlowResponse;

public class ButtonRow
{
    public IEnumerable<ButtonData> Buttons { get; set; } = default!;

    public ButtonRow(params ButtonData[] buttons)
    {
        Buttons = buttons;
    }
}
