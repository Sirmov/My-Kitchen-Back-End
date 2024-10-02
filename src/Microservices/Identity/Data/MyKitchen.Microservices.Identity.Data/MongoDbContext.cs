namespace MyKitchen.Microservices.Identity.Data
{
    using MongoDB.Driver;

    using MyKitchen.Microservices.Identity.Data.Contracts;

    public class MongoDbContext : IMongoDbContext
    {
        public MongoDbContext(IMongoClient mongoClient, IMongoDatabase mongoDatabase)
        {
            this.MongoClient = mongoClient;
            this.MongoDatabase = mongoDatabase;
        }

        public IMongoClient MongoClient { get; }

        public IMongoDatabase MongoDatabase { get; }

        public IMongoCollection<TDocument> Collection<TDocument>(string? collectionName = null)
        {
            string name = collectionName ?? typeof(TDocument).Name;

            return this.MongoDatabase.GetCollection<TDocument>(name);
        }
    }
}
