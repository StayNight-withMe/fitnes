using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using L = fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Language.SetLanguage;

public record SetLanguageMessage(long ChatId, L.Language Language) : IRequest<Result<WorkFlowResponse>>, IAuthorizedBotRequest { }
