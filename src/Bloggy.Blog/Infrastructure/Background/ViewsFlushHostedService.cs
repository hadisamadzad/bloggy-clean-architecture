using Bloggy.Blog.Application.Interfaces;

namespace Bloggy.Blog.Infrastructure.Background;

using Microsoft.Extensions.Logging;

public class ViewsFlushHostedService(
    IViewMemoryRepository viewRepository,
    IRepositoryManager repository,
    ILogger<ViewsFlushHostedService> logger
    ) : BackgroundService
{
    private readonly IViewMemoryRepository _viewRepository = viewRepository;
    private readonly IRepositoryManager _repository = repository;
    private readonly ILogger<ViewsFlushHostedService> _logger = logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var pending = await _viewRepository.GetPendingArticleIdsAsync();
                foreach (var articleId in pending)
                {
                    var count = await _viewRepository.AtomicPopCountAsync(articleId);
                    if (count <= 0) continue;

                    _logger.LogInformation("Applying {Count} views to article {ArticleId}", count, articleId);

                    // Increment article views atomically in MongoDB
                    await _repository.Articles.IncrementViewsAsync(articleId, count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while flushing views");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
