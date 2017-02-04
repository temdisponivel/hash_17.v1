namespace Hash17.Utils
{
    public class NonUnitySingleton<T>
        where T : NonUnitySingleton<T>
    {
        private static T _instance;
        public static T Instance;

        public NonUnitySingleton()
        {
            _instance = this as T;
        } 
    }
}