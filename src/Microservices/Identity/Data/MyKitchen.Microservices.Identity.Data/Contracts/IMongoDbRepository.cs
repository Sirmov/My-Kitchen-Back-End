namespace MyKitchen.Microservices.Identity.Data.Contracts
{
    using MongoDB.Driver;

    public interface IMongoDbRepository<TDocument, TKey>
    {
        public IFindFluent<TDocument, TDocument> All(bool withDeleted);

        public TDocument Find(TKey id, bool withDeleted);

        public Task<TDocument> FindAsync(TKey id, bool withDeleted);

        public void Add(TDocument document);

        public Task AddAsync(TDocument document);

        public void AddRange(IEnumerable<TDocument> documents);

        public Task AddRangeAsync(IEnumerable<TDocument> documents);

        public ReplaceOneResult Update(TKey id, TDocument document);

        public Task<ReplaceOneResult> UpdateAsync(TKey id, TDocument document);

        public UpdateResult Delete(TKey id);

        public Task<UpdateResult> DeleteAsync(TKey id);

        public TDocument Undelete(TKey id);

        public Task<TDocument> UndeleteAsync(TKey id);

        public DeleteResult HardDelete(TKey id);

        public Task<DeleteResult> HardDeleteAsync(TKey id);
    }
}
