using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginGeraldeli
{
    public class AccountPreValidationGeraldeli : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {           
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.
                GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.
                GetService(typeof(IOrganizationServiceFactory));           

            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity entidadeContexto = null;

            if (context.InputParameters.Contains("Target"))
                entidadeContexto = (Entity)context.InputParameters["Target"];

            if (entidadeContexto == null)
                return;

            if (!entidadeContexto.Contains("primarycontactid"))
                throw new Exception("Contato relacionado obrigatório! -- Geraldeli");

        }
    }
}
