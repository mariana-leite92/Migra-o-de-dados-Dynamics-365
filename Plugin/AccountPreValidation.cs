using Microsoft.Xrm.Sdk;
using System;

namespace Plugin_SeuNome {
    public class AccountPreValidation_SeuNome : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity entidadeContexto = null;

            if (context.InputParameters.Contains("Target"))
                entidadeContexto = (Entity)context.InputParameters["Target"];
            else
                return;

            if (!entidadeContexto.Contains("primarycontactid"))
                throw new InvalidPluginExecutionException("Contato principal obrigatório! -- Geraldeli");

        }
    }
}
