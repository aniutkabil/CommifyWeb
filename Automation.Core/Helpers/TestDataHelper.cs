namespace Automation.Core.Helpers
{
    public static class TestDataHelper
    {
        private static readonly Random _random = new();

        public static T GetRandomItem<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("List cannot be null or empty", nameof(list));

            int index = _random.Next(list.Count);
            return list[index];
        }
    }
}
