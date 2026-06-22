using fitnes.Domain.Entities;

namespace fitnes.Domain.Abstraction.Common;

public interface IRequestContext
{
    UserSession Session { get; set; }
}
