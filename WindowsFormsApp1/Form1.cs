using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;


namespace WindowsFormsApp1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            var connectionStringCRM = @"AuthType=Office365;Username=adm@academiatalentosFYI2021.onmicrosoft.com;Password=Abcd@1234;Url=https://orgb6ef4f6c.crm8.dynamics.com";
            var p = Provider(connectionStringCRM);

            QueryExpression queryExpression = new QueryExpression("account");

            queryExpression.Criteria.AddCondition("name", ConditionOperator.Equal, "teste");
            queryExpression.ColumnSet = new ColumnSet(true);
            EntityCollection colecaoEntidades = p.RetrieveMultiple(queryExpression);
            foreach (var item in colecaoEntidades.Entities) {
                Console.WriteLine(item["name"]);
            }
           
        }

        private static CrmServiceClient crmServiceClient;
        public static IOrganizationService Provider(string connectionString) {
            if (crmServiceClient == null) {
                string lastCrmError = string.Empty;
                int tentativa = 0;
                while (true) {
                    if (tentativa > 3)
                        throw new Exception(lastCrmError);

                    tentativa++;

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    crmServiceClient = new CrmServiceClient(connectionString);

                    if (!crmServiceClient.IsReady) {
                        if (tentativa > 3)
                            throw crmServiceClient.LastCrmException;
                        else
                            lastCrmError = crmServiceClient.LastCrmError;
                    } else {
                        System.Diagnostics.Trace.WriteLine("Performance: Provider 365");
                        break;
                    }
                }
            }

            return crmServiceClient.OrganizationServiceProxy;
        }
    }
}
