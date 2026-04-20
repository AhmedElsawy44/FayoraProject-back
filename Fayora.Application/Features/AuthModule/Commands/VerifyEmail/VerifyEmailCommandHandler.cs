<<<<<<< HEAD
using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
=======
﻿using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
>>>>>>> f50de591342cfd91cb5e500935cbeb8d57ede447
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
<<<<<<< HEAD
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;
=======
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;
>>>>>>> f50de591342cfd91cb5e500935cbeb8d57ede447

namespace Fayora.Application.Features.AuthModule.Commands.VerifyEmail;

public class VerifyEmailCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator,
    ICodeHasher codeHasher,
    IUnitOfWork unitOfWork)
    : ICommandHandler<VerifyEmailCommand, Result<VerifyEmailResult>>
{
    public async Task<Result<VerifyEmailResult>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        Language? languageEnum = null;
        if (!Enum.TryParse<Language>(request.DeviceLanguage, true, out var parsedLanguage))
            return AuthErrors.InvalidLanguage;
        languageEnum = parsedLanguage;

        var user = await userRepository.GetUserByEmailAsync(
            request.Email,
            new UserQueryOptions { IsReadOnly = false },
            cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (user.IsVerified) return AuthErrors.EmailIsAlreadyVerified;

        var registerOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id,
            request.Email,
            CodePurpose.VerifyAccount,
            cancellationToken);

        if (registerOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = registerOtp.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        user.VerifyEmail();
        user.UpdateRegionalPreferences(request.SimCountryIsoCode, languageEnum.Value, request.TimeZone);
        user.Login();

        await userDeviceManager.UpsertDeviceAsync(
            user.Id,
            request.DeviceId,
            request.FcmToken,
            languageEnum.Value,
            cancellationToken);

        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new VerifyEmailResult(
            user.Id,
            user.FirstName,
            user.LastName,
            request.Email,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}