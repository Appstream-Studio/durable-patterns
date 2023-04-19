namespace AppStream.DurablePatterns
{
    internal static class TypeEnumerableExtensions
    {
        public static bool IsCollection(this Type collectionType)
        {
            if (collectionType.IsArray)
            {
                return false;
            }

            if (collectionType.IsInterface && collectionType.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return true;
            }

            foreach (Type interfaceType in collectionType.GetInterfaces())
            {
                if (interfaceType.IsGenericType
                    && interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsEnumerableOf(this Type collectionType, Type elementType)
        {
            return collectionType.GetElementType() == elementType;
        }

        public static Type? GetCollectionElementType(this Type collectionType)
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
