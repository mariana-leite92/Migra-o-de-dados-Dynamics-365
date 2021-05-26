using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Plugin {
    public class contactExcluir : IPlugin {
        public void Execute(IServiceProvider serviceProvider) {



            var context = (IPluginExecutionContext)serviceProvider.
                GetService(typeof(IPluginExecutionContext));


            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.
                GetService(typeof(IOrganizationServiceFactory));
            var serviceAdmin = serviceFactory.CreateOrganizationService(context.UserId);

            var trace = (ITracingService)serviceProvider.
                GetService(typeof(ITracingService));

            EntityReference entidadeContexto = null;

            if (context.InputParameters.Contains("Target")) {
                entidadeContexto = (EntityReference)context.InputParameters["Target"];
            }

        }
    }
}