namespace MyKitchen.Microservices.Identity.Data.Contracts
{
    using MongoDB.Driver;

    using MyKitchen.Microservices.Identity.Data.Common;

    public interface IMongoDbRepository<TDocument, TKey>
    {
        public IFindFluent<TDocument, TDocument> All(bool withDeleted);

        public QueryResult<TDocument> Find(TKey id, bool withDeleted);

        public Task<QueryResult<TDocument>> FindAsync(TKey id, bool withDeleted);

        public QueryResult Add(TDocument document);

        public Task<QueryResult> AddAsync(TDocument document);

        public QueryResult AddRange(IEnumerable<TDocument> documents);

        public Task<QueryResult> AddRangeAsync(IEnumerable<TDocument> documents);

        public QueryResult<ReplaceOneResult> Update(TKey id, TDocument document);

        public Task<QueryResult<ReplaceOneResult>> UpdateAsync(TKey id, TDocument document);

        public QueryResult<UpdateResult> Delete(TKey id);

        public Task<QueryResult<UpdateResult>> DeleteAsync(TKey id);

        public QueryResult<TDocument> Undelete(TKey id);

        public Task<QueryResult<TDocument>> UndeleteAsync(TKey id);

        public QueryResult<DeleteResult> HardDelete(TKey id);

        public Task<QueryResult<DeleteResult>> HardDeleteAsync(TKey id);
    }
}
