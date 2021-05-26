using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace TreinamentoExtending
{
    class Conexao
    {
        public OrganizationServiceProxy Obter()
        {
            Uri uri = new Uri("https://alfapeople4.api.crm.dynamics.com/XRMServices/2011/Organization.svc");
            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = "jdelfino@alfapeople.com.br";
            clientCredentials.UserName.Password = "JBD@2018";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
           

            OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(uri, null, clientCredentials, null);

            ///serviceProxy.EnableProxyTypes();

            return serviceProxy;
        }
    }
}
