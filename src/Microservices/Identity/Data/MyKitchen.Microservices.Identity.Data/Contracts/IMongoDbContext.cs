namespace MyKitchen.Microservices.Identity.Data.Contracts
{
    using MongoDB.Driver;

    public interface IMongoDbContext
    {
        public IMongoClient MongoClient { get; }

        public IMongoDatabase MongoDatabase { get; }

        public IMongoCollection<TDocument> Collection<TDocument>(string? collectionName = null);
    }
}
