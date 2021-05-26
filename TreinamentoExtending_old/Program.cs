using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Net;

namespace TreinamentoExtending
{
    class Program
    {
        static void Main(string[] args)
        {

            Conexao conexao = new Conexao();
            var serviceProxy = conexao.Obter();

            //RetornarMultiplo(serviceProxy);
            ////FetchXMLAgregate(serviceProxy);

            //// CriarEntidade(serviceProxy);
            ////CriarAtributo(serviceProxy);
            //ConsultasLinq(serviceProxy);
            ////Teste(serviceProxy);

            //Descoberta();

            Create(serviceProxy);

            Console.ReadKey();
        }

        static void Create(OrganizationServiceProxy serviceProxy)
        {
            for (int i = 0; i < 10; i++)
            {
                Guid registro = new Guid();
                Entity entidade = new Entity("account");
                entidade.Attributes.Add("name", "Treinamento " + i.ToString());

                registro = serviceProxy.Create(entidade);

                if (registro != Guid.Empty)
                    Console.WriteLine("Id do Registro criado : " + registro);
                else
                    Console.WriteLine("Registro não criado!");
            }
        }


        #region Descoberta
        static void Descoberta()
        {
            Uri local = new Uri("https://disco.crm2.dynamics.com/XRMServices/2011/Discovery.svc");

            ClientCredentials clientcred = new ClientCredentials();
            clientcred.UserName.UserName = "adm@academia2019noturna.onmicrosoft.com";
            clientcred.UserName.Password = "Abcd@1234";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            DiscoveryServiceProxy dsp = new DiscoveryServiceProxy(local, null, clientcred, null);
            dsp.Authenticate();


            RetrieveOrganizationsRequest rosreq = new RetrieveOrganizationsRequest();
            rosreq.AccessType = EndpointAccessType.Default;
            rosreq.Release = OrganizationRelease.Current;

            RetrieveOrganizationsResponse r = (RetrieveOrganizationsResponse)dsp.Execute(rosreq);

            foreach (var item in r.Details)
            {
                Console.WriteLine("Unique " + item.UniqueName);
                Console.WriteLine("Friendly " + item.FriendlyName);
                foreach (var endpoint in item.Endpoints)
                {
                    Console.WriteLine(endpoint.Key);
                    Console.WriteLine(endpoint.Value);
                }
            }
            Console.ReadKey();
        }
        #endregion


        #region QueryExpression

        static void RetornarMultiplo(OrganizationServiceProxy serviceProxy)
        {
            QueryExpression queryExpression = new QueryExpression("account");

            queryExpression.Criteria.AddCondition("name", ConditionOperator.BeginsWith, "teste");
            queryExpression.ColumnSet = new ColumnSet(true);
            EntityCollection colecaoEntidades = serviceProxy.RetrieveMultiple(queryExpression);
            foreach (var item in colecaoEntidades.Entities)
            {
                Console.WriteLine(item["name"]);
            }
        }


        static void QueryExpression(OrganizationServiceProxy serviceProxy)
        {
            QueryExpression queryExpression = new QueryExpression("account");
            queryExpression.ColumnSet = new ColumnSet(true);            

            ConditionExpression condicao = new ConditionExpression("address1_city", ConditionOperator.Equal, "Natal");
            queryExpression.Criteria.AddCondition(condicao);

            LinkEntity link = new LinkEntity("account", "contact", "primarycontactid", "contactid", JoinOperator.Inner);
            link.Columns = new ColumnSet("firstname", "lastname");
            link.EntityAlias = "Contato";

            queryExpression.LinkEntities.Add(link);

            EntityCollection colecaoEntidades = serviceProxy.RetrieveMultiple(queryExpression);
            foreach (var item in colecaoEntidades.Entities)
            {
                Console.WriteLine("Nome conta " + item["name"]);
                Console.WriteLine("Nome Contato " + ((AliasedValue)item["Contato.firstname"]).Value);
                Console.WriteLine("Sobrenome Contato " + ((AliasedValue)item["Contato.lastname"]).Value);
            }
        }
        #endregion


        #region Linq

        static void ConsultasLinq(OrganizationServiceProxy serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);

            var resultados = from a in context.CreateQuery("contact")
                             join b in context.CreateQuery("account")
                                    on a["contactid"] equals b["primarycontactid"]
                             //where a["contactid"].ToString().Contains("en")
                             select new
                             {
                                 Contact = new
                                 {
                                     FirstName = a["firstname"],
                                     LastName = a["lastname"]
                                 }
                             };

            foreach (var item in resultados)
            {
                Console.WriteLine("Nome : " + item.Contact.FirstName);
                Console.WriteLine("Nome : " + item.Contact.LastName);
            }
        }


        static void CriacaoLinq(OrganizationServiceProxy serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);
            Entity account = new Entity("account");
            account["name"] = "Treinamento Extending";

