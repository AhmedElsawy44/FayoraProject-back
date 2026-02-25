using Fayora.Application.Common.Authentication;
using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Errors;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands;

public class RegisterCommandHandler(IPasswordHasher passwordHasher, IRefreshTokenService refreshTokenService, IJwtTokenService jwtTokenService, IUserRepository userRepository, IRefreshTokensRepository refreshTokensRepository, IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (request.Email != null)
        {
            if (await userRepository.IsEmailExistAsync(request.Email))
            {
                return UserErrors.EmailAlreadyExists;
            }
        }
        else
        {
            if (request.PhoneNumber != null)
            {
                if (await userRepository.IsPhoneExistAsync(request.PhoneNumber))
                {
                    return UserErrors.PhoneAlreadyExists;
                }
            }
        }

        var passwordHash = passwordHasher.Hash(request.Password);

        var userResult = User.Create(request.FirstName, request.LastName, request.Email, request.PhoneNumber, passwordHash, request.SimCountryIsoCode, request.PreferredLanguage, request.TimeZone);

        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var user = userResult.Value;
        var refreshToken = refreshTokenService.GenerateToken();
        var jwtToken = jwtTokenService.GenerateToken(user);

        await refreshTokensRepository.AddTokenAsync(refreshToken);
        await userRepository.AddUserAsync(userResult.Value);
        await unitOfWork.CommitChangesAsync();

        return new AuthResult(user.Id, user.FirstName, user.LastName, user.PrimaryEmail?.Value, user.PhoneNumber, jwtToken, refreshToken.Token);
    }
}
