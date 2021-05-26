using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace RoboDeImportacao
{
    class Program
    {

        static void Main(string[] args)
        {
            Conexao conexao = new Conexao();


            //CRM de Origem
            var serviceProxyOrigem = conexao.ObterOrigem();
            var serviceProxyDestino = conexao.ObterDestino();

            //Criar5MilContasDestino(serviceProxyDestino);

            //EntityCollection contatosOrigem = RetornarMultiploContato(serviceProxyOrigem);
            EntityCollection contatosOrigem = RetornaRegistros(serviceProxyOrigem, "contact", new ConditionExpression("cr5d5_integrar", ConditionOperator.Equal, true));
            CriarContatos(serviceProxyOrigem, serviceProxyDestino, contatosOrigem);

            EntityCollection contasOrigem = RetornaRegistros(serviceProxyOrigem, "account", new ConditionExpression("cr5d5_integrar", ConditionOperator.Equal, true));
            CriarContas(serviceProxyOrigem, serviceProxyDestino, contasOrigem);

            EntityCollection leadOrigem = RetornaRegistros(serviceProxyOrigem, "lead", new ConditionExpression("cr5d5_integrar", ConditionOperator.Equal, true));
            Criarlead(serviceProxyOrigem, serviceProxyDestino, leadOrigem);

            EntityCollection OrdensOrigem = RetornaRegistros(serviceProxyOrigem, "salesorder", new ConditionExpression("cr5d5_integrar", ConditionOperator.Equal, true));
            CriarOrdem(serviceProxyOrigem, serviceProxyDestino, OrdensOrigem);

        }

        static EntityCollection RetornaRegistros(CrmServiceClient service, string entidade, ConditionExpression condicao)
        {
            QueryExpression queryExpression = new QueryExpression(entidade);
            queryExpression.ColumnSet = new ColumnSet(true);
            queryExpression.Criteria.AddCondition(condicao);
            return service.RetrieveMultiple(queryExpression);
        }

        static void CriarContatos(CrmServiceClient serviceProxyOrigem, CrmServiceClient serviceProxyDestino, EntityCollection contatosOrigem)
        {

            foreach (var contato in contatosOrigem.Entities)
            {
                RemoveCamposDesnecessarios(contato);
                try
                {
                    try
                    {
                        serviceProxyDestino.Retrieve("contact", contato.Id, new ColumnSet(true));
                        serviceProxyDestino.Update(contato);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        serviceProxyDestino.Create(contato);
                    }

                    Entity contatoOrigem = new Entity("contact", contato.Id);
                    contatoOrigem["cr5d5_integrar"] = false;
                    serviceProxyOrigem.Update(contatoOrigem);
                }
                catch (Exception ex1)
                {
                    Entity Log = new Entity("cr5d5_logdeintegracao");
                    Log["cr5d5_name"] = "Erro na integracao do contato";
                    Log["cr5d5_erro"] = ex1.Message.Length > 2000 ? ex1.Message.Substring(0, 1999) : ex1.Message;
                    Log["cr5d5_contato"] = contato.ToEntityReference();
                    serviceProxyOrigem.Create(Log);
                }
            }
        }

        static void CriarContas(CrmServiceClient serviceProxyOrigem, CrmServiceClient serviceProxyDestino, EntityCollection contasOrigem)
        {


            foreach (var conta in contasOrigem.Entities)
            {
                try
                {

                    RemoveCamposDesnecessarios(conta);

                    try
                    {
                        serviceProxyDestino.Retrieve("account", conta.Id, new ColumnSet(true));
                        serviceProxyDestino.Update(conta);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        serviceProxyDestino.Create(conta);
                    }

                    Entity contaorigem = new Entity("account", conta.Id);
                    contaorigem["cr5d5_integrar"] = false;
                    serviceProxyOrigem.Update(contaorigem);
                }

                catch (Exception ex1)
                {
                    Entity Log = new Entity("cr5d5_logdeintegracao");
                    Log["cr5d5_name"] = "Erro na integracao da conta";
                    Log["cr5d5_erro"] = ex1.Message.Length > 2000 ? ex1.Message.Substring(0, 1999) : ex1.Message;
                    Log["cr5d5_conta"] = conta.ToEntityReference();
                    serviceProxyOrigem.Create(Log);
                }


            }
        }

        static void Criarlead(CrmServiceClient serviceProxyOrigem, CrmServiceClient serviceProxyDestino, EntityCollection LeadsOrigem)
        {
            foreach (var lead in LeadsOrigem.Entities)
            {
                try
                {
                    RemoveCamposDesnecessarios(lead);

                    try
                    {
                        serviceProxyDestino.Retrieve("lead", lead.Id, new ColumnSet(true));
                        serviceProxyDestino.Update(lead);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        serviceProxyDestino.Create(lead);
                    }

                    Entity leadorigem = new Entity("lead", lead.Id);
                    leadorigem["cr5d5_integrar"] = false;
                    serviceProxyOrigem.Update(leadorigem);
                }
                catch (Exception ex1)
                {
                    Entity Log = new Entity("cr5d5_logdeintegracao");
                    Log["cr5d5_name"] = "Erro na integracao do Cliente Potencial(lead)";
                    Log["cr5d5_erro"] = ex1.Message.Length > 2000 ? ex1.Message.Substring(0, 1999) : ex1.Message;
                    Log["cr5d5_lead"] = lead.ToEntityReference();
                    serviceProxyOrigem.Create(Log);
                }

            }
        }

        static void CriarOrdem(CrmServiceClient serviceProxyOrigem, CrmServiceClient serviceProxyDestino, EntityCollection SalesOrderOrigem)
        {
            foreach (var pedido in SalesOrderOrigem.Entities)
            {
                try
                {
                    RemoveCamposDesnecessarios(pedido);
                    EntityCollection ProdutosDaOrdem = RetornaRegistros(serviceProxyOrigem, "salesorderdetail", new ConditionExpression("salesorderid", ConditionOperator.Equal, pedido.Id));
                    if (pedido.Contains("cr5d5_codigo"))
                    {
                        pedido["cr1a4_codigo"] = pedido["cr5d5_codigo"];
                        pedido.Attributes.Remove("cr5d5_codigo");
                    }
                    try
                    {
                        serviceProxyDestino.Retrieve("salesorder", pedido.Id, new ColumnSet(true));
                        serviceProxyDestino.Update(pedido);
                        EntityCollection ProdutosDaOrdemDestino = RetornaRegistros(serviceProxyDestino, "salesorderdetail", new ConditionExpression("salesorderid", ConditionOperator.Equal, pedido.Id));
                        foreach (Entity ProdutoDaOrdem in ProdutosDaOrdemDestino.Entities)
                        {
                            serviceProxyDestino.Delete("salesorderdetail", ProdutoDaOrdem.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        serviceProxyDestino.Create(pedido);
                    }

                    foreach (Entity ProdutoDaOrdem in ProdutosDaOrdem.Entities)
                    {
                        RemoveCamposDesnecessarios(ProdutoDaOrdem);
                        if (ProdutoDaOrdem.Contains("productid"))
                        {
                            Entity produto = serviceProxyOrigem.Retrieve("product", ((EntityReference)ProdutoDaOrdem["productid"]).Id, new ColumnSet("name"));
                            ProdutoDaOrdem["productdescription"] = produto["name"];
                            ProdutoDaOrdem["isproductoverridden"] = true;
                            ProdutoDaOrdem.Attributes.Remove("productid");
                        }

                        serviceProxyDestino.Create(ProdutoDaOrdem);
                    }

                    Entity OrderOrigem = new Entity("salesorder", pedido.Id);
                    OrderOrigem["cr5d5_integrar"] = false;
                    serviceProxyOrigem.Update(OrderOrigem);
                }
                catch (Exception ex1)
                {
                    Entity Log = new Entity("cr5d5_logdeintegracao");
                    Log["cr5d5_name"] = "Erro na integracao da Ordem";
                    Log["cr5d5_erro"] = ex1.Message.Length > 2000 ? ex1.Message.Substring(0, 1999) : ex1.Message;
                    Log["cr5d5_salesorder"] = pedido.ToEntityReference();
                    serviceProxyOrigem.Create(Log);
                }

            }
        }

        private static void RemoveCamposDesnecessarios(Entity entidade)
        {

            if (entidade.Contains("cr5d5_integrar"))
                entidade.Attributes.Remove("cr5d5_integrar");
            if (entidade.Contains("transactioncurrencyid"))
                entidade.Attributes.Remove("transactioncurrencyid");
            if (entidade.Contains("owninguser"))
                entidade.Attributes.Remove("owninguser");
            if (entidade.Contains("ownerid"))
                entidade.Attributes.Remove("ownerid");
            if (entidade.Contains("createdby"))
                entidade.Attributes.Remove("createdby");
            if (entidade.Contains("modifiedby"))
                entidade.Attributes.Remove("modifiedby");
            if (entidade.Contains("parentcustomerid"))
                entidade.Attributes.Remove("parentcustomerid");
            if (entidade.Contains("campaignid"))
                entidade.Attributes.Remove("campaignid");
            if (entidade.Contains("uomid"))
                entidade.Attributes.Remove("uomid");
        }

        //static void Criar5MilContasDestino(CrmServiceClient proxy)
        //{

        //    for (int i = 0; i < 10; i++)
        //    {
        //        Entity conta = new Entity("account");
        //        conta["name"] = $"Mariana Leite{i}";
        //        //conta["lastname"] = "leite" + i;
        //        conta["emailaddress1"] = $"mariana.leite{i}@teste.com";
        //        proxy.Create(conta);
        //    }
        //}
    }
}
