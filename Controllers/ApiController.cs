using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using MongoDB.Bson;
using MongoDB.Driver;
using VueJsAspNetCoreSample.Documents;

namespace VueJsAspNetCoreSample.Controllers {
    [Route ("api/[controller]")]
    public class UsersController : Controller {
        private MongoDatabase _db;
        private IFaceServiceClient _faceClient = new FaceServiceClient ("ae10dbb146c749ce8810068d9b83a868");

        const string _personGroupKey = "paralect";

        public UsersController (MongoDatabase db) {
            _db = db;
        }
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Users () {
            return this.Json (_db.Persons.AsQueryable ());
        }

        [Route ("{userId}/faces")]
        [HttpGet]
        public IActionResult Faces (string userId) {
            return this.Json (_db.Faces.AsQueryable ().Where (x => x.UserId == userId).ToList ().Select (x => {
                x.ImageBase64 = _imgPrefix + x.ImageBase64;
                return x;
            }));
        }

        [Route ("{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetUser (string userId) {
            var cursor = await _db.Persons.FindAsync (Builders<PersonDocument>.Filter.Eq (x => x.Id, userId));
            var doc = await cursor.FirstAsync ();
            return this.Json (doc);
        }

        public class FaceUploadModel {
            public string UserID { get; set; }
            public string Photo { get; set; }
        }

        private const string _imgPrefix = "data:image/jpeg;base64,";

        [Route ("faces")]
        [HttpPost]
        public async Task<IActionResult> AddFace ([FromBody] FaceUploadModel model) {
            var cursor = await _db.Persons.FindAsync (Builders<PersonDocument>.Filter.Eq (x => x.Id, model.UserID));
            var doc = await cursor.FirstAsync ();
            var photo = model.Photo.Substring (_imgPrefix.Length);
            var bytes = Convert.FromBase64String (photo);
            var memoryStream = new MemoryStream (bytes);
            var result = await _faceClient.AddPersonFaceAsync (_personGroupKey, doc.PersonId, memoryStream /*, null, targetFace */ );
            var face = new FaceDocument () {
                Id = ObjectId.GenerateNewId ().ToString (),
                    UserId = doc.Id,
                    ImageBase64 = photo,
                    PersonId = doc.PersonId,
                    PersistedFaceId = result.PersistedFaceId,
                    Created = DateTime.Now
            };
            await _db.Faces.InsertOneAsync(face);            
            return this.Json (doc);
        }

        [Route("{photoId}/deletePhoto")]
        [HttpGet]
        public async Task<IActionResult> DeletePhoto(string photoId)
        {
            var cursor = _db.Faces.FindOneAndDelete(Builders<FaceDocument>.Filter.Eq(x => x.Id, photoId));             
            if (cursor == null)
            {
                return this.Json(BadRequest());
            }
            await _faceClient.DeletePersonFaceAsync(_personGroupKey, cursor.PersonId, cursor.PersistedFaceId);       
            return this.Json(Ok());
        }

        [Route ("detect")]
        [HttpPost]
        public async Task<IActionResult> Detect ([FromBody] FaceUploadModel model) {
            //var faceDetect = await _faceClient.DetectAsync(this.Request.Body,true,true);
            //var targetFace = faceDetect.First().FaceRectangle;
            //this.Request.Body.Seek(0, SeekOrigin.Begin);
            var photo = model.Photo.Substring (_imgPrefix.Length);
            var bytes = Convert.FromBase64String (photo);

            var memoryStream = new MemoryStream (bytes);
            var result = await _faceClient.DetectAsync (memoryStream, true, true /*, null, targetFace */ );
            var face = result[0];
            return this.Json (new {
                FaceId = face.FaceId,
                    Rect = face.FaceRectangle,
                    Landmarks = face.FaceLandmarks
            });;
        }

        private const double ConfidenceTreshold = 0.8;

        [Route ("check")]
        [HttpPost]
        public async Task<IActionResult> Check () {
            var faceDetect = await _faceClient.DetectAsync(this.Request.Body);
            var identity = await _faceClient.IdentifyAsync(_personGroupKey,new[]{faceDetect[0].FaceId});
            var result = identity.FirstOrDefault()?.Candidates.FirstOrDefault() ?? new Candidate();
            var response = new CheckResponse {
              Access = false,
              PersonId = result.PersonId,
              Confidence = result.Confidence,
              Name = "Unknown"
            };
            if (response.PersonId != null)
            {
              var doc = _db.Persons.AsQueryable ().Where (x => x.PersonId == response.PersonId).FirstOrDefault ();
              response.Name = doc.Name;
            }
            if (response.Confidence >= ConfidenceTreshold)
            {
              response.Access = true;
            }
            return this.Json(response);;
        }

        [Route ("train")]
        [HttpPost]
        public async Task<IActionResult> Train () {
            await _faceClient.TrainPersonGroupAsync (_personGroupKey);
            return this.Ok ();
        }

        public class IdentifyRequest {
            public Guid FaceId { get; set; }
        }

        [Route ("identify")]
        [HttpPost]
        public async Task<IActionResult> Identify ([FromBody] IdentifyRequest request) {
            var result = await _faceClient.IdentifyAsync (_personGroupKey, new [] { request.FaceId });
            var doc = _db.Persons.AsQueryable ().Where (x => x.PersonId == result[0].Candidates[0].PersonId).FirstOrDefault ();
            return this.Json (doc);
        }
    }

    internal class CheckResponse
    {
        public bool Access { get; set; }
        public Guid PersonId { get; set; }
        public double Confidence { get; set; }
        public string Name { get; set; }
    }
}