            context.AddObject(account);
            context.SaveChanges();
        }
        static void UpdateLinq(OrganizationServiceProxy serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);
            var resultados = from a in context.CreateQuery("contact")
                             where ((string)a["firstname"]) == "Dan"
                             select a;

            foreach (var item in resultados)
            {                
                item.Attributes["firstname"] = "Daniel Geraldeli";
                context.UpdateObject(item);
            }
            context.SaveChanges();
        }

        static void ExcluirLinq(OrganizationServiceProxy serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);

            var resultados = from a in context.CreateQuery("account")
                             where (string)a["name"] == "Treinamento Extending 2"
                             select a;

            foreach (var item in resultados)
            {
                context.DeleteObject(item);
            }

            context.SaveChanges();
        }

        #endregion


        #region FetchXML
        static void FetchXMLAgregate(OrganizationServiceProxy serviceProxy)
        {

            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                              <entity name='opportunity'>
                                <attribute name='budgetamount' alias='budgetamount_soma' aggregate='avg'/>
                              </entity>
                            </fetch>";

            EntityCollection colecao = serviceProxy.RetrieveMultiple(new FetchExpression(query));

            foreach (var item in colecao.Entities)
            {
                Console.WriteLine(((Money)((AliasedValue)item["budgetamount_soma"]).Value).Value.ToString());
            }
        }

        static void FetchXML(OrganizationServiceProxy serviceProxy)
        {
            string query = @"";


            EntityCollection colecao = serviceProxy.RetrieveMultiple(new FetchExpression(query));

            foreach (var item in colecao.Entities)
            {
                Console.WriteLine(item["name"]);
            }

        }
        #endregion


        static void ExecuteAssign(OrganizationServiceProxy serviceProxy)
        {

            EntityReference fonte = new EntityReference("systemuser", Guid.Empty);

            EntityReference Alvo = new EntityReference("account", Guid.Empty);

            AssignRequest assignRequest = new AssignRequest();

            assignRequest.Assignee = fonte;
            assignRequest.Target = Alvo;

            AssignResponse resposta = (AssignResponse)serviceProxy.Execute(assignRequest);
        }

        static void CriacaoRetornoAtualizacaoDelete(OrganizationServiceProxy serviceProxy)
        {            
            for (int i = 0; i < 10; i++)
            {
                Entity entidade = new Entity("account");
                Entity RegistroResposta = new Entity();
                Guid registro = new Guid();                
              
                entidade.Attributes.Add("name", "Treinamento " + i.ToString());

                registro = serviceProxy.Create(entidade);

                RegistroResposta.LogicalName = "account";
                RegistroResposta.Id = registro;
                RegistroResposta.Attributes.Add("name", "meu valor");
                //RegistroResposta = serviceProxy.Retrieve("account", registro, new ColumnSet(true));
                //if (RegistroResposta.Attributes.Contains("name"))
                //    RegistroResposta.Attributes["name"] = "Treinamento " + (i + 1).ToString();
                //else
                //    RegistroResposta.Attributes.Add("name", "meu valor");

                serviceProxy.Update(RegistroResposta);

                serviceProxy.Delete("account", RegistroResposta.Id);

                Console.WriteLine("Registro Deletado: "+ registro.ToString());
            }
        }


        #region Metadados
        static void CriarEntidade(OrganizationServiceProxy serviceProxy)
        {
            CreateEntityRequest createrequest = new CreateEntityRequest
            {

                //Define the entity
                Entity = new EntityMetadata
                {
                    SchemaName = "new_bankaccount",
                    DisplayName = new Label("Bank Account", 1033),
                    DisplayCollectionName = new Label("Bank Accounts", 1033),
                    Description = new Label("An entity to store information about customer bank accounts", 1033),
                    OwnershipType = OwnershipTypes.UserOwned,
                    IsActivity = false,

                },

                // Define the primary attribute for the entity
                PrimaryAttribute = new StringAttributeMetadata
                {
                    SchemaName = "new_accountname",
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = 100,
                    FormatName = StringFormatName.Text,
                    DisplayName = new Label("Account Name", 1033),
                    Description = new Label("The primary attribute for the Bank Account entity.", 1033)
                },

            };
            serviceProxy.Execute(createrequest);
            Console.WriteLine("Entidade criada com sucesso.");
        }

        static void CriarAtributo(OrganizationServiceProxy serviceProxy)
        {
            CreateAttributeRequest createCheckedDateRequest = new CreateAttributeRequest
            {
                EntityName = "new_bankaccount",
                Attribute = new DateTimeAttributeMetadata
                {
                    SchemaName = "new_checkeddate",
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    Format = DateTimeFormat.DateOnly,
                    DisplayName = new Label("Date", 1033),
                    Description = new Label("The date the account balance was last confirmed", 1033)

                }
            };
            serviceProxy.Execute(createCheckedDateRequest);
            Console.WriteLine("Campo criado com sucesso"); 
        }


        static void CriacaoExecuteMultipleRequest(OrganizationServiceProxy serviceProxy)
        {
            Entity entidade = new Entity("account");
            Entity RegistroResposta = new Entity();
            Guid registro = Guid.Empty;

            ExecuteMultipleRequest multiple = new Microsoft.Xrm.Sdk.Messages.ExecuteMultipleRequest();
            multiple.Settings = new ExecuteMultipleSettings();
            multiple.Settings.ContinueOnError = true;
            multiple.Settings.ReturnResponses = true;
            multiple.Requests = new OrganizationRequestCollection();
            for (int i = 0; i < 10; i++)
            {
                registro = new Guid();
                entidade = new Entity("account");
                entidade.Attributes.Add("name", "Treinamento " + i.ToString());

                CreateRequest createRequest = new CreateRequest();
                createRequest.Target = entidade;

                multiple.Requests.Add(createRequest);
            }

            ExecuteMultipleResponse execute = (ExecuteMultipleResponse)serviceProxy.Execute(multiple);

            foreach (var item in execute.Results)
            {
                Console.WriteLine(item.Key);
                Console.Write(item.Value);
            }
        }
        #endregion
    }
}
