using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.MoveBack;

public record MoveBackMessage(long ChatId) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest { }  
