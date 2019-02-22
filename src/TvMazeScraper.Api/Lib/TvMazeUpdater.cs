using System;
using System.Threading;
using System.Threading.Tasks;

namespace TvMazeScraper.Api.Lib
{
    public class TvMazeUpdater : HostedService
    {
        private readonly ITvMazeApi _tvMazeApi;
        private readonly IShowStorage _showStorage;
        private readonly ITvMazeUpdateQueue _updateQueue;

        public TvMazeUpdater(ITvMazeApi tvMazeApi, IShowStorage showStorage, ITvMazeUpdateQueue updateQueue)
        {
            _tvMazeApi = tvMazeApi;
            _showStorage = showStorage;
            _updateQueue = updateQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    string showId;
                    if ((showId = _updateQueue.GetNextShowToUpdate()) != null)
                    {
                        // Get show from TvMaze API.
                        var show = await _tvMazeApi.GetShowAsync(showId, ct);

                        // Store or update show in storage.
                        await _showStorage.CreateOrUpdateShow(show, ct);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    // Wait some time before getting the next item of the queue.
                    await Task.Delay(TimeSpan.FromMilliseconds(100), ct);
                }
            }
        }
    }
}