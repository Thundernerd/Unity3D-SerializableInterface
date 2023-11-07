using System.Collections.Generic;
using System.Linq;

namespace TNRD
{
    public static class Extensions
    {
        /// <summary>
        /// Checks to see if the containing value is valid and returns it through the out parameter if so
        /// </summary>
        /// <param name="serializableInterface"></param>
        /// <param name="value">The containing value, if valid</param>
        /// <returns>True if the containing value is valid, false if not</returns>
        public static bool IsDefined<TInterface>(
            this SerializableInterface<TInterface> serializableInterface,
            out TInterface value
        )
            where TInterface : class
        {
            if (serializableInterface == null)
            {
                value = default;
                return false;
            }

            if (EqualityComparer<TInterface>.Default.Equals(serializableInterface.Value, default))
            {
                value = default;
                return false;
            }

            value = serializableInterface.Value;
            return true;
        }

        /// <inheritdoc cref="IsDefined{TInterface}"/>
        public static bool TryGetValue<TInterface>(
            this SerializableInterface<TInterface> serializableInterface,
            out TInterface value
        )
            where TInterface : class
        {
            return IsDefined(serializableInterface, out value);
        }

        /// <summary>
        /// Convert a IEnumerable of Interfaces to a List of SerializableInterfaces
        /// </summary>
        public static List<SerializableInterface<T>> ToSerializableInterfaceList<T>(this IEnumerable<T> list) where T : class
        {
            return list.Select(e => new SerializableInterface<T>(e)).ToList();
        }

         /// <summary>
        /// Convert a IEnumerable of Interfaces to an Array of SerializableInterfaces
        /// </summary>
        public static SerializableInterface<T>[] ToSerializableInterfaceArray<T>(this IEnumerable<T> list) where T : class
        {
            return list.Select(e => new SerializableInterface<T>(e)).ToArray();
        }
    }
}
