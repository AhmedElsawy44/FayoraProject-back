using Fayora.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IUserRepository
{
    Task<bool> IsEmailExistAsync(string email, CancellationToken cancellationToken);
    Task<bool> IsPhoneExistAsync(string phoneNumber, CancellationToken cancellationToken);
    Task AddUserAsync(User user, CancellationToken cancellationToken);
}
