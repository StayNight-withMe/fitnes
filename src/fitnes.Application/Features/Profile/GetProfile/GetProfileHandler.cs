using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Profile.GetProfile;

public class GetProfileHandler : IRequestHandler<GetProfileRequest, Result<WorkFlowResponse>>
{
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly ILocalizer _localizer;

    public GetProfileHandler(IBaseRepository<User, long> userRepository, ILocalizer localizer)
    {
        _userRepository = userRepository;
        _localizer = localizer;
    }

    public async Task<Result<WorkFlowResponse>> Handle(GetProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.ChatId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.UserNotFound);
        }

        var notSet = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.Profile.NotSet);

        string weight = user.Weight > 0 ? $"{user.Weight} кг" : notSet;
        string height = user.Height > 0 ? $"{user.Height} см" : notSet;
        string age = user.Age > 0 ? user.Age.ToString() : notSet;
        string gender = user.Gender != Gender.Unknown ? user.Gender.ToString() : notSet;
        string waist = user.WaistCm is double waistValue && waistValue > 0 ? $"{waistValue} см" : notSet;
        string neck = user.NeckCm is double neckValue && neckValue > 0 ? $"{neckValue} см" : notSet;
        string hips = user.HipsCm is double hipsValue && hipsValue > 0 ? $"{hipsValue} см" : notSet;

        var cardTemplate = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.Profile.Card);
        var text = string.Format(cardTemplate, weight, height, age, gender, waist, neck, hips);

        var btnEdit = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.Profile.BtnEdit);
        var btnWeightEdit = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.Profile.BtnWeightEdit);
        var btnBack = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack);

        var response = new WorkFlowResponse
        {
            Text = text,
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(btnEdit, CallbackPrefixConstants.ProfileEdit)),
                new ButtonRow(new ButtonData(btnWeightEdit, CallbackPrefixConstants.ProfileWeightEdit)),
                new ButtonRow(new ButtonData(btnBack, CallbackPrefixConstants.Back))
            }
        };

        return Result<WorkFlowResponse>.Success(response);
    }
}
