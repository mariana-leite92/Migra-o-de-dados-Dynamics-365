using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using PluginMarianaLeite;

namespace Plugin.Ordem
{
    public class OrderPreCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)


        {

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));




            if (context.MessageName.ToLower() == "create" && context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
                context.Stage == Convert.ToInt32(MeuEnum.Stage.PreValidation))
            {
                var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var serviceUser = serviceFactory.CreateOrganizationService(context.UserId);
                var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                trace.Trace("Inicio Plugin");
                Entity entidadeContexto = null;


                if (context.InputParameters.Contains("Target"))
                    entidadeContexto = (Entity)context.InputParameters["Target"];


                if (entidadeContexto.LogicalName == "salesorder")
                {

                    var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                   <entity name='salesorder'>
                                <attribute name='name' />
                                <attribute name='customerid' />
                                <attribute name='statuscode' />
                                <attribute name='totalamount' />
                                <attribute name='salesorderid' />
                                <order attribute='name' descending='false' />
                              <filter type='and'>
                            <condition attribute='cr1a4_codigo' operator='eq' value='{entidadeContexto["cr1a4_codigo"]}' />
                              </filter>
                            </entity>
                        </fetch>";
                        

                    var Retorno = serviceUser.RetrieveMultiple(new FetchExpression(fetch));

                    if (Retorno.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("Este pedido já foi cadastrado no sistema");
                    }

                }
            }
        }
    }
}
    
 
