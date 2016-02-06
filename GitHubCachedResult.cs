using System;

namespace GitHubReleases
{
    internal interface GitHubCachedResult<T1, T2>
    {
        DateTime RetreivedAt { get; set; }
        T1 Key { get; set; }
        T2 Data { get; set; }
    }
}
