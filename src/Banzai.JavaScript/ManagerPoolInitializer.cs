using ClearScript.Manager;

namespace Banzai.JavaScript
{
    public static class ManagerPoolInitializer
    {
        private static bool _isInitialized;
        private static readonly object _syncLock = new object();

        public static void Initialize()
        {
            if (!_isInitialized)
            {
                lock (_syncLock)
                {
                    if (!_isInitialized)
                    {
                        //ManagerSettings should contain the maximum pool count
                        ManagerPool.InitializeCurrentPool(new ManagerSettings());
                        _isInitialized = true;
                    }
                }
            }
        }
    }
}