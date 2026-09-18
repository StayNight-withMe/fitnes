using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Constants.Profile;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Profile.Survey;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileRequest, Result<WorkFlowResponse>>
{
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly ISessionService _sessionService;
    private readonly IRequestContext _context;
    private readonly ILocalizer _localizer;

    public UpdateProfileHandler(IBaseRepository<User, long> userRepository, ISessionService sessionService, IRequestContext context, ILocalizer localizer)
    {
        _userRepository = userRepository;
        _sessionService = sessionService;
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<WorkFlowResponse>> Handle(UpdateProfileRequest request, CancellationToken ct)
    {
        var session = _context.Session;
        if (session is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.SessionExpired);
        }

        var user = await _userRepository.GetById(request.ChatId, ct);
        if (user is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.UserNotFound);
        }

        switch (session.State)
        {
            case WorkflowStep.AwaitingTimezone:
            {
                if (!UpdateProfileTimezoneParser.TryParse(request.Input, out var offsetMinutes))
                {
                    return Result<WorkFlowResponse>.Success(InvalidTimezoneResponse());
                }

                user.TimezoneOffsetMinutes = offsetMinutes;
                await _userRepository.Update(user, ct);
                var s0 = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingWeight, ct);
                _context.Session = s0;
                return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                {
                    Text = _localizer.GetPhrase(WorkflowStep.AwaitingWeight, LocalizationKeysConstants.Profile.AskWeight),
                    ButtonRows = new[] { CancelRow() }
                });
            }

            case WorkflowStep.AwaitingWeight:
            {
                if (!double.TryParse(request.Input.Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var weight))
                {
                    return Result<WorkFlowResponse>.Success(InvalidNumberResponse(WorkflowStep.AwaitingWeight, LocalizationKeysConstants.Profile.AskWeight));
                }

                user.Weight = weight;
                await _userRepository.Update(user, ct);
                var s1 = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingHeight, ct);
                _context.Session = s1;
                return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                {
                    Text = _localizer.GetPhrase(WorkflowStep.AwaitingHeight, LocalizationKeysConstants.Profile.AskHeight),
                    ButtonRows = new[] { CancelRow() }
                });
            }

            case WorkflowStep.AwaitingHeight:
            {
                if (!double.TryParse(request.Input.Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var height))
                {
                    return Result<WorkFlowResponse>.Success(InvalidNumberResponse(WorkflowStep.AwaitingHeight, LocalizationKeysConstants.Profile.AskHeight));
                }

                user.Height = height;
                await _userRepository.Update(user, ct);
                var s2 = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingAge, ct);
                _context.Session = s2;
                return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                {
                    Text = _localizer.GetPhrase(WorkflowStep.AwaitingAge, LocalizationKeysConstants.Profile.AskAge),
                    ButtonRows = new[] { CancelRow() }
                });
            }

            case WorkflowStep.AwaitingAge:
            {
                if (!int.TryParse(request.Input, out var age))
                {
                    return Result<WorkFlowResponse>.Success(InvalidNumberResponse(WorkflowStep.AwaitingAge, LocalizationKeysConstants.Profile.AskAge));
                }

                user.Age = age;
                await _userRepository.Update(user, ct);
                var s3 = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingGender, ct);
                _context.Session = s3;
                return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                {
                    Text = _localizer.GetPhrase(WorkflowStep.AwaitingGender, LocalizationKeysConstants.Profile.AskGender),
                    ButtonRows = new[] { CancelRow() }
                });
            }

            case WorkflowStep.AwaitingGender:
            {
                if (!TryParseGender(request.Input, out var gender))
                {
                    var invalidText = _localizer.GetPhrase(WorkflowStep.AwaitingGender, LocalizationKeysConstants.Profile.InvalidGender);
                    var askText = _localizer.GetPhrase(WorkflowStep.AwaitingGender, LocalizationKeysConstants.Profile.AskGender);
                    return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                    {
                        Text = invalidText + "\n" + askText,
                        ButtonRows = new[] { CancelRow() }
                    });
                }

                user.Gender = gender;
                await _userRepository.Update(user, ct);
                var s4 = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingWaist, ct);
                _context.Session = s4;
                return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                {
                    Text = _localizer.GetPhrase(WorkflowStep.AwaitingWaist, LocalizationKeysConstants.Profile.AskWaist),
                    ButtonRows = new[] { CancelRow() }
                });
            }

            case WorkflowStep.AwaitingWaist:
            {
                if (!double.TryParse(request.Input.Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var waist))
                {
                    return Result<WorkFlowResponse>.Success(InvalidNumberResponse(WorkflowStep.AwaitingWaist, LocalizationKeysConstants.Profile.AskWaist));
                }

                user.WaistCm = waist;
                await _userRepository.Update(user, ct);
                var s5 = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingNeck, ct);
                _context.Session = s5;
                return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                {
                    Text = _localizer.GetPhrase(WorkflowStep.AwaitingNeck, LocalizationKeysConstants.Profile.AskNeck),
                    ButtonRows = new[] { CancelRow() }
                });
            }

            case WorkflowStep.AwaitingNeck:
            {
                if (!double.TryParse(request.Input.Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var neck) || (user.WaistCm is double savedWaist && neck >= savedWaist))
                {
                    return Result<WorkFlowResponse>.Success(InvalidNeckResponse());
                }

                user.NeckCm = neck;
                await _userRepository.Update(user, ct);

                if (user.Gender is Gender.Female)
                {
                    var s6 = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingHips, ct);
                    _context.Session = s6;
                    return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                    {
                        Text = _localizer.GetPhrase(WorkflowStep.AwaitingHips, LocalizationKeysConstants.Profile.AskHips),
                        ButtonRows = new[] { CancelRow() }
                    });
                }

                var s7 = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.Idle, ct);
                _context.Session = s7;
                return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                {
                    Text = _localizer.GetPhrase(WorkflowStep.Idle, LocalizationKeysConstants.Profile.ProfileSaved),
                    ButtonRows = new[] { new ButtonRow (new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack), CallbackPrefixConstants.Back)) }

                });
            }

            case WorkflowStep.AwaitingHips:
            {
                if (!double.TryParse(request.Input.Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var hips))
                {
                    return Result<WorkFlowResponse>.Success(InvalidNumberResponse(WorkflowStep.AwaitingHips, LocalizationKeysConstants.Profile.AskHips));
                }

                user.HipsCm = hips;
                await _userRepository.Update(user, ct);
                var s8 = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.Idle, ct);
                _context.Session = s8;
                return Result<WorkFlowResponse>.Success(new WorkFlowResponse
                {
                    Text = _localizer.GetPhrase(WorkflowStep.Idle, LocalizationKeysConstants.Profile.ProfileSaved),
                    ButtonRows = new[] { new ButtonRow (new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack), CallbackPrefixConstants.Back)) }

                });
            }
        }

        return Result.Failure<WorkFlowResponse>(Errors.InternalError);
    }

    private ButtonRow CancelRow()
    {
        return new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnCancel), CallbackPrefixConstants.Cancel));
    }

    private WorkFlowResponse InvalidNumberResponse(WorkflowStep step, string askKey)
    {
        var invalidText = _localizer.GetPhrase(step, LocalizationKeysConstants.Profile.InvalidNumber);
        var askText = _localizer.GetPhrase(step, askKey);

        return new WorkFlowResponse
        {
            Text = invalidText + "\n" + askText,
            ButtonRows = new[] { CancelRow() }
        };
    }

    private WorkFlowResponse InvalidTimezoneResponse()
    {
        var invalidText = _localizer.GetPhrase(WorkflowStep.AwaitingTimezone, LocalizationKeysConstants.Profile.InvalidTimezone);
        var askText = _localizer.GetPhrase(WorkflowStep.AwaitingTimezone, LocalizationKeysConstants.Profile.AskTimezone);

        return new WorkFlowResponse
        {
            Text = invalidText + "\n" + askText,
            ButtonRows = new[] { CancelRow() }
        };
    }

    private WorkFlowResponse InvalidNeckResponse()
    {
        var invalidText = _localizer.GetPhrase(WorkflowStep.AwaitingNeck, LocalizationKeysConstants.Profile.InvalidNeck);
        var askText = _localizer.GetPhrase(WorkflowStep.AwaitingNeck, LocalizationKeysConstants.Profile.AskNeck);

        return new WorkFlowResponse
        {
            Text = invalidText + "\n" + askText,
            ButtonRows = new[] { CancelRow() }
        };
    }

    private bool TryParseGender(string input, out Gender gender)
    {
        gender = Gender.Unknown;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var normalized = input.Trim().ToLowerInvariant();
        if (normalized == GenderInputConstants.MaleFull || normalized == GenderInputConstants.MaleShortEn || normalized == GenderInputConstants.MaleShortRu || normalized == GenderInputConstants.MaleRu || normalized == GenderInputConstants.MaleOptionNumber)
        {
            gender = Gender.Male;
            return true;
        }

        if (normalized == GenderInputConstants.FemaleFull || normalized == GenderInputConstants.FemaleShortEn || normalized == GenderInputConstants.FemaleShortRu || normalized == GenderInputConstants.FemaleRu || normalized == GenderInputConstants.FemaleOptionNumber)
        {
            gender = Gender.Female;
            return true;
        }

        if (Enum.TryParse<Gender>(normalized, true, out var parsed) && Enum.IsDefined(parsed))
        {
            gender = parsed;
            return true;
        }

        return false;
    }
}
