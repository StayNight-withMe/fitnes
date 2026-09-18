using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Goals.New;

public record GoalNewMessage(long ChatId) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest;
