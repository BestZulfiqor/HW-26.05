namespace Infrastructure.Interfaces;

public interface IMemoryCacheService
{
    Task SetData<T>(string key, T value, int expirationMinutes);
    Task<T?> GetData<T>(string key);
    Task DeleteData(string key);
}
