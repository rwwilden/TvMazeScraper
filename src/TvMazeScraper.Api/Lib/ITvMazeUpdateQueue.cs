namespace TvMazeScraper.Api.Lib
{
    public interface ITvMazeUpdateQueue
    {
        void UpdateShow(string showId);

        string GetNextShowToUpdate();
    }
}