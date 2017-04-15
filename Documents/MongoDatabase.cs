using MongoDB.Driver;

namespace VueJsAspNetCoreSample.Documents {
    public class MongoDatabase {
        private MongoClient _client;
        private IMongoDatabase _db;

        const string DatabaseName = "faceaccess";
        private class Collections {
            public const string Persons = "persons";
        }
        public MongoDatabase (string hostName, int port) {
            _client = new MongoClient (new MongoClientSettings {
                Server = new MongoServerAddress (hostName, port)
            });
            _db = _client.GetDatabase (DatabaseName);
        }

        public IMongoCollection<PersonDocument> Persons {
            get { return _db.GetCollection<PersonDocument> (Collections.Persons); }
        }
    }
}
