using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Goals.New;

public class GoalNewHandler : IRequestHandler<GoalNewMessage, Result<WorkFlowResponse>>
{
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly ILocalizer _localizer;
    private readonly ISessionService _sessionService;
    private readonly IRequestContext _context;

    public GoalNewHandler(IBaseRepository<User, long> userRepository, ILocalizer localizer, ISessionService sessionService, IRequestContext context)
    {
        _userRepository = userRepository;
        _localizer = localizer;
        _sessionService = sessionService;
        _context = context;
    }

    public async Task<Result<WorkFlowResponse>> Handle(GoalNewMessage request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.ChatId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.UserNotFound);
        }

        if (user.Weight <= 0 || user.Height <= 0 || user.Age <= 0)
        {
            var needText = _localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.NeedWeight);
            var btnEdit = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.Profile.BtnEdit);
            var btnBack = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack);

            return Result<WorkFlowResponse>.Success(new WorkFlowResponse
            {
                Text = needText,
                ButtonRows = new[]
                {
                    new ButtonRow(new ButtonData(btnEdit, CallbackPrefixConstants.ProfileEdit)),
                    new ButtonRow(new ButtonData(btnBack, CallbackPrefixConstants.Back))
                }
            });
        }

        var session = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingGoalWeight, cancellationToken);
        _context.Session = session;

        return Result<WorkFlowResponse>.Success(new WorkFlowResponse
        {
            Text = _localizer.GetPhrase(WorkflowStep.AwaitingGoalWeight, LocalizationKeysConstants.Goals.AskWeight),
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnCancel), CallbackPrefixConstants.Cancel))
            }
        });
    }
}
