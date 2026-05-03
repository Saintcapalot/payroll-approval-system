using PayrollApprovalSystem.Api.Services;
using PayrollApprovalSystem.Application.Services;

namespace PayrollApprovalSystem.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<PayrollCalculationService>();
        services.AddScoped<PayrollGenerationService>();
        services.AddScoped<ApprovalService>();
        services.AddScoped<PayslipService>();
        services.AddScoped<PayslipPdfService>();

        return services;
    }
}
