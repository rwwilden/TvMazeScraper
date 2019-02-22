using System.Collections.Concurrent;

namespace TvMazeScraper.Api.Lib
{
    public class TvMazeUpdateQueue : ITvMazeUpdateQueue
    {
        private readonly ConcurrentQueue<string> _showFetchQueue = new ConcurrentQueue<string>();

        public void UpdateShow(string showId)
        {
            _showFetchQueue.Enqueue(showId);
        }

        public string GetNextShowToUpdate()
        {
            return _showFetchQueue.TryDequeue(out var showId) ? showId : null;
        }
    }
}