using Fayora.Application.Common.Models;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IClientContextProvider
{
    ClientContext GetContext();
}
