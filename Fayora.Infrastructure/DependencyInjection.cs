using Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;
using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Presistances.TouristModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TouCompanyModule;
using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Infrastructure.Persistence.Repositories;
using Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;
using Fayora.Infrastructure.Persistence.Repositories.IdentityModule;
using Fayora.Infrastructure.Persistence.Repositories.TourCompanyModule;
using Fayora.Infrastructure.Persistence.Repositories.TouristModule;
using Fayora.Infrastructure.Persistence.Repositories.TourGuideModule;
using Fayora.Infrastructure.Services.Authentication;
using Fayora.Infrastructure.Services.AuthModule;
using Fayora.Infrastructure.Services.SharedModule;
using Fayora.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Fayora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddAuthentication(configuration)
            .AddPersistence(configuration)
            .AddService(configuration);
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Auth Module
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
        services.AddScoped<IUserIdentityRepository, UserIdentityRepository>();
        services.AddScoped<IVerificationRepository, VerificationRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IHousingUnitRepository, HousingUnitRepository>();
        services.AddScoped<IUnitOwnerRepository, UnitOwnerRepository>();
        services.AddScoped<IMasterInterestRepository, MasterInterestRepository>();
        services.AddScoped<ITouristRepository, TouristRepository>();
        services.AddScoped<IMasterAmenityRepository, MasterAmenityRepository>();

        // Tour Guide Module
        services.AddScoped<ITourGuideRepository, TourGuideRepository>();
        services.AddScoped<ITourGuidePackageRepository, TourGuidePackageRepository>();

        // Tour Company Module
        services.AddScoped<ITourCompanyRepository, TourCompanyRepository>();

        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());


        return services;
    }

    public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IUserTokenService, UserTokenService>();
        services.AddSingleton<IVerificationCodeService, VerificationCodeService>();
        services.AddSingleton<ICodeHasher, CodeHasher>();
        services.AddSingleton<ITokenHasher, TokenHasher>();
        services.AddSingleton<IMessageGenerator, MessageGenerator>();
        services.AddScoped<IUserDeviceManager, UserDeviceManager>();
        services.AddScoped<IAuthTokenGenerator, AuthTokenGenerator>();

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<TwilioSettings>(configuration.GetSection("TwilioSettings"));
        services.Configure<GoogleSettings>(configuration.GetSection("GoogleSettings"));
        services.Configure<FacebookSettings>(configuration.GetSection("FacebookSettings"));

        services.AddScoped<IMessageService, MessageService>();

        services.AddSingleton<IMessageSenderStrategy, SmsSenderStrategy>();
        services.AddSingleton<IMessageSenderStrategy, WhatsAppSenderStrategy>();
        services.AddSingleton<IEmailService, EmailService>();

        services.AddSingleton<ISocialAuthService, SocialAuthService>();
        services.AddHttpClient<ISocialAuthStrategy, FacebookAuthStrategy>();
        services.AddSingleton<ISocialAuthStrategy, GoogleAuthStrategy>();
        services.AddSingleton<ISocialAuthStrategy, MockAppleAuthService>();


        services.AddScoped<IFileStorageService, LocalFileService>();

        return services;
    }

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.Section, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            });



        return services;
    }
}