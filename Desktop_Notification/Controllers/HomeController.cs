using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Desktop_Notification.Models;
using MySql.Data.MySqlClient;

namespace Desktop_Notification.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            string title = "Notification";
            string msg = "Hi my name is Ritesh Kumar Sriwastav";
            var result = "-1";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}",
            "AAAAg8GtrSU:APA91bFLSxipT4oS5QWPLXTcZQHaWt_Zl5naMYh5Gh3kDd-vGCmdSei_DP2xa_1Mx8ajM4Qg33GOEPWM1F1g2iRwFWfq9Oc2kllL1V0MUP-TMIfjZp1K2IF_4FEHsY9Re9C7wOiJifYS"));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", "565890100517"));
            httpWebRequest.Method = "POST";

            var payload = new
            {
                to = Hardware_ID.Get_HardWareID,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = msg,
                    title = title
                },
            };
            var serializer = new JavaScriptSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = serializer.Serialize(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ReadExcel()
        {
            return View();
        }
        [ActionName("Importexcel")]
        [HttpPost]
        public ActionResult Importexcel1()
        {
            string connString = string.Empty;
            DataTable dt = new DataTable();
            if (Request.Files["FileUpload1"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();

                string[] validFileTypes = { ".xls", ".xlsx", ".csv" };

                string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads"), Request.Files["FileUpload1"].FileName);
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/Uploads"));
                }
                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                    { System.IO.File.Delete(path1); }
                    Request.Files["FileUpload1"].SaveAs(path1);
                    if (extension == ".csv")
                    {
                        dt = Utility.ConvertCSVtoDataTable(path1);
                        ViewBag.Data = dt;
                    }
                    //Connection String to Excel Workbook  
                    else if (extension.Trim() == ".xls")
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                        ViewBag.Data = dt;
                    }
                    else if (extension.Trim() == ".xlsx")
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                    }
                    connString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(connString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {

                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "[dbo].[excelread2]";

                            //[OPTIONAL]: Map the Excel columns with that of the database table

                            sqlBulkCopy.ColumnMappings.Add("Sr#No", dt.Columns[0].ToString());
                            sqlBulkCopy.ColumnMappings.Add("URL", dt.Columns[1].ToString());
                            sqlBulkCopy.ColumnMappings.Add("Name", dt.Columns[2].ToString());
                            sqlBulkCopy.ColumnMappings.Add("Parameter", dt.Columns[3].ToString());
                            sqlBulkCopy.ColumnMappings.Add("Complete_Url", dt.Columns[4].ToString());
                            sqlBulkCopy.ColumnMappings.Add("Local_Url", dt.Columns[5].ToString());
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                        }
                    }
                }
                else
                {
                    ViewBag.Error = "Please Upload Files in .xls, .xlsx or .csv format";

                }

            }

            return View("Importexcel1");
        }
    }
}