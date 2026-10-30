using Microsoft.Extensions.Hosting;

namespace Serenity.Testing;

public class TestApplicationLifetime : IHostApplicationLifetime
{
    private readonly CancellationTokenSource _startedSource = new();
    private readonly CancellationTokenSource _stoppingSource = new();
    private readonly CancellationTokenSource _stoppedSource = new();

    public CancellationToken ApplicationStarted => _startedSource.Token;
    public CancellationToken ApplicationStopping => _stoppingSource.Token;
    public CancellationToken ApplicationStopped => _stoppedSource.Token;

    public void StopApplication()
    {
        _stoppingSource.Cancel(throwOnFirstException: false);
    }

    public void Start()
    {
        _startedSource.Cancel(throwOnFirstException: false);
    }
}
