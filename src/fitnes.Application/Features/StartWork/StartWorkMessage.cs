using System;
using System.Collections.Generic;
using System.Text;
using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.StartWork;

public record StartWorkMessage(long ChatId) : IAuthorizedBotRequest, IRequest<Result<WorkFlowResponse>> { }
