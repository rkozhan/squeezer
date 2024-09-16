using Microsoft.JSInterop;

namespace Squeezer.Client.Services;

public class SessionStorageService
{
    public const string LongUrlKey = "long-url";
    private IJSRuntime _jsRuntime;

    public SessionStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveAsync(string key, string value)
    {
        await _jsRuntime.InvokeVoidAsync("window.sessionStorage.setItem", key , value);
    } 

    public async Task<string?> GetAsync(string key)
    {
        return await _jsRuntime.InvokeAsync<string?>("window.sessionStorage.getItem", key);
    }

    public async Task RemoveAsync(string key)
    {
        await _jsRuntime.InvokeVoidAsync("window.sessionStorage.removeItem", key);
    }
}