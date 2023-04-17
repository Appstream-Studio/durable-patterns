namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    internal static class TypeEnumerableExtensions
    {
        public static bool IsEnumerableOf(this Type collectionType, Type elementType)
        {
            return collectionType.GetElementType() == elementType;
        }

        public static Type? GetElementType(this Type collectionType)
        {
            if (collectionType.IsInterface && collectionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return collectionType.GetGenericArguments()[0];
            }

            foreach (Type interfaceType in collectionType.GetInterfaces())
            {
                if (interfaceType.IsGenericType
                    && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }

            return null;
        }
    }
}
