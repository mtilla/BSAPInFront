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
    public class DocumentRepository3 //TODO use ADO.NET to access data in database, not this JSON file technique
    {
        public static string connString = @"Server=localhost\sqlexpress;Database=BSDB;Trusted_Connection=True;";

        //Källa: https://jinishbhardwaj.wordpress.com/2014/07/01/c-dynamic-and-expandoobject-to-fetch-data-from-sql-server/
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
    }
}
