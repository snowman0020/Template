namespace Template.Helper.DataCache
{
    public interface IDataCache
    {
        public T? GetData<T>(string key);
        void SetData<T>(string key, T data);
    }
}