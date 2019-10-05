using System;
using System.Collections.Concurrent;

namespace PacketMessagingTS.Core.Helpers
{
    public static class Singleton<T>
        where T : new()
    {
        private static ConcurrentDictionary<Type, T> _instances = new ConcurrentDictionary<Type, T>();

        public static T Instance
        {
            get
            {
                return _instances.GetOrAdd(typeof(T), (t) => new T());
            }
        }

        public static void UpdateInstance()
        {
            T newValue = new T();
            _instances.TryUpdate(typeof(T), newValue, Instance);
        }

        public static bool RemoveInstance()
        {
            return _instances.TryRemove(typeof(T), out T value);
        }

    }
}
