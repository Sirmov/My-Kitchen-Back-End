namespace MyKitchen.Microservices.Identity.Services.Common.QueryOptions
{
    public class QueryOptions<TDocument>
    {
        // public bool IsReadOnly { get; set; } = true;

        public bool WithDeleted { get; set; } = false;

        public List<OrderOption<TDocument>> OrderOptions { get; set; } = new List<OrderOption<TDocument>>();

        public int Skip { get; set; } = 0;

        public int Take { get; set; } = 10;
    }
}
