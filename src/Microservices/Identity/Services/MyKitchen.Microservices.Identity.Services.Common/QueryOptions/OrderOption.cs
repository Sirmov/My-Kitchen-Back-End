namespace MyKitchen.Microservices.Identity.Services.Common.QueryOptions
{
    using System.Linq.Expressions;

    using MyKitchen.Common.Constants;

    public class OrderOption<TClass>
    {
        public OrderOption()
        {
            this.PropertyName = string.Empty;
            this.PropertyLambda = x => this.GetProperty(x!, this.PropertyName);
        }

        public OrderOption(string propertyName, OrderByOrder order)
        {
            this.PropertyName = propertyName;
            this.PropertyLambda = x => this.GetProperty(x!, propertyName);
            this.Order = order;
        }

        public OrderOption(Expression<Func<TClass, object>> propertyLambda, OrderByOrder order)
        {
            this.PropertyName = string.Empty;
            this.PropertyLambda = propertyLambda;
            this.Order = order;
        }

        public string PropertyName { get; set; }

        public Expression<Func<TClass, object>> PropertyLambda { get; set; }

        public OrderByOrder Order { get; set; }

        private object GetProperty(object obj, string propertyName)
        {
            Type type = obj.GetType();
            var property = type.GetProperty(propertyName);

            if (property != null)
            {
                return property;
            }
            else
            {
                throw new ArgumentException(string.Format(ExceptionMessages.PropertyNotFound, propertyName, type.FullName));
            }
        }

    }
}
