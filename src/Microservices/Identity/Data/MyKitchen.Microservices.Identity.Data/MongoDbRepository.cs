namespace MyKitchen.Microservices.Identity.Data
{
    using System.Runtime.Versioning;

    using MongoDB.Driver;

    using MyKitchen.Common.Guard;
    using MyKitchen.Microservices.Identity.Data.Contracts;
    using MyKitchen.Microservices.Identity.Data.Models.Common;

    public class MongoDbRepository<TDocument, TKey> : IMongoDbRepository<TDocument, TKey>
        where TDocument : BaseDocument<TKey>
        where TKey : notnull
    {
        protected readonly IMongoDbContext mongoDbContext;

        protected readonly IMongoCollection<TDocument> mongoCollection;

        public MongoDbRepository(IMongoDbContext mongoDbContext)
        {
            this.mongoDbContext = mongoDbContext;
            this.mongoCollection = this.mongoDbContext.Collection<TDocument>();
        }

        public IFindFluent<TDocument, TDocument> All(bool withDeleted)
        {
            var filter = withDeleted ?
                Builders<TDocument>.Filter.Empty :
                Builders<TDocument>.Filter.Eq(d => d.IsDeleted, false);

            return this.mongoCollection.Find(filter);
        }

        /// <inheritdoc/>
        public TDocument Find(TKey id, bool withDeleted)
        {
            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            if (!withDeleted)
            {
                filter &= Builders<TDocument>.Filter.Eq(d => d.IsDeleted, false);
            }

            var document = this.mongoCollection.Find(filter).FirstOrDefault();

            return document;
        }

        /// <inheritdoc/>
        public async Task<TDocument> FindAsync(TKey id, bool withDeleted)
        {
            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            if (!withDeleted)
            {
                filter &= Builders<TDocument>.Filter.Eq(d => d.IsDeleted, false);
            }

            var document = await this.mongoCollection.Find(filter).FirstAsync();

            return document;
        }

        /// <inheritdoc/>
        public void Add(TDocument document)
        {
            Guard.AgainstNull(document, nameof(document), true);

            document.CreatedOn = DateTime.UtcNow;
            this.mongoCollection.InsertOne(document);
        }

        /// <inheritdoc/>
        public async Task AddAsync(TDocument document)
        {
            Guard.AgainstNull(document, nameof(document), true);

            document.CreatedOn = DateTime.UtcNow;
            await this.mongoCollection.InsertOneAsync(document);
        }

        /// <inheritdoc/>
        public void AddRange(IEnumerable<TDocument> documents)
        {
            Guard.AgainstNull(documents, nameof(documents), true);

            foreach (var document in documents)
            {
                Guard.AgainstNull(document, nameof(document));
                document.CreatedOn = DateTime.UtcNow;
            }

            this.mongoCollection.InsertMany(documents);
        }

        /// <inheritdoc/>
        public async Task AddRangeAsync(IEnumerable<TDocument> documents)
        {
            Guard.AgainstNull(documents, nameof(documents), true);

            foreach (var document in documents)
            {
                Guard.AgainstNull(document, nameof(document));
                document.CreatedOn = DateTime.UtcNow;
            }

            await this.mongoCollection.InsertManyAsync(documents);
        }

        /// <inheritdoc/>
        public ReplaceOneResult Update(TKey id, TDocument document)
        {
            Guard.AgainstNull(id, nameof(id), true);
            Guard.AgainstNull(document, nameof(document), true);

            if (!EqualityComparer<TKey>.Default.Equals(id, document.Id))
            {
                return new ReplaceOneResult.Acknowledged(0, 0, null);
            }

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            document.ModifiedOn = DateTime.UtcNow;

            return this.mongoCollection.ReplaceOne(filter, document);
        }

        /// <inheritdoc/>
        public async Task<ReplaceOneResult> UpdateAsync(TKey id, TDocument document)
        {
            Guard.AgainstNull(id, nameof(id), true);
            Guard.AgainstNull(document, nameof(document), true);

            if (!EqualityComparer<TKey>.Default.Equals(id, document.Id))
            {
                return new ReplaceOneResult.Acknowledged(0, 0, null);
            }

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            document.ModifiedOn = DateTime.UtcNow;

            return await this.mongoCollection.ReplaceOneAsync(filter, document);
        }

        /// <inheritdoc/>
        public UpdateResult Delete(TKey id)
        {
            Guard.AgainstNull(id, nameof(id), true);

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, true)
                .Set(d => d.DeletedOn, DateTime.UtcNow);

            return this.mongoCollection.UpdateOne(filter, update);
        }

        /// <inheritdoc/>
        public async Task<UpdateResult> DeleteAsync(TKey id)
        {
            Guard.AgainstNull(id, nameof(id), true);

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, true)
                .Set(d => d.DeletedOn, DateTime.UtcNow);

            return await this.mongoCollection.UpdateOneAsync(filter, update);
        }

        /// <inheritdoc/>
        public TDocument Undelete(TKey id)
        {
            Guard.AgainstNull(id, nameof(id), true);

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, false)
                .Set(d => d.DeletedOn, null);

            return this.mongoCollection.FindOneAndUpdate(filter, update);
        }

        /// <inheritdoc/>
        public async Task<TDocument> UndeleteAsync(TKey id)
        {
            Guard.AgainstNull(id, nameof(id), true);

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, false)
                .Set(d => d.DeletedOn, null);

            return await this.mongoCollection.FindOneAndUpdateAsync(filter, update);
        }

        /// <inheritdoc/>
        public DeleteResult HardDelete(TKey id)
        {
            Guard.AgainstNull(id, nameof(id), true);

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            return this.mongoCollection.DeleteOne(filter);
        }

        /// <inheritdoc/>
        public async Task<DeleteResult> HardDeleteAsync(TKey id)
        {
            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            return await this.mongoCollection.DeleteOneAsync(filter);
        }
    }
}
