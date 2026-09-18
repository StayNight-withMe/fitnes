using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Profile.Survey;

public record UpdateProfileRequest(long ChatId, string Input) : IAuthorizedBotRequest, IRequest<Result<WorkFlowResponse>>;
