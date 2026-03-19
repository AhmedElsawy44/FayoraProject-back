using Fayora.Application.Common.Models;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IClientContextProvider
{
    ClientContext GetContext();
}
