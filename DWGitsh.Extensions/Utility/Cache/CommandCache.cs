using DWPowerShell.Utility.Cache;

namespace DWGitsh.Extensions.Utility.Cache
{
    public interface ICommandCache : ICacheContainer
    {

    }

    public class CommandCache : CacheContainer, ICommandCache
    {
    }
}
