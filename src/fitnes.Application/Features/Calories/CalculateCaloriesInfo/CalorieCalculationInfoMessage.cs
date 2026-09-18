using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Calories.CalculateCaloriesInfo;

public record CalorieCalculationInfoMessage(long ChatId) : IAuthorizedBotRequest, IRequest<Result<WorkFlowResponse>> { }
