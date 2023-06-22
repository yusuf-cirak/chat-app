using System.Reflection;
using Application.Common.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddBusinessRuleServices(this IServiceCollection services,Func<IServiceCollection,Type,IServiceCollection>? addWithLifeCycle=null)
    {
        var typeOfBusinessRules = typeof(BaseBusinessRules);
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeOfBusinessRules) && typeOfBusinessRules != t);

        if (addWithLifeCycle is null)
        {
            foreach (Type businessRuleType in types)
            {
                services.AddScoped(businessRuleType);
            }
        }
        else
        {
            foreach (Type businessRuleType in types)
            {
                addWithLifeCycle(services, businessRuleType);
            }
        }
        
        return services;

    }
    
    public static void AddApplicationServices(this IServiceCollection services)
    {
        AddBusinessRuleServices(services);

        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(e=>e.RegisterServicesFromAssembly(executingAssembly));
    }
    
}