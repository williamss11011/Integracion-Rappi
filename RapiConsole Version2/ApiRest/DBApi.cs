using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;



namespace RapiConsole.ApiRest
{
    class DBApi
    {
        public dynamic GetProduct(string url)
        { 
          try { 
                HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myWebRequest.Headers.Add("api_key", "a52375e9-5959-47f0-96d5-f9e6de8fd255");
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
            Stream myStream = myHttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myStream);
            string Datos = HttpUtility.HtmlDecode(myStreamReader.ReadToEnd());
            dynamic data = JsonConvert.DeserializeObject(Datos); ///deseareliza
            return data;
        }
            catch (Exception ex)
            {
                 Console.WriteLine("Error desde GET:" + ex);
                dynamic datos = "no se encuentra sku";


                return datos;

            }

        }

        public dynamic PutProduct(string url, string json, string autorizacion = null)
        {
         //   try
           // {
                var client = new RestClient(url);
                var request = new RestRequest(Method.PUT);
                request.AddHeader("api_key", "x4/oW55H/nr+o+gB/sNPwr0QzZAmzcpa");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                if (autorizacion != null)
                {
                    request.AddHeader("Authorization", autorizacion);
                }
            IRestResponse response = client.Execute(request);
            dynamic dat = response.IsSuccessful;
            dynamic dat2 = response.StatusDescription;
            dynamic dat3 = response.Content;



            if (dat == true || dat2 == "OK")
            {

                dynamic datos = JsonConvert.DeserializeObject(response.Content);
                return datos;
            }
            else
            {
                dynamic datos = dat3;
                return datos;
            }


            // }
            /*   catch (Exception ex)
               {
                   Console.WriteLine("Error desde PUT:" + ex);
                   return null;
               }*/
        }

        public dynamic PostProduct(string url, string json, string autorizacion = null)
        {
            //try
            //{
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("api_key", "a52375e9-5959-47f0-96d5-f9e6de8fd255");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            if (autorizacion != null)
            {
                request.AddHeader("Authorization", autorizacion);
            }

            IRestResponse response = client.Execute(request);
            dynamic dat = response.IsSuccessful;
            dynamic dat2 = response.StatusDescription;

            dynamic dat3 = response.Content;



            if (dat == true || dat2 == "OK")
                {
                    
                    dynamic datos = JsonConvert.DeserializeObject(response.Content);
                    return datos;
                }
                else 
                {
                dynamic datos =dat3;
                return datos;
                }

               
                
                
         /*   }
            catch (Exception ex)
            {
                Console.WriteLine("Error desde POST:" + ex);
                return null;
            }*/
        }

    }
}
