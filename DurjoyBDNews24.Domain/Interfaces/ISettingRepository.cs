namespace DurjoyBDNews24.Domain.Interfaces;

public interface ISettingRepository
{
    Task<string?> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
}