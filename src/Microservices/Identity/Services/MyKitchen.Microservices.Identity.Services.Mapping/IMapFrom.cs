namespace MyKitchen.Microservices.Identity.Services.Mapping
{
    /// <summary>
    /// This interface is used to mark that the implementing class maps from <typeparamref name="TClass"/>.
    /// </summary>
    /// <typeparam name="TClass">The class that the implemented one maps from.</typeparam>
    public interface IMapFrom<TClass>
    {
    }
}
