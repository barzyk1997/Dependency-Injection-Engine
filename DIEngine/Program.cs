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
            if(_singletons.ContainsKey(typeof(T)))
            {
                throw new Exception($"Singleton of type {typeof(T).Name} is already registered.");
            }
            else if (Singleton)
            {
                _singletons.Add(typeof(T), new T());
            }
            else
            {
                _singletons.Add(typeof(T), null);
            }
        }

        public void RegisterType<From, To>(bool Singleton) where To : From, new()
        {
            _mappings.Add(typeof(From), typeof(To));
            if (Singleton)
            {
                _singletons.Add(typeof(To), new To());
            }
            else
            {
                _singletons.Add(typeof(To), null);
            }
        }

        public T Resolve<T>() where T : new()
        {
            Type returningType = typeof(T);
            if(_mappings.ContainsKey(returningType))
            {
                returningType = _mappings[returningType];
            }
            if(_singletons.ContainsKey(returningType))
            {
                if (_singletons[returningType] == null)
                {
                    return new T();
                }
                return (T)_singletons[returningType];
            }
            throw new Exception($"Type {typeof(T).Name} is not registered.");
        }
    }
}
