namespace LS.WHD.Client.Mcp;

/// <summary>
/// Wraps <see cref="WhdClient"/> and ensures that the metadata cache (used for
/// name-based priority/request-type resolution) is initialised exactly once before
/// the first tool call that needs it.
/// </summary>
internal sealed class WhdClientService : IDisposable
{
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _metadataInitialized;

    public WhdClientService(WhdClient client) => Client = client;

    public WhdClient Client { get; }

    /// <summary>
    /// Ensures the metadata cache has been populated. Safe to call from multiple
    /// concurrent tool invocations — the HTTP fetch runs only once.
    /// </summary>
    public async Task EnsureMetadataAsync(CancellationToken cancellationToken = default)
    {
        if (_metadataInitialized)
            return;

        await _initLock.WaitAsync(cancellationToken);
        try
        {
            if (!_metadataInitialized)
            {
                await Client.InitializeMetadataCacheAsync(cancellationToken);
                _metadataInitialized = true;
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    public void Dispose()
    {
        _initLock.Dispose();
        Client.Dispose();
    }
}
