using System;
using System.Collections.Generic;
using System.Text;

namespace DIEngine
{
    #region EXCEPTIONS
    public class UndefinedServiceProviderException : Exception { }
    #endregion


    class LocalFactory<T>
    {
        private static Func<T> _provider;
        
        public T CreateService()
        {
            if(_provider != null)
            {
                return _provider();
            }
            throw new UndefinedServiceProviderException();
        }

        public void SetProvider(Func<T> provider)
        {
            _provider = provider;
        }
    }
}
