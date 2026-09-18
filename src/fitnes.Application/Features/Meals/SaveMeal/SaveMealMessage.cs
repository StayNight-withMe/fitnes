using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Meals.SaveMeal;

public record SaveMealMessage(long ChatId, Guid AnalysisResultId) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest;
