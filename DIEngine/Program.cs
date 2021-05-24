using System;
using System.Collections.Generic;

namespace DIEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public class SimpleContainer
    {
        private static Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();
        private static Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        public void RegisterType<T>(bool Singleton) where T : class, new()
        {
            if (Singleton)
            {
                _singletons.Add(typeof(T), Activator.CreateInstance(typeof(T)));
            }
        }

        public void RegisterType<From, To>(bool Singleton) where To : From
        {
            _mappings.Add(typeof(From), typeof(To));
            if (Singleton)
            {
                _singletons.Add(typeof(From), Activator.CreateInstance(typeof(To)));
            }
        }

        public T Resolve<T>()
        {
            Type returningType = typeof(T);
            if(_singletons.ContainsKey(returningType))
            {
                return (T)_singletons[returningType];
            }
            if (_mappings.ContainsKey(returningType))
            {
                returningType = _mappings[returningType];
            }
            return (T)Activator.CreateInstance(returningType);
        }
    }
}
