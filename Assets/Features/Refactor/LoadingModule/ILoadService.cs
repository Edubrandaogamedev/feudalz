using System.Threading.Tasks;

namespace Features.Refactor
{
    public interface ILoadService
    {
        public bool IsLoaded { get; }
        public Task<T> LoadFromAPI<T>(string path, string token) where T: class;
        public bool IsLoadingFromAPI(string path);
    }
}