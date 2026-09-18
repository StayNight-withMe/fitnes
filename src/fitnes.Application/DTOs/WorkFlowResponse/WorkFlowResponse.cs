namespace fitnes.Application.DTOs.WorkFlowResponse;

public class WorkFlowResponse
{
    public string Text { get; set; } = default!;
    public IEnumerable<ButtonRow>? ButtonRows {  get; set; }
}
