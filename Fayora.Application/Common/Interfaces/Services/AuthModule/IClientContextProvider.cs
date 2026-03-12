using Fayora.Application.Common.Models;

namespace Fayora.Application.Common.Interfaces.Services.AuthServices;

public interface IClientContextProvider
{
    ClientContext GetContext();
}
