namespace MyKitchen.Microservices.Identity.Data.Models.Common
{
    using MongoDB.Bson.Serialization.Attributes;

    using MyKitchen.Microservices.Identity.Data.Models.Contracts;

    public abstract class BaseDocument<TKey> : IAuditInfo, IDeletableDocument
        where TKey : notnull
    {
        [BsonId]
        public TKey Id { get; set; } = default!;

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
