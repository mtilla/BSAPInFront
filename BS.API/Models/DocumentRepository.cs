using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HostEnvrmt = System.Web.Hosting.HostingEnvironment;

namespace BS.API.Models
{
    /// <summary>
    /// At the moment it stores the data in a json file so that no database is required.. later TODO
    /// </summary>
    public class DocumentRepository //TODO use ADO.NET to access data in database, not this JSON file technique
    {
        /// <summary>
        /// Creates a new document and sets server-defined defaults (default values)
        /// </summary>
        /// <returns></returns>
        internal Document Create()
        {
            Document doc = new Document
            {
                CreateDate = DateTime.Now
                //Kanske även måste implementera Guid property?
            };       
            return doc;
        }

        /// <summary>
        /// Retrieves the list of documents.
        /// </summary>
        /// <returns></returns>
        internal List<Document> Retrieve() //TODO: Rewrite the method to fetch data from the DB
        {
            var filePath = HostEnvrmt.MapPath(@"~/App_Data/document.json");
            var json = System.IO.File.ReadAllText(filePath);
            var documents = JsonConvert.DeserializeObject<List<Document>>(json);
            return documents;
        }

        /// <summary>
        /// Saves a newly-created document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        internal Document Save(Document document)
        {
            var docsList = this.Retrieve();
            // Assign a new CUSTOMER_NR
            var maxId = docsList.Max(d => d.CustomerNr);
            //Convert.ToInt32(input)
            document.CustomerNr = maxId + 1;
            docsList.Add(document);
            WriteData(docsList);
            return document;
        }

        /// <summary>
        /// Updates an existing document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        internal Document Save(int id, Document document)
        {
            // Read in the existing documents
            var docsList = this.Retrieve();
            // Locate and replace the item
            var itemIndex = docsList.FindIndex(p => p.PartID == document.PartID);
            if (itemIndex > 0)
            {
                docsList[itemIndex] = document;
            }
            else
            {
                return null;
            }
            WriteData(docsList);
            return document;
        }

        private bool WriteData(List<Document> documents)
        {
            // Writes all of the data back to the JSON file
            var filePath = HostEnvrmt.MapPath(@"~/App_Data/document.json");
            var json = JsonConvert.SerializeObject(documents, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);
            return true;
        }
    }
}