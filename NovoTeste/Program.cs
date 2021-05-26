using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NovoTeste {
    class Program {
        static void Main(string[] args) {
            var connectionStringCRM = @"AuthType=OAuth;
            Username = admin@fyi2021.onmicrosoft.com;
            Password = Abcd@1234; SkipDiscovery = True;
            AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
            RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
            Url = https://fyi2021.api.crm2.dynamics.com/XRMServices/2011/Organization.svc;";

            var p = Provider(connectionStringCRM);


            QueryExpression queryExpression = new QueryExpression("account");

            queryExpression.Criteria.AddCondition("name", ConditionOperator.Equal, "teste");
            queryExpression.ColumnSet = new ColumnSet(true);
            EntityCollection colecaoEntidades = p.RetrieveMultiple(queryExpression);
            foreach (var item in colecaoEntidades.Entities) {
                Console.WriteLine(item["name"]);
            }


        }
        

       
    }
}
