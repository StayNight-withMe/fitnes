using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Start;

public record StartMessage(long ChatId) : IRequest<Result<WorkFlowResponse>>;
