using System;
using MongoDB.Bson.Serialization.Attributes;

namespace VueJsAspNetCoreSample.Documents
{
    public class PersonDocument
    {
        [BsonId]
        public string Id {get;set;}

        public string Name {get;set;}

        public Guid RecognitionId {get;set;}
    }
}
