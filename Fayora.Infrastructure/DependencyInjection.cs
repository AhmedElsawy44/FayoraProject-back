using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Interfaces;
using Fayora.Infrastructure.Persistence;
using Fayora.Infrastructure.Persistence.Repositories;
using Fayora.Infrastructure.Services.Authentication;
using Fayora.Infrastructure.Services.AuthServices;
using Fayora.Infrastructure.Services.AuthServices.FacebookLoginService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
        services.AddScoped<IUserIdentityRepository, UserIdentityRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IUserTokenService, UserTokenService>();
        services.AddSingleton<IVerificationCodeService, VerificationCodeService>();
        services.AddSingleton<ICodeHasher, CodeHasher>();
        services.AddSingleton<ITokenHasher, TokenHasher>();

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<SmsSettings>(configuration.GetSection("SmsSettings"));
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<ISmsService, MockSmsService>();
        services.AddSingleton<IMessageGenerator, MessageGenerator>();
        services.AddSingleton<IWhatsAppService, MockWhatsAppService>();

        // Facebook
        services.Configure<FacebookSettings>(
            configuration.GetSection(FacebookSettings.Section));
        services.AddHttpClient<IFacebookAuthService, FacebookAuthService>();

        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

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