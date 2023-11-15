using RapiConsole.ApiRest;
using System;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using System.Text;


namespace RapiConsole
{
    public class Program
    {
        static DBApi dBApi = new DBApi();
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("ejecutando.. InsertarProductos Rappi");

                using (SqlConnection con = new SqlConnection(@"Data Source=facturacion.bebelandia.com.ec;Initial Catalog=InventarioSQL;User ID=inventario;Password=inventa3iobm11$"))

                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("select Codigo_Tienda_Rappi from local where Codigo_Tienda_Rappi is not null", con);

                    cmd.ExecuteNonQuery();
                   

                    var dt = new DataTable();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                   

                    foreach (DataRow row in dt.Rows)
                    {
                        string local = row["Codigo_tienda_Rappi"].ToString();
                        InsertarProductos(local);
                    }


                }
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            finally
            {
                Console.WriteLine("FIN");
                //ActualizarProductos();

                // ObtenerProductos();

                //Console.WriteLine("ejecutando.. InsertarStock");
                // InsertarStock();

                //Console.WriteLine("ejecutando.. ActualizarStock");
                //ActualizarStock();

                // ObtenerStock();

            }
        }
        private static void SendMail(string error)
        {
            //error = "";
            //  Console.WriteLine(error);
            // Console.WriteLine("Mail enviado co56565656565te");
            /***********/
            string notificacion = error.ToString();
            string destinatario = "william.marcillo@bebemundo.ec";
            string destinatario2 = "jaime.barrionuevo@bebemundo.ec";

            SmtpClient client = new SmtpClient()
            {

                Host = "mx2.crmbebemundo.ec",
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = "administrador/mx2",
                    Password = "Vpayp8vb2UmIdfD7z99sLtIn"
                }

            };

            MailAddress FromEmail = new MailAddress("notificacion.rappi@mundobebemundo.ec", "Notificacion");
            MailAddress ToEmail = new MailAddress(destinatario);
            MailAddress ToEmail2 = new MailAddress(destinatario2);


            //string gpsc = System.IO.File.ReadAllText(@"C:\HTML\GPSC.txt");
            string dd = notificacion;
            string html = dd;
            string htmlBody = html;

            MailMessage Message = new MailMessage();

            Message.From = FromEmail;
            Message.Subject = "Notificacion de Error";
            Message.Body = htmlBody;
            Message.IsBodyHtml = true;
            Message.BodyTransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
            Message.To.Add(ToEmail);
            Message.CC.Add(ToEmail2);

            // Message.Attachments.Add(new Attachment("C:/HTML/AAA.pdf"));
            try
            {
                client.Send(Message);
                Console.WriteLine("Mail enviado correctamente");
            }
            catch (Exception exc)
            {
                Console.WriteLine("" + exc.Message, "");
            }

        }


        private static void ObtenerProductos()
        {
            try
            {
                // dynamic response = dBApi.GetProduct($"https://ecom-public-external-ec.security.rappi.com:4443/api/ecom-public/produc");
                dynamic response = dBApi.GetProduct($"https://ecom-public-external-ec.security.rappi.com:4443/api/ecom-public/v2/products?page=0&page_size=9730");

                string res = response.products.ToString();
                Console.WriteLine(res);

            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                SendMail(error);
                Console.WriteLine(ex);

            }

        }
        private static void ActualizarProductos()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=facturacion.bebelandia.com.ec;Initial Catalog=InventarioSQL;User ID=inventario;Password=inventa3iobm11$"))
                //using (con)

                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("dbo.spProductosRapi", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    var dt = new DataTable();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        string cod = row["cod"].ToString();
                        string put = row["put"].ToString();

                        dynamic respuesta = dBApi.PutProduct($"https://ecom-public-external-ec.security.rappi.com:4443/api/ecom-public/v2/products/{cod}", put);
                        string res = respuesta.ToString();
                        Console.WriteLine(res);

                        string update = "update item set fecha_upd_rappi= getdate(), upd_rappi='S' where id_item = (select i.id_item from dbo.ITEM i INNER JOIN  dbo.vMaxId_ItemEnCS_Barra AS vCB ON i.id_item = vCB.id_item where i.rappi = 1 and replace(vCB.Codigo_Item,'/','-')='" + cod + "');";
                        SqlCommand cmd2 = new SqlCommand(update, con);
                        cmd2.ExecuteNonQuery();


                    }
                }
            }
            catch (SqlException ex)
            {
                string error = ex.ToString();
                SendMail(error);
                Console.WriteLine(ex);
            }
        }

        private static void InsertarProductos( string tienda)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=facturacion.bebelandia.com.ec;Initial Catalog=InventarioSQL;User ID=inventario;Password=inventa3iobm11$"))


                {

                   // tienda = "5989";

                    con.Open();
                    SqlCommand cmd = new SqlCommand("dbo.spInsertProductosRapiV2", con);
                    
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@store", SqlDbType.VarChar).Value = tienda;

                    var dt = new DataTable();
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    string jsonA = @"
                       {                        
                        ""records"":
                        [";

                    string jsonC = @"]}";


                    StringBuilder jsonf = new StringBuilder();

                    ////////////////
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        string store_id = dt.Rows[i]["store_id"].ToString();
                        string id = dt.Rows[i]["id"].ToString();
                        string stock = dt.Rows[i]["stock"].ToString();
                        string price = dt.Rows[i]["price"].ToString();
                        string is_available = dt.Rows[i]["is_available"].ToString();
                        string discount_price = dt.Rows[i]["discount_price"].ToString();
                        string ean = dt.Rows[i]["ean"].ToString();
                        string name = dt.Rows[i]["name"].ToString();
                        string description = dt.Rows[i]["description"].ToString();
                        string trademark = dt.Rows[i]["trademark"].ToString();
                        string sale_type = dt.Rows[i]["sale_type"].ToString();
                        string image_url = dt.Rows[i]["image_url"].ToString();
                        

                        string jsonB = @"
                        {
                        ""store_id"": ""store_id2"",
                        ""id"": ""id2"",
                        ""stock"": stock2,
                        ""price"": price2,
                        ""is_available"": is_available2,
                        ""discount_price"": discount2,
                        ""ean"": ""ean2"",
                        ""name"": ""name2"",
                        ""description"": ""description2"",
                        ""trademark"": ""trademark2"",
                        ""sale_type"": ""sale_type2"",
                        ""image_url"": ""image_url2""
                        },";


                        jsonB = jsonB.Replace("store_id2", store_id);
                        jsonB = jsonB.Replace("id2", id);
                        jsonB = jsonB.Replace("stock2", stock);
                        jsonB = jsonB.Replace("price2", price);
                        jsonB = jsonB.Replace("is_available2", is_available);
                        jsonB = jsonB.Replace("discount2", discount_price);
                        jsonB = jsonB.Replace("ean2", ean);
                        jsonB = jsonB.Replace("name2", name);
                        jsonB = jsonB.Replace("description2", description);
                        jsonB = jsonB.Replace("trademark2", trademark);
                        jsonB = jsonB.Replace("sale_type2", sale_type);
                        jsonB = jsonB.Replace("image_url2", image_url);
                       
                        jsonf.Append(jsonB);
                        string jsonN = jsonf.ToString();
                  
                    }

                    string json = jsonA + jsonf + jsonC;
                    json = json.Replace("},]}", "}]}");
                   
                    dynamic respuesta = dBApi.PostProduct("https://services.grability.rappi.com/api/cpgs-integration/datasets", json);
                    string res = respuesta.ToString();
                   Console.WriteLine("Respuesta WS:" + res);

                }
            }
            ///////////////









            catch (SqlException ex)
            {

                string error = ex.ToString();
                SendMail(error);
                Console.WriteLine(ex);

            }
        }

        private static void InsertarStock()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=facturacion.bebelandia.com.ec;Initial Catalog=InventarioSQL;User ID=inventario;Password=inventa3iobm11$"))

                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("dbo.spStockRapi", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    var dt = new DataTable();
                    var da = new SqlDataAdapter(cmd);
                    da.SelectCommand.CommandTimeout = 320;
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        string json = row["stock"].ToString();
                        dynamic respuesta = dBApi.PostProduct("https://ecom-public-external-ec.security.rappi.com:4443/api/ecom-public/v2/product-stock", json);
                        string res = respuesta.ToString();
                        Console.WriteLine(res);
                    }
                }
            }
            catch (SqlException ex)
            {
                string error = ex.ToString();
                SendMail(error);
                Console.WriteLine(ex);
            }
        }
        private static void ActualizarStock()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=facturacion.bebelandia.com.ec;Initial Catalog=InventarioSQL;User ID=inventario;Password=inventa3iobm11$"))

                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("dbo.spStockRapi", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    var dt = new DataTable();
                    var da = new SqlDataAdapter(cmd);
                    da.SelectCommand.CommandTimeout = 320;
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        string sku = row["sku"].ToString();
                        string stock = row["stock"].ToString();

                        dynamic respuesta = dBApi.PutProduct($"https://ecom-public-external-ec.security.rappi.com:4443/api/ecom-public/v2/product-stock", stock);
                        string res = respuesta.ToString();
                        Console.WriteLine(res);
                        Console.WriteLine(sku);

                    }

                }
            }
            catch (SqlException ex)
            {

                string error = ex.ToString();
                SendMail(error);
                Console.WriteLine(ex);


            }

        }
        private static void ObtenerStock()
        {

            try
            {

                dynamic response = dBApi.GetProduct($"https://ecom-public-external-ec.security.rappi.com:4443/api/ecom-public/v2/product_stock/780.2200485K");
                string res = response.ToString();
                Console.WriteLine(res);

            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                SendMail(error);
                Console.WriteLine(ex);
            }
        }
    }
}
