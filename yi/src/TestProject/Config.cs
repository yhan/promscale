using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace TestProject;

public class Config: IConfiguration
{
    public IConfigurationSection GetSection(string key)
    {
        throw new NotImplementedException();
    }
    public IEnumerable<IConfigurationSection> GetChildren()
    {
        throw new NotImplementedException();
    }
    public IChangeToken GetReloadToken()
    {
        throw new NotImplementedException();
    }
    public string this[string key]
    {
        get => store[key];
        set => store[key] = value;
    }

    private Dictionary<string, string> store = new();

    public void Add<T>(string key, T value)
    {
        this[key] = value.ToString();
    }
}
