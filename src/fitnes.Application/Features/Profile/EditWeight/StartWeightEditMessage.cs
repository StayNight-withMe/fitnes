using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Profile.EditWeight;

public record StartWeightEditMessage(long ChatId) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest;
