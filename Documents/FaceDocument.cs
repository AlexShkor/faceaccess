using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace VueJsAspNetCoreSample.Documents
{
    public class FaceDocument
    {
        [BsonId]
        public string Id {get;set;}

        public string UserId {get;set;}

        public Guid PersonId {get; set;}

        public DateTime Created { get;  set; }

        public string ImageBase64 {get;set;}

        public Guid PersistedFaceId {get; set;}

        public List<Landmark> Landmarks {get;set;} = new List<Landmark>();

        public Rectangle Rectangle {get;set;}
    }

    public class Rectangle
    {
      public int X {get;set;}

      public int Y  {get;set;}

      public int Width {get;set;}

      public int Height {get;set;}
    }

    public class Landmark{
      public string Type {get;set;}

      public int X {get;set;}

      public int Y {get;set;}
    }
}
