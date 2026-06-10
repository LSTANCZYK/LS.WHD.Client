using LS.WHD.Client.Http;
using LS.WHD.Client.Models;

namespace LS.WHD.Client.Services;

internal sealed class CustomFieldService : ICustomFieldService
{
    private const string Resource = "customfields";
    private readonly WhdHttpClient _http;

    public CustomFieldService(WhdHttpClient http) => _http = http;

    public async Task<IReadOnlyList<CustomFieldDefinition>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _http.GetAsync<List<CustomFieldDefinition>>(Resource, cancellationToken: cancellationToken);
        return result ?? [];
    }

    public Task<CustomFieldDefinition?> GetAsync(long id, CancellationToken cancellationToken = default)
        => _http.GetAsync<CustomFieldDefinition>($"{Resource}/{id}", cancellationToken: cancellationToken);
}
