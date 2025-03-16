namespace Pipeline.App.Service
{
    using System.Threading.Tasks;

    public interface ILogParser
    {
        Task ParseLogAsync(string path);
    }
}
