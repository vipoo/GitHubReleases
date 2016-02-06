using System;

namespace GitHubReleases
{
    internal class GitHubCachedReleases : GitHubCachedResult<RepoKey, VersionItem[]>
    {
        public VersionItem[] Data { get; set; }
        public RepoKey Key { get; set; }
        public DateTime RetreivedAt { get; set; }
    }
}
