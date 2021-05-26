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


namespace Plugin.Contato
{
    public class ContactPreUpdate : IPlugin

    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName.ToLower() == "update" && context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
                context.Stage == Convert.ToInt32(MeuEnum.Stage.PreValidation))
            {
                var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var serviceUser = serviceFactory.CreateOrganizationService(context.UserId);
                var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                trace.Trace("Inicio Plugin1");
                Entity entidadeContexto = null;



                if (context.InputParameters.Contains("Target"))
                    entidadeContexto = (Entity)context.InputParameters["Target"];

                if (entidadeContexto.Contains("cr1a4_cpf"))
                {
                    trace.Trace("foi");
                    var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                <attribute name='fullname' />
                                <attribute name='telephone1' />
                                <attribute name='contactid' />
                                <order attribute='fullname' descending='false' />
                                <filter type='and'>
                                   <condition attribute='cr1a4_cpf' operator='eq' value='{entidadeContexto["cr1a4_cpf"]}' />
                                   <condition attribute='contactid' operator='ne' uitype='contact' value='{entidadeContexto.Id}' />
                                  </filter>
                               </entity>
                             </fetch>";

                    //var teste = entidadeContexto.GetAttributeValue<int>("cr1a4_cpf");

                    var Retorno = serviceUser.RetrieveMultiple(new FetchExpression(fetch));




                    if (Retorno != null && Retorno.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("Este CPF já existe no sistema, digite outro cpf!");
                    }
                }
            }


        }
    }
}



