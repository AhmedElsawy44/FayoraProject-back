using Fayora.Application.Common.Models;

namespace Fayora.Application.Common.Interfaces;

public interface IClientContextProvider
{
    ClientContext GetContext();
}
