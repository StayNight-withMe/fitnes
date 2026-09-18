using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Calories.CalculateCalories;

public record CalculateCaloriesMessage(byte[] ImageBytes, double Weight, string? AdditionalInfo) : IRequest<Result<WorkFlowResponse>>;
