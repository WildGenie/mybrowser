using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SalesforceUploader
{
    public class Program
    {
        static void Main(string[] args)
        {            

        }

        public static Exception UploadException { get; set; }

        public static Exception UploadFile(string FileToUpload, string EndPointAddress, string username, string password, string token, string Title, String CaseID, string MachineName)
        {
            Exception retVal = null;
            UploadException = null;
            try
            {
                EndpointAddress ea = new EndpointAddress(EndPointAddress);
                ServiceReference.SessionHeader header = new ServiceReference.SessionHeader();
                string ServerURL = null;

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;

                using (ServiceReference.SoapClient loginClient = new ServiceReference.SoapClient(binding, ea))
                {
                    var loginResult = loginClient.login(null, username, string.Format("{0}{1}", password, token));
                    header.sessionId = loginResult.sessionId;
                    ServerURL = loginResult.serverUrl;
                }

                using (ServiceReference.SoapClient client = new ServiceReference.SoapClient(binding, new EndpointAddress(ServerURL)))
                {

                    ServiceReference.SaveResult[] sr;
                    ServiceReference.LimitInfo[] li;
                    ServiceReference.ContentVersion cv = new ServiceReference.ContentVersion();
                    cv.VersionData = File.ReadAllBytes(FileToUpload);
                    //cv.VersionNumber = "1";
                    cv.Title = Title;
                    //cv.FileExtension = "txt";
                    cv.PathOnClient = FileToUpload;
                    client.create(header, null, null, null, null, null, null, null, null, null, null, null, new ServiceReference.sObject[] { cv }, out li, out sr);

                    for (int i = 0; i < sr.Length; i++)
                    {
                        if (sr[i].success)
                        {
                            ServiceReference.SaveResult[] csr;
                            // Create Call
                            ServiceReference.Call__c call = new ServiceReference.Call__c();
                            call.CaseId__c = CaseID;
                            call.File_Name__c = cv.Title;
                            call.Machine_Name__c = MachineName;
                            client.create(header, null, null, null, null, null, null, null, null, null, null, null, new ServiceReference.sObject[] { call }, out li, out csr);

                            // Create Share
                            for (int j = 0; j < sr.Length; j++)
                            {
                                CreateLink(ref retVal, header, client, sr, ref li, i, csr, j);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                retVal = ex;
            }
            return retVal;
        }

        private static void CreateLink(ref Exception retVal, ServiceReference.SessionHeader header, ServiceReference.SoapClient client, ServiceReference.SaveResult[] sr, ref ServiceReference.LimitInfo[] li, int i, ServiceReference.SaveResult[] csr, int j)
        {
            if (csr[j].success)
            {
                string sQuery = String.Format("SELECT ContentDocumentId FROM ContentVersion WHERE Id = '{0}'", sr[i].id);
                ServiceReference.QueryResult res;
                var lmt = client.query(header, null, null, null, sQuery, out res);
                IEnumerable<ServiceReference.ContentVersion> cvL = res.records.Cast<ServiceReference.ContentVersion>();
                ServiceReference.ContentDocumentLink link = new ServiceReference.ContentDocumentLink();
                link.ContentDocumentId = cvL.First().ContentDocumentId;
                link.LinkedEntityId = csr[j].id;
                link.ShareType = "V";
                ServiceReference.SaveResult[] xsr;
                client.create(header, null, null, null, null, null, null, null, null, null, null, null, new ServiceReference.sObject[] { link }, out li, out xsr);
                for (int x = 0; x < xsr.Length; x++)
                {
                    Console.WriteLine(xsr[x].success);
                    try
                    {
                        if(retVal == null)
                        {
                            retVal = xsr[x].success ? null : new Exception(xsr[x].errors[0].message);
                        }
                    }
                    catch(Exception ex)
                    {
                        retVal = new Exception("Create Link Error: " + ex.Message + ex.StackTrace);
                    }
                    
                }
            }
        }
    }
}
