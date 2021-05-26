using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using PluginGeraldeli;

namespace Plugin
{
    public class AccountPostOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.
                GetService(typeof(IPluginExecutionContext));

            if (context.MessageName.ToLower() =="update" && 
               context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) && 
               context.Stage == Convert.ToInt32(MeuEnum.Stage.PostOperation) )
            {
                var serviceFactory = (IOrganizationServiceFactory)serviceProvider.
                    GetService(typeof(IOrganizationServiceFactory));

                var serviceAdmin = serviceFactory.CreateOrganizationService(null);

                var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                trace.Trace("Inicio Plugin");

                Entity entidadeContexto = null;
                Entity entidadePre = null;

                if (context.InputParameters.Contains("Target"))
                    entidadeContexto = (Entity)context.InputParameters["Target"];

                if (context.PreEntityImages.Contains("preImagem"))
                    entidadePre = (Entity)context.PreEntityImages["preImagem"];

                if (entidadeContexto == null || entidadePre == null)
                    return;

                if (entidadeContexto.Contains("primarycontactid") && entidadePre.Contains("primarycontactid")
                    && (entidadeContexto["primarycontactid"]) != entidadePre["primarycontactid"])
                    throw new InvalidPluginExecutionException(MontarMensagemErro("Não é possível alterar o contato principal"));
            }
        }

        public static string MontarMensagemErro(string str)
        {
            return @"<img height='16px' src='https://www.thompsonsurfschool.com/wp-content/uploads/2018/02/ConfusedEmoji.jpg'> 
                    <strong>Oh Oh! " + str + "</strong> </br></br>";
        }
    }
}
