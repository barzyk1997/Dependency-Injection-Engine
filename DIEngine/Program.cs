using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


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

    public class DependencyProperty : Attribute { }

    public class DependencyMethod : Attribute { }
    #endregion

    #region MAPPINGS
    public interface Mapping
    {
        public object GetInstance(SimpleContainer container);
    }

    public class TypeMapping : Mapping
    {
        private Type t;

        public TypeMapping(Type t)
        {
            this.t = t;
        }

        public object GetInstance(SimpleContainer container)
        {
            return container.CreateInstance(t);
        }
    }

    public class InstanceMapping : Mapping
    {
        private object _instance;

        public InstanceMapping(object Instance)
        {
            _instance = Instance;
        }

        public object GetInstance(SimpleContainer container)
        {
            return _instance;
        }
    }
    #endregion

    #region IoC IMPLEMENTATION
    public class SimpleContainer
    {
        private Dictionary<Type, Mapping> _mappings = new Dictionary<Type, Mapping>();
        private HashSet<Type> _busy = new HashSet<Type>();

        public void RegisterType<T>(bool Singleton) where T : class
        {
            if (Singleton)
            {
                _mappings[typeof(T)] = new InstanceMapping(CreateInstance(typeof(T)));
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
                _mappings[typeof(From)] = new InstanceMapping(CreateInstance(typeof(To)));
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
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type t)
        {
            _busy.Add(t);
            object instance;
            if (_mappings.ContainsKey(t))
            {
                instance = _mappings[t].GetInstance(this);
            }
            else
            {
                if (t.IsAbstract || t.IsInterface)
                {
                    throw new MissingTypeException(t, "Nie zarejestrowano typu konkretnego dla typu: " + t.ToString());
                }
                instance = CreateInstance(t);
            }
            _busy.Remove(t);
            return instance;
        }

        public T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        public object CreateInstance(Type t)
        {
            var constructors = t.GetConstructors();
            var maxParams = constructors.Max((constructor) => constructor.GetParameters().Length);
            var candidates = constructors.Where((constructor) => constructor.GetParameters().Length == maxParams);

            foreach(var c in constructors)
            {
                if(c.GetCustomAttribute<DependencyConstructor>() != null)
                {
                    candidates = new ConstructorInfo[] { c };
                    break;
                }
            }

            foreach (var candidate in candidates)
            {
                try
                {
                    object[] parameters = candidate.GetParameters().Select((param) =>
                    {
                        if(_busy.Contains(param.ParameterType))
                        {
                            throw new Exception();
                        }
                        return Resolve(param.ParameterType);
                    }).ToArray();

                    object instance = Activator.CreateInstance(t, parameters);
                    BuildUp(t, instance);
                    return instance;
                }
                catch (Exception _) { }
            }

            throw new DependencyResolvingException();
        }

        public void BuildUp<T>(T instance)
        {
            BuildUp(typeof(T), instance);
        }

        public void BuildUp(Type t, object instance)
        {
            var properties = t.GetProperties();
            foreach(var prop in properties)
            {
                if(prop.GetCustomAttribute<DependencyProperty>() != null)
                {
                    if(_busy.Contains(prop.PropertyType))
                    {
                        throw new DependencyResolvingException();
                    }
                    prop.SetValue(instance, Resolve(prop.PropertyType));
                }
            }

            var methods = t.GetMethods();
            foreach(var method in methods)
            {
                if(method.GetCustomAttribute<DependencyMethod>() != null)
                {
                    object[] parameters = method.GetParameters().Select((param) =>
                    {
                        if (_busy.Contains(param.ParameterType))
                        {
                            throw new DependencyResolvingException();
                        }
                        return Resolve(param.ParameterType);
                    }).ToArray();

                    method.Invoke(instance, parameters);
                }
            }
        }
    }
    #endregion
}
