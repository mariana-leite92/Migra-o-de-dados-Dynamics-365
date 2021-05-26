using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace CargaItemPedido
{
    class Program
    {
        static void Main(string[] args)
        {
            Conexao conexao = new Conexao();
            var service = conexao.ObterOrigem();
            QueryExpression ConsulOrder = new QueryExpression("salesorder");
            EntityCollection pedidos = service.RetrieveMultiple(ConsulOrder);
            foreach (var pedido in pedidos.Entities)
            {
                Entity salesorderdetail = new Entity("salesorderdetail");
                salesorderdetail["salesorderid"] = pedido.ToEntityReference();
                salesorderdetail["priceperunit"] = new Money(50m);
                salesorderdetail["quantity"] = 10m;
                salesorderdetail["isproductoverridden"] = true;
                salesorderdetail["productdescription"] = "Pacote Office 2020 grupo 6";

                service.Create(salesorderdetail);
            }

        }
    }
}
