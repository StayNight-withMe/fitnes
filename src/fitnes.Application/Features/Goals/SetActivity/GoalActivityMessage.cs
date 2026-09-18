using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Goals.SetActivity;

public record GoalActivityMessage(long ChatId, ActivityLevel Activity, GoalType Type, double TargetWeight) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest;
