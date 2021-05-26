using System;
using System.Net;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;

namespace RoboDeImportacao
{
    class Conexao
    {
        private static CrmServiceClient crmServiceClientOrigem;
        private static CrmServiceClient crmServiceClientDestino;
        public CrmServiceClient ObterOrigem() {
            var connectionStringCRM = @"AuthType=OAuth;
            Username = grupo7FYI@grupo7FYI.onmicrosoft.com;
            Password = Ac@demiaTalentos7; SkipDiscovery = True;
            AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
            RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
            Url = https://Projeto2021.api.crm.dynamics.com/XRMServices/2011/Organization.svc;";


            if (crmServiceClientOrigem == null) {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                crmServiceClientOrigem = new CrmServiceClient(connectionStringCRM);
            }
            return crmServiceClientOrigem;
        }
        public CrmServiceClient ObterDestino()
        {
            var connectionStringCRM = @"AuthType=OAuth;
            Username = grupo7FYI@grupo7FYI.onmicrosoft.com;
            Password = Ac@demiaTalentos7; SkipDiscovery = True;
            AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
            RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
            Url = https://grupo7fyi.api.crm.dynamics.com/XRMServices/2011/Organization.svc;";


            if (crmServiceClientDestino == null)
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                crmServiceClientDestino = new CrmServiceClient(connectionStringCRM);
            }
            return crmServiceClientDestino;
        }
    }
}