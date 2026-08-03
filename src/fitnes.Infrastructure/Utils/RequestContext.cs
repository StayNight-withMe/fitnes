using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Entities;

namespace fitnes.Infrastructure.Utils;

public class RequestContext : IRequestContext
{
    public UserSession Session { get; set; } = default!;
}
