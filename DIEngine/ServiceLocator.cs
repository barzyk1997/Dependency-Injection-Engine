using System;

namespace DIEngine
{
    #region EXCEPTIONS
    public class UndefinedContainerProviderException : Exception { }
    #endregion

    #region Delegates
    public delegate SimpleContainer ContainerProviderDelegate();
    #endregion

    #region SERVICE LOCATOR
    class ServiceLocator
    {
        private static ServiceLocator _instance;
        private static object _lock = new object();

        private static ContainerProviderDelegate _providerDelegate;
        public static void SetContainerProvider(ContainerProviderDelegate ContainerProvider)
        {
            _providerDelegate = ContainerProvider;
        }

        public static ServiceLocator Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ServiceLocator();
                        }
                    }
                }
                return _instance;
            }
        }

        public T GetInstance<T>()
        {
            if (_providerDelegate != null)
            {
                return _providerDelegate().Resolve<T>();
            }
            else
            {
                throw new UndefinedContainerProviderException();
            }
        }
    }
    #endregion
}
