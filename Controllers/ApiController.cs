using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Face;
using MongoDB.Bson;
using MongoDB.Driver;
using VueJsAspNetCoreSample.Documents;

namespace VueJsAspNetCoreSample.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private MongoDatabase _db;
        private IFaceServiceClient _faceClient = new FaceServiceClient("ae10dbb146c749ce8810068d9b83a868");

        const string _personGroupKey = "paralect";

        public UsersController(MongoDatabase db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Users()
        {
            return this.Json(_db.Persons.AsQueryable());
        }

        [Route("{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetUser(string userId)
        {
            var cursor = await _db.Persons.FindAsync(Builders<PersonDocument>.Filter.Eq(x=> x.Id, userId));
            var doc = await cursor.FirstAsync();
            return this.Json(_db.Persons.AsQueryable());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]PersonDocument doc){
            var data = _faceClient.CreatePersonAsync(_personGroupKey, doc.Name);
            doc.RecognitionId = data.Result.PersonId;
            doc.Created = DateTime.Now;
            doc.Id = ObjectId.GenerateNewId().ToString();
            await _db.Persons.InsertOneAsync(doc);
            return this.Json(doc);
        }

        public class FaceUploadModel{
          public string UserID {get;set;}
          public string Photo {get;set;}
        }

        [Route("faces")]
        [HttpPost]
        public async Task<IActionResult> AddFace([FromBody]FaceUploadModel model){
            var cursor = await _db.Persons.FindAsync(Builders<PersonDocument>.Filter.Eq(x=> x.Id, model.UserID));
            var doc = await cursor.FirstAsync();
            //var faceDetect = await _faceClient.DetectAsync(this.Request.Body,true,true);
            //var targetFace = faceDetect.First().FaceRectangle;
            //this.Request.Body.Seek(0, SeekOrigin.Begin);
            var photo = model.Photo.Substring("data:image/jpeg;base64,".Length);
            var bytes = Convert.FromBase64String(photo);

            var memoryStream = new MemoryStream(bytes);
            var result = await _faceClient.AddPersonFaceAsync(_personGroupKey, doc.RecognitionId,memoryStream /*, null, targetFace */);
            return this.Json(doc);
        }

        [Route("detect")]
        [HttpPost]
        public async Task<IActionResult> Detect([FromBody]FaceUploadModel model){
            //var faceDetect = await _faceClient.DetectAsync(this.Request.Body,true,true);
            //var targetFace = faceDetect.First().FaceRectangle;
            //this.Request.Body.Seek(0, SeekOrigin.Begin);
            var photo = model.Photo.Substring("data:image/jpeg;base64,".Length);
            var bytes = Convert.FromBase64String(photo);

            var memoryStream = new MemoryStream(bytes);
            var result = await _faceClient.DetectAsync(memoryStream, true, true /*, null, targetFace */);
            var face = result[0];
            return this.Json(new {
              Rect = face.FaceRectangle,
              Landmarks = face.FaceLandmarks
            });;
        }
    }
}
