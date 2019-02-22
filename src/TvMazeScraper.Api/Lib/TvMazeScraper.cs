using System;
using System.Threading;
using System.Threading.Tasks;

namespace TvMazeScraper.Api.Lib
{
    public class TvMazeScraper : HostedService
    {
        private readonly ITvMazeApi _tvMazeApi;
        private readonly IShowStorage _showStorage;
        private readonly ITvMazeUpdateQueue _tvMazeUpdateQueue;

        public TvMazeScraper(ITvMazeApi tvMazeApi, IShowStorage showStorage, ITvMazeUpdateQueue tvMazeUpdateQueue)
        {
            _tvMazeApi = tvMazeApi;
            _showStorage = showStorage;
            _tvMazeUpdateQueue = tvMazeUpdateQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    // Get show updates.
                    var showUpdates = await _tvMazeApi.GetShowUpdatesAsync(ct);

                    // Get shows from storage.
                    var shows = await _showStorage.GetShowsAsync(ct);

                    // Compare shows with updates.
                    foreach (var showUpdate in showUpdates)
                    {
                        var id = showUpdate.Key;

                        // Check if show is already known in storage.
                        if (shows.TryGetValue(id, out var show))
                        {
                            // Show is available from storage, compare updated timestamps.
                            if (showUpdate.Value > show.UpdatedTimestamp)
                            {
                                _tvMazeUpdateQueue.UpdateShow(id);
                            }
                        }
                        else
                        {
                            // Show is not available: fetch show.
                            _tvMazeUpdateQueue.UpdateShow(id);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromMinutes(30), ct);
                }
            }
        }
    }
}