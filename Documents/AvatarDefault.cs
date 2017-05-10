using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace VueJsAspNetCoreSample.Documents
{
    public class AvatarDefault
    {
        [BsonId]
        public string Id { get; set; }
        public string Photo { get; set; }
    }
}
