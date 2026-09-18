using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Meals.MealStats;

public record MealStatsMessage(long ChatId, StatsGranularity Granularity, int Offset) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest;
