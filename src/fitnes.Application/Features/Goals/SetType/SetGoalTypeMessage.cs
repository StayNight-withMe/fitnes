using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Goals.SetType;

public record SetGoalTypeMessage(long ChatId, GoalType Type, double TargetWeight) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest;
