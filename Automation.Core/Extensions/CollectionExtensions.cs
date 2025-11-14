namespace Automation.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddIfNotNull<T>(this ICollection<T> collection, T? item) where T : class
        {
            if (item != null)
            {
                collection.Add(item);
            }
        }
    }
}
