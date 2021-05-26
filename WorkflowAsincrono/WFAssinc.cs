using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace WorkflowAsincronoGeraldeli
{
    public class WFAssincGeraldeli : CodeActivity {
        #region Parametros
        [Input("Nome Contato")]
        public InArgument<string> nomeContato { get; set; }

        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> usuarioEntrada { get; set; }

        [Input("Conta")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> conta { get; set; }

        [Output("Saida")]        
        public OutArgument<string> saida { get; set; }
        #endregion
        protected override void Execute(CodeActivityContext context) {
            IWorkflowContext contextoWorkFlow = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();            
            ITracingService trace = context.GetExtension<ITracingService>();

            trace.Trace("Inicio do WorkFlow");
            Guid usuario = Guid.Empty;
            Guid account = Guid.Empty;

            if (usuarioEntrada.Get<EntityReference>(context) != null) {
                usuario = usuarioEntrada.Get<EntityReference>(context).Id;
            } else {
                usuario = contextoWorkFlow.InitiatingUserId;
            }

            Entity entidade = new Entity("contact");
            entidade["firstname"] = nomeContato.Get<string>(context);
            entidade["lastname"] = nomeContato.Get<string>(context);

            if (conta.Get<EntityReference>(context) != null) {
                account = conta.Get<EntityReference>(context).Id;
                entidade["parentcustomerid"] = new EntityReference("account", account);
            }
            IOrganizationService usuarioService = serviceFactory.CreateOrganizationService(usuario);            
            trace.Trace("Guid Usuário " + usuario.ToString());
            usuarioService.Create(entidade);
            trace.Trace("Contato criado");
            saida.Set(context, "Teste realizado com sucesso!");
            trace.Trace("Final do WorkFlow");
        }
    }
}
