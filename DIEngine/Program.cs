using System;
using System.Collections.Generic;

namespace DIEngine
{
    public class MissingTypeException : Exception
    {
        public Type missingType;

        public MissingTypeException(Type missingType, string message) : base(message)
        {
            this.missingType = missingType;
        }
    }
    
    public class SimpleContainer
    {
        private Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();
        private Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        public void RegisterType<T>(bool Singleton) where T : class, new()
        {
            if (Singleton)
            {
                _singletons[typeof(T)] = Activator.CreateInstance(typeof(T));
            }
        }

        public void RegisterType<From, To>(bool Singleton) where To : From
        {
            _mappings[typeof(From)] = typeof(To);
            if (Singleton)
            {
                _singletons[typeof(From)] = Activator.CreateInstance(typeof(To));
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
            if (returningType.IsAbstract || returningType.IsInterface)
            {
                throw new MissingTypeException(returningType, "Nie zarejestrowano typu konkretnego dla typu: " + returningType.ToString());
            }
            return (T)Activator.CreateInstance(returningType);
        }
    }
}
