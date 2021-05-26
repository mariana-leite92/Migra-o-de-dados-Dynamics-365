using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace ConexaoAlternativaExtending {
    public class ConexaoAlternativa {
        public OrganizationServiceProxy Obter() {
            Uri uri = new Uri("");
            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = "";
            clientCredentials.UserName.Password = "";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(uri, null, clientCredentials, null);

            //serviceProxy.EnableProxyTypes();

            return serviceProxy;
        }
    }
}
