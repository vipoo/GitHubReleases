using System;

namespace GitHubReleases
{
    public class GitHubCachedReleases : GitHubCachedResult<RepoKey, VersionItem[]>
    {
        public VersionItem[] Data { get; set; }
        public RepoKey Key { get; set; }
        public DateTime RetreivedAt { get; set; }
    }
}
