using System;
using System.Collections.Generic;

namespace DIEngine
{
    #region EXCEPTIONS
    public class MissingTypeException : Exception
    {
        public Type missingType;

        public MissingTypeException(Type missingType, string message) : base(message)
        {
            this.missingType = missingType;
        }
    }

    public class DependencyResolvingException : Exception { }
    #endregion

    #region ATTRIBUTES
    public class DependencyConstructor : Attribute { }
    #endregion

    #region MAPPINGS
    public interface Mapping
    {
        public object GetInstance();
    }

    public class TypeMapping : Mapping
    {
        private Type t;

        public TypeMapping(Type t)
        {
            this.t = t;
        }

        public object GetInstance()
        {
            return Activator.CreateInstance(t);
        }
    }

    public class InstanceMapping : Mapping
    {
        private object _instance;

        public InstanceMapping(object Instance)
        {
            _instance = Instance;
        }

        public object GetInstance()
        {
            return _instance;
        }
    }
    #endregion

    #region IoC IMPLEMENTATION
    public class SimpleContainer
    {
        private Dictionary<Type, Mapping> _mappings = new Dictionary<Type, Mapping>();

        public void RegisterType<T>(bool Singleton) where T : class, new()
        {
            if (Singleton)
            {
                _mappings[typeof(T)] = new InstanceMapping(Activator.CreateInstance(typeof(T)));
            }
            else
            {
                _mappings[typeof(T)] = new TypeMapping(typeof(T));
            }
        }

        public void RegisterType<From, To>(bool Singleton) where To : From
        {
            if (Singleton)
            {
                _mappings[typeof(From)] = new InstanceMapping(Activator.CreateInstance(typeof(To)));
            }
            else
            {
                _mappings[typeof(From)] = new TypeMapping(typeof(To));

            }
        }

        public void RegisterInstance<T>(T instance)
        {
            _mappings[typeof(T)] = new InstanceMapping(instance);
        }

        public T Resolve<T>()
        {
            Type returningType = typeof(T);

            if (_mappings.ContainsKey(returningType))
            {
                return (T)_mappings[returningType].GetInstance();
            }

            if (returningType.IsAbstract || returningType.IsInterface)
            {
                throw new MissingTypeException(returningType, "Nie zarejestrowano typu konkretnego dla typu: " + returningType.ToString());
            }
            return (T)Activator.CreateInstance(returningType);
        }
    }
    #endregion
}
