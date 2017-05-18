using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using faceaccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using MongoDB.Bson;
using MongoDB.Driver;
using VueJsAspNetCoreSample.Documents;

namespace VueJsAspNetCoreSample.Controllers
{
  [Route("api/[controller]")]
  public class UsersController : Controller
  {
    private MongoDatabase _db;
    private IFaceServiceClient _faceClient;

    public UsersController(MongoDatabase db, IFaceServiceClient faceClient)
    {
      _db = db;
      _faceClient = faceClient;
    }
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    public IActionResult Users()
    {     
      var users = _db.Persons.AsQueryable().ToList();
      foreach (var user in users)
      {
        if (!string.IsNullOrEmpty(user.Photo))
        {
          user.Photo = Setting.FaceClientImgPrefix + user.Photo;
        }
      }
      return this.Json(users);
    }

    [Authorize]
    [Route("{userId}/faces")]
    [HttpGet]
    public IActionResult Faces(string userId)
    {
      return this.Json(_db.Faces.AsQueryable().Where(x => x.UserId == userId).ToList().Select(x =>
      {
        x.ImageBase64 = Setting.FaceClientImgPrefix + x.ImageBase64;
        return x;
      }));
    }

    [Authorize]
    [Route("{userId}")]
    [HttpGet]
    public async Task<IActionResult> GetUser(string userId)
    {
      var cursor = await _db.Persons.FindAsync(Builders<PersonDocument>.Filter.Eq(x => x.Id, userId));
      var doc = await cursor.FirstAsync();
      return this.Json(doc);
    }

    public class FaceUploadModel
    {
      public string UserID { get; set; }
      public string Photo { get; set; }
    }

    public class FacesUploadModel
    {
      public string UserID { get; set; }
      public string[] Photos { get; set; }
    }

    [Authorize]
    [Route("faces")]
    [HttpPost]
    public async Task<IActionResult> AddFace([FromBody] FacesUploadModel model)
    {
      var cursor = await _db.Persons.FindAsync(Builders<PersonDocument>.Filter.Eq(x => x.Id, model.UserID));
      var doc = await cursor.FirstAsync();
      List<FaceDocument> facesDoc = new List<FaceDocument>();
      foreach (var photoFace in model.Photos)
      {
        var photo = photoFace.Substring(Setting.FaceClientImgPrefix.Length);
        var bytes = Convert.FromBase64String(photo);
        var memoryStream = new MemoryStream(bytes);
        var result = await _faceClient.AddPersonFaceAsync(Setting.FaceClientPersonGroupKey, doc.PersonId, memoryStream /*, null, targetFace */ );

        var face = new FaceDocument()
        {
          Id = ObjectId.GenerateNewId().ToString(),
          UserId = doc.Id,
          ImageBase64 = photo,
          PersonId = doc.PersonId,
          PersistedFaceId = result.PersistedFaceId,
          Created = DateTime.Now
        };
        facesDoc.Add(face);
        await _db.Faces.InsertOneAsync(face);
      }
      facesDoc.ForEach(c => c.ImageBase64 = Setting.FaceClientImgPrefix + c.ImageBase64);

      return this.Json(facesDoc);
    }

    [Authorize]
    [Route("{photoId}/deletePhoto")]
    [HttpGet]
    public async Task<IActionResult> DeletePhoto(string photoId)
    {
      var cursor = _db.Faces.FindOneAndDelete(Builders<FaceDocument>.Filter.Eq(x => x.Id, photoId));
      if (cursor == null)
      {
        return this.Json(BadRequest());
      }
      await _faceClient.DeletePersonFaceAsync(Setting.FaceClientPersonGroupKey, cursor.PersonId, cursor.PersistedFaceId);
      return this.Json(Ok());
    }

    [Authorize]
    [Route("detect")]
    [HttpPost]
    public async Task<IActionResult> Detect([FromBody] FaceUploadModel model)
    {
      //var faceDetect = await _faceClient.DetectAsync(this.Request.Body,true,true);
      //var targetFace = faceDetect.First().FaceRectangle;
      //this.Request.Body.Seek(0, SeekOrigin.Begin);
      var photo = model.Photo.Substring(Setting.FaceClientImgPrefix.Length);
      var bytes = Convert.FromBase64String(photo);

      var memoryStream = new MemoryStream(bytes);
      var result = await _faceClient.DetectAsync(memoryStream, true, true /*, null, targetFace */ );
      var face = result[0];
      return this.Json(new
      {
        FaceId = face.FaceId,
        Rect = face.FaceRectangle,
        Landmarks = face.FaceLandmarks
      });
    }

    private const double ConfidenceTreshold = 0.8;

    [Authorize]
    [Route("check")]
    [HttpPost]
    public async Task<IActionResult> Check()
    {
      var faceDetect = await _faceClient.DetectAsync(this.Request.Body);
      var identity = await _faceClient.IdentifyAsync(Setting.FaceClientPersonGroupKey, new[] { faceDetect[0].FaceId });
      var result = identity.FirstOrDefault()?.Candidates.FirstOrDefault() ?? new Candidate();
      var response = new CheckResponse
      {
        Access = false,
        PersonId = result.PersonId,
        Confidence = result.Confidence,
        Name = "Unknown"
      };
      if (response.PersonId != null)
      {
        var doc = _db.Persons.AsQueryable().Where(x => x.PersonId == response.PersonId).FirstOrDefault();
        response.Name = doc.Name;
      }
      if (response.Confidence >= Convert.ToDouble(ConfidenceTreshold))
      {
        response.Access = true;
      }
      return this.Json(response); ;
    }

    [Authorize]
    [Route("train")]
    [HttpPost]
    public async Task<IActionResult> Train()
    {
      await _faceClient.TrainPersonGroupAsync(Setting.FaceClientPersonGroupKey);
      return this.Json(Ok());
    }

    public class IdentifyRequest
    {
      public Guid FaceId { get; set; }
    }

    [Authorize]
    [Route("identify")]
    [HttpPost]
    public async Task<IActionResult> Identify([FromBody] IdentifyRequest request)
    {
      var result = await _faceClient.IdentifyAsync(Setting.FaceClientPersonGroupKey, new[] { request.FaceId });
      var doc = _db.Persons.AsQueryable().Where(x => x.PersonId == result[0].Candidates[0].PersonId).FirstOrDefault();
      return this.Json(doc);
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
