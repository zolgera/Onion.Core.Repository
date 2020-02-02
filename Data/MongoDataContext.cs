using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repository.Data
{
    public class MongoDataContext
    {
        private static string MongoConnectionString => new DatabaseConfiguration().GetMongoConnectionString();

        public MongoDataContext()
        {
            var mongoUrl = new MongoUrl(MongoConnectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            mongoClientSettings.ClusterConfigurator = cb => {
                cb.Subscribe<CommandStartedEvent>(e => {
                   System.Diagnostics.Debug.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };
            InitializeGuidRepresentation();
            IMongoClient client = new MongoClient(mongoClientSettings);

            MongoDatabase = client.GetDatabase(mongoUrl.DatabaseName);
        }
        protected virtual void InitializeGuidRepresentation()
        {
            // by default, avoid legacy UUID representation: use Binary 0x04 subtype.
            MongoDefaults.GuidRepresentation = MongoDB.Bson.GuidRepresentation.Standard;
        }
        public IMongoDatabase MongoDatabase { get; }
    }
}
