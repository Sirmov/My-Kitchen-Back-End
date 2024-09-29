namespace MyKitchen.Microservices.Identity.Data.Models.Contracts
{
    public interface IDeletableDocument
    {
        bool IsDeleted { get; set; }

        DateTime? DeletedOn { get; set; }
    }
}
