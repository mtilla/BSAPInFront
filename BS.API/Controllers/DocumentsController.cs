using BS.API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;



namespace BS.API.Controllers
{
    [EnableCorsAttribute("*", "*", "*")] //http://localhost:53574
    public class DocumentsController : ApiController //Testa, ifall du hinner, göra om till vanlig Controller med Http attribut, [HttpGet]
    {
        //public static string connString = ConfigurationManager.ConnectionStrings["BSCS"].ConnectionString;
        public static string connString = @"Server=localhost\sqlexpress;Database=BSDB;Trusted_Connection=True;";

        // GET: api/Documents
        //public IEnumerable<DocumentDBStyle> Get()
        //{
        //    DocumentRepository2 documentRepository = new DocumentRepository2();
        //    var list = documentRepository.Retrieve();
        //    return list;
        //}

        //GET: api/Documents
        //public IEnumerable<TableRow> Get2()
        //{
        //    var documentRepository = new DocumentRepository2();
        //    List<TableRow> resultList = documentRepository.GetAll2matchRetrieve1("SELECT TOP 12 * FROM [dbo].[BS_LIGHT]");
        //    return resultList;
        //}

        ////GET: api/Documents
        //public IEnumerable<TableRow> Get() //Med list av Tablerows
        //{
        //    var documentRepository = new DocumentRepository2();
        //    List<TableRow> resultList = documentRepository.GetAll2matchRetrieve1("SELECT TOP 12 * FROM [dbo].[BS_LIGHT]");
        //    return resultList;
        //}

        ////GET: api/Documents
        //public IEnumerable<ExpandoObject> Get() //Test med list av expando obj
        //{
        //    var documentRepository = new DocumentRepository2();  //("SELECT TOP 12 * FROM [dbo].[COL]")
        //    List<ExpandoObject> resultList = documentRepository.GetAllWithExpando("SELECT * FROM [dbo].[COL]");
        //    return resultList;
        //}

        //GET: api/Documents
        public dynamic Get() //Test med list av expando obj
        {
            var documentRepository = new DocumentRepository3();  //("SELECT TOP 12 * FROM [dbo].[COL]")
            dynamic dynamicData = documentRepository.GetAllData("SELECT top 7 * FROM [dbo].[COL]");
            return dynamicData;
        }

        // GET: api/Documents/5
        public dynamic Get(string id)
        {
            var documentRepository = new DocumentRepository2();
            return documentRepository.GetDocument(id);
        }

        // GET: api/Documents/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Documents
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Documents/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Documents/5
        public void Delete(int id)
        {
        }
    }
}
