using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Profile.StartSurvey;

public record StartSurveyPromptRequest(long ChatId) : IAuthorizedBotRequest, IRequest<Result<WorkFlowResponse>>;
