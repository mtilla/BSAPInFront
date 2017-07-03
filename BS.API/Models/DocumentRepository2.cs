using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using HostEnvrmt = System.Web.Hosting.HostingEnvironment;

namespace BS.API.Models
{
    /// <summary>
    /// At the moment it stores the data in a json file so that no database is required.. later TODO
    /// </summary>
    public class DocumentRepository2 //TODO use ADO.NET to access data in database, not this JSON file technique
    {
        public static string connString = @"Server=localhost\sqlexpress;Database=BSDB;Trusted_Connection=True;";
        /// <summary>
        /// Retrieves the list of documents.
        /// </summary>
        /// <returns></returns>
        internal List<DocumentDBStyle> Retrieve() //TODO: Rewrite the method to fetch data from the DB
        {
            var filePath = HostEnvrmt.MapPath(@"~/App_Data/documentDBStyle.json");
            string json = System.IO.File.ReadAllText(filePath);
            var documents = JsonConvert.DeserializeObject<List<DocumentDBStyle>>(json);
            return documents;
        }

        internal List<TableRow> GetAll2matchRetrieve1(string query)
        {
            List<TableRow> tableRows = new List<TableRow>();
            DbAccess dbAccess = new DbAccess(connString);
            SqlConnection connection = dbAccess.getConnection();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = query;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    TableRow tableRow = new TableRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        tableRow.RowValues.Add(reader.GetName(i), reader[i].ToString());
                    }
                    tableRows.Add(tableRow);
                }
            }



            return tableRows.ToList();
        }

        private static SqlDataReader GetAll(string query)
        {
            DbAccess dbAccess = new DbAccess(connString);
            SqlConnection connection = dbAccess.getConnection();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = query;
            SqlDataReader reader = command.ExecuteReader();
            return reader;
        }

        public IEnumerable<dynamic> GetAllData(string query)
        {
            using (var conn = new SqlConnection(connString))
            {
                using (var command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return GetExpandoData(reader);
                        }
                    }
                    conn.Close();
                }
            }
        }


        //Provar med expando
        public dynamic GetExpandoData(SqlDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            for (var i = 0; i < reader.FieldCount; i++)
            {
                expandoObject.Add(reader.GetName(i), reader[i]);
            }
            return expandoObject;
        }

        ////Provar med expando
        //internal List<ExpandoObject> GetAllWithExpando(string query)
        //{
        //    //var tableResults = [
        //    //	{ "customer": "Abbas", "invoiceNr": 1233, "dueDate": new Date("December 12, 2016") },
        //    //	{ "customer": "ByggAB", "invoiceNr": 1236, "dueDate": new Date("May 25, 2017") },
        //    //	{ "customer": "Revyn", "invoiceNr": 1232, "dueDate": new Date("June 09, 2017") }
        //    //];
        //    //[
        //    //{ "ColumnName":"Age", "value":"32"},{"ColumnName":"Age", "value":"18"},
        //    //{ "ColumnName":"Age", "value":"47"},{"ColumnName":"Age", "value":"19"},
        //    //{ "ColumnName":"Age", "value":"32"},{"ColumnName":"Age", "value":"32"}]
        //    List< ExpandoObject> expandoObjsList = new List<ExpandoObject>();
        //    DbAccess dbAccess = new DbAccess(connString);
        //    SqlConnection connection = dbAccess.getConnection();
        //    SqlCommand command = connection.CreateCommand();
        //    command.CommandText = query;
        //    using (SqlDataReader reader = command.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {

        //            //TableRow tableRow = new TableRow();
        //            dynamic xpando = new ExpandoObject();
        //            for (int i = 0; i < reader.FieldCount; i++)
        //            {
        //                xpando.col = reader.GetName(i);    
        //                xpando.value = reader[i].ToString();
        //                //tableRow.RowValues.Add(reader.GetName(i), reader[i].ToString());
        //            }
        //            expandoObjsList.Add(xpando);
        //        }
        //    }
        //    return expandoObjsList.ToList();
        //}

        internal List<Column> GetAll2matchRetrieve(string query)
        {

            DbAccess dbAccess = new DbAccess(connString);
            SqlConnection connection = dbAccess.getConnection();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = query;
            List<Column> columnList = new List<Column>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read()) //varje rad i Tabell
                {
                    Column col;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (columnList.Count <= 0)
                        {
                            col = new Column();
                            col.ColumnName = reader.GetName(i);
                            col.Values.Add(reader[i].ToString());
                            columnList.Add(col);
                        }
                        else if (columnList.Count > 0)
                        {
                            var tempColumnList = new List<Column>();
                            foreach (var column in columnList)
                                if (reader.GetName(i) != column.ColumnName && column.ColumnName != null)
                                {
                                    Column newCol = new Column();
                                    newCol.ColumnName = reader.GetName(i);
                                    newCol.Values.Add(reader[i].ToString());
                                    tempColumnList.Add(newCol);
                                }
                            columnList.Add(tempColumnList[0]);
                        }
                        else if (columnList.Count > 0)
                        {
                            foreach (var column in columnList)
                                if (reader.GetName(i) == column.ColumnName && column.ColumnName != null)
                                {
                                    column.Values.Add(reader[i].ToString());
                                }
                        }
                    }
                }
            }
            return columnList.ToList();
        }



        public dynamic GetDocument(string fakturaNr)
        {
            List<DocumentDBStyle> docs = new List<DocumentDBStyle>();
            //SqlDataReader reader = GetDocument(fakturaNr.ToString());            
            DbAccess dbAccess = new DbAccess(connString);
            SqlConnection connection = dbAccess.getConnection();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM [dbo].[BS_LIGHT] WHERE FAKTURA_NR = @fakturanr";
            command.Parameters.AddWithValue("fakturanr", fakturaNr);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                byte[] documentDataData = null;
                while (reader.Read())
                {
                    DocumentDBStyle doc = new DocumentDBStyle();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        doc.Rows.Add(reader.GetName(i), reader[i].ToString());
                        if (reader.GetName(i) == "DocumentData")
                        {
                            documentDataData = (byte[])reader[i];

                            if (FileWriter.IsGZipHeader(documentDataData))
                            {
                                documentDataData = FileWriter.Decompress(documentDataData);
                            }
                        }
                    }
                    docs.Add(doc); //behövs eg inte
                }

                ////return Json(docs.FirstOrDefault()); 
                //Response.ContentType = "Application/pdf";
                //Response.ContentLength = documentDataData.Length;
                //Response.Body.Write(documentDataData, 0, documentDataData.Length);
                //return Response;
                ////return documentDataData;

                var response = new HttpResponseMessage(); //se https://forums.asp.net/t/1824120.aspx?returning+a+pdf+from+a+Web+Api+get
                response.Content = new ByteArrayContent(documentDataData);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                return response;
            }

        }


        //public SqlDataReader GetDocument(string fakturaNr) //Den var private static innan..why?
        //{
        //    DbAccess dbAccess = new DbAccess(connString);
        //    SqlConnection connection = dbAccess.getConnection();
        //    SqlCommand command = connection.CreateCommand();
        //    command.CommandText = "SELECT * FROM [dbo].[BS_LIGHT] WHERE FAKTURA_NR = @fakturanr";
        //    command.Parameters.AddWithValue("fakturanr", fakturaNr);
        //    SqlDataReader reader = command.ExecuteReader();
        //    return reader;
        //}

        /// <summary>
        /// Creates a new document and sets server-defined defaults (default values)
        /// </summary>
        /// <returns></returns>
        //    public internal Document Create()
        //{
        //    Document doc = new Document
        //    {
        //        CreateDate = DateTime.Now
        //        //Kanske även måste implementera Guid property?
        //    };
        //    return doc;
        //}

        /// <summary>
        /// Saves a newly-created document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        //internal DocumentDBStyle Save(DocumentDBStyle document)
        //{
        //    var docsList = this.Retrieve();

        //    // Assign a new CUSTOMER_NR
        //    var maxId = docsList.Max(d => d.CUSTOMER_NR);
        //    //Convert.ToInt32(input)
        //    document.CUSTOMER_NR = maxId + 1;
        //    docsList.Add(document);

        //    WriteData(docsList);
        //    return document;
        //}

        /// <summary>
        /// Updates an existing document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        //        internal DocumentDBStyle Save(int id, DocumentDBStyle document)
        //        {
        //            // Read in the existing documents
        //            var docsList = this.Retrieve();

        //            // Locate and replace the item
        //            var itemIndex = docsList.FindIndex(p => p.PartID == document.PartID);
        //            if (itemIndex > 0)
        //            {
        //                docsList[itemIndex] = document;
        //            }
        //            else
        //            {
        //                return null;
        //            }

        //            WriteData(docsList);
        //            return document;
        //        }

        //        private bool WriteData(List<DocumentDBStyle> documents)
        //        {
        //            // Writes all of the data back to the JSON file
        //            var filePath = HostEnvrmt.MapPath(@"~/App_Data/document.json");

        //            var json = JsonConvert.SerializeObject(documents, Formatting.Indented);
        //            System.IO.File.WriteAllText(filePath, json);

        //            return true;
        //        }
    }
}
