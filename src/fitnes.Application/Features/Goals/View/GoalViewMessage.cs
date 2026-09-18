using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Goals.View;

public record GoalViewMessage(long ChatId) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest;
