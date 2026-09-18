using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Application.Features.StartWork;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.MoveBack;

public class MoveBackHandler : IRequestHandler<MoveBackMessage, Result<WorkFlowResponse>>
{
    private readonly IRequestContext _context;
    private readonly IMediator _mediator;

    public MoveBackHandler(IMediator mediator, IRequestContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task<Result<WorkFlowResponse>> Handle(MoveBackMessage request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new StartWorkMessage(_context.Session.Id));
    }
}
