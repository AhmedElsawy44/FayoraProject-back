using Fayora.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IUserRepository
{
    Task<bool> IsEmailExistAsync(string email);
    Task<bool> IsPhoneExistAsync(string phoneNumber);
    Task AddUserAsync(User user);
}
