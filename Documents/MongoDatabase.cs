using MongoDB.Driver;

namespace VueJsAspNetCoreSample.Documents {
    public class MongoDatabase {
        private MongoClient _client;
        private IMongoDatabase _db;

        const string DatabaseName = "faceaccess";
        private class Collections {
            public const string Persons = "persons";
        }
        public MongoDatabase (string connectionString) {
           var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            _client = new MongoClient (settings);
            _db = _client.GetDatabase (DatabaseName);
        }

        public IMongoCollection<PersonDocument> Persons {
            get { return _db.GetCollection<PersonDocument> (Collections.Persons); }
        }
    }
}
