using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Profile.EditWeight;

public record UpdateWeightMessage(long ChatId, string Input) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest;
