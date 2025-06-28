using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

using System.Reflection;
using Joblin.Controllers;

namespace Joblin.Infrastructure;

public class JoblinControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var controllerType = typeof(JoblinWebhookController).GetTypeInfo();
        if (!feature.Controllers.Contains(controllerType))
        {
            feature.Controllers.Add(controllerType);
        }
    }
}
