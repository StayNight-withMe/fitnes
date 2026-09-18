using System.Globalization;
using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Application.Features.Profile.GetProfile;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Profile;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Profile.EditWeight;

public class UpdateWeightHandler : IRequestHandler<UpdateWeightMessage, Result<WorkFlowResponse>>
{
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly ISessionService _sessionService;
    private readonly IRequestContext _context;
    private readonly IMediator _mediator;

    public UpdateWeightHandler(IBaseRepository<User, long> userRepository, ISessionService sessionService, IRequestContext context, IMediator mediator)
    {
        _userRepository = userRepository;
        _sessionService = sessionService;
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<WorkFlowResponse>> Handle(UpdateWeightMessage request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.ChatId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.UserNotFound);
        }

        if (!double.TryParse(request.Input.Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot), NumberStyles.Any, CultureInfo.InvariantCulture, out var weight))
        {
            return Result.Failure<WorkFlowResponse>(Errors.ValidationError);
        }

        user.Weight = weight;
        await _userRepository.Update(user, cancellationToken);

        var session = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.Idle, cancellationToken);
        _context.Session = session;

        return await _mediator.Send(new GetProfileRequest(request.ChatId), cancellationToken);
    }
}
