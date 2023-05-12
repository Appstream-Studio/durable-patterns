namespace AppStream.DurablePatterns
{
    internal static class TypeEnumerableExtensions
    {
        public static bool IsGenericCollection(this Type collectionType)
        {
            if (collectionType.IsArray)
            {
                return false;
            }

            return GetCollectionElementType(collectionType) != null;
        }

        public static bool IsEnumerableOf(this Type collectionType, Type elementType)
        {
            return collectionType.GetElementType() == elementType;
        }

        public static Type? GetCollectionElementType(this Type collectionType)
        {
            if (collectionType.IsInterface && 
                collectionType.IsGenericType && 
                collectionType.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return collectionType.GetGenericArguments()[0];
            }

            foreach (Type interfaceType in collectionType.GetInterfaces())
            {
                if (interfaceType.IsGenericType
                    && interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }

            return null;
        }
    }
}
