using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Language.SelectLanguage;

public class SelectLanguageHandler : IRequestHandler<SelectLanguageMessage, Result<WorkFlowResponse>>
{
    private readonly ILocalizer _localizer;
    private readonly IRequestContext _context;
    private readonly ISessionService _sessionService;

    public SelectLanguageHandler(ILocalizer localizer, IRequestContext context, ISessionService sessionService)
    {
        _localizer = localizer;
        _context = context;
        _sessionService = sessionService;
    }

    public async Task<Result<WorkFlowResponse>> Handle(SelectLanguageMessage request, CancellationToken cancellationToken)
    {
        var session = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.LanguageSelection, cancellationToken);
        _context.Session = session;
        var languages = LocalizationKeysConstants.LanguageSelection.AvailableLanguages;
        var Rows = new List<ButtonRow>();
        foreach (var language in languages)
        {
            var button = new ButtonData(_localizer.GetPhrase(WorkflowStep.LanguageSelection, language.LangName), $"{CallbackPrefixConstants.LanguageSelection}{CallbackPrefixConstants.Separator}{language.LangValue.ToString()}");
            if (language.LangValue == _context.Session.Language)
            {
                button.Text += _localizer.GetPhrase(WorkflowStep.LanguageSelection, LocalizationKeysConstants.LanguageSelection.Current);
            }

            Rows.Add(new ButtonRow(button));
        }

        var result = new WorkFlowResponse
        {
            ButtonRows = Rows,
            Text = _localizer.GetPhrase(WorkflowStep.LanguageSelection, LocalizationKeysConstants.LanguageSelection.Text)
        };
        return Result<WorkFlowResponse>.Success(result);
    }
}
