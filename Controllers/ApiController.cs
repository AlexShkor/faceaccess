using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Face;
using MongoDB.Driver;
using VueJsAspNetCoreSample.Documents;

namespace VueJsAspNetCoreSample.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private MongoDatabase _db;

        const string _personGroupKey = "paralect";

        public ApiController()
        {
        }

        [Route("users")]
        [HttpGet]
        public IActionResult Users()
        {
           return this.Json(new[]{new {Name = "test"}});
            return this.Json(_db.Persons.AsQueryable());
        }

        [Route("users")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]PersonDocument doc){
            var faceClient = new FaceServiceClient("ae10dbb146c749ce8810068d9b83a868");
            var data = faceClient.CreatePersonAsync(_personGroupKey, doc.Name);
            doc.RecognitionId = data.Result.PersonId;
            await _db.Persons.InsertOneAsync(doc);
            return this.Json(doc);
        }
    }
}
