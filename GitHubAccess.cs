using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubReleases
{
    internal static class GitHubAccess
    {
        static readonly GitHubClient Client;

        static GitHubAccess()
        {
            Client = new GitHubClient(new ProductHeaderValue("iracing-replay-director-installer"));
        }

        public async static Task<string> GetReleaseDownloadUrl(string user, string repo, string versionStamp)
        {
            var release = (await GetVersions(user, repo)).Where(r => r.VersionStamp == versionStamp).First();

            var assets = await Client.Release.GetAllAssets(user, repo, release.Id);
            var asset = assets.First();
            return asset.BrowserDownloadUrl;
        }

        public static async Task<VersionItem[]> GetVersions(string user, string repo)
        {
            var key = new RepoKey { Repo = repo, User = user };
            var allReleases = GitHubAccessSettings.GitHubCachedReleases == null ? new List<GitHubCachedReleases>() : GitHubAccessSettings.GitHubCachedReleases.ToList();

            //Clean data, due to previous bug of not reusing cached items.
            allReleases = allReleases.GroupBy(r => r.Key.Equals(key)).Select( r => r.First() ).ToList();

            var cacheHit = allReleases.FirstOrDefault(r => r.Key.Equals(key));

            if (cacheHit != null && cacheHit.RetreivedAt.AddHours(1) > DateTime.Now)
            {
                await Task.Delay(1).ConfigureAwait(true);
                return cacheHit.Data;
            }

            var releases = await Client.Release.GetAll(user, repo);

            var versionReleases = releases
                    .Select(r => new VersionItem { Prerelease = r.Prerelease, DateTimeStamp = r.CreatedAt.ToString(), VersionStamp = r.TagName, Id = r.Id })
                    .ToArray();

            if( cacheHit == null)
                allReleases.Add(new GitHubCachedReleases { Key = key, Data = versionReleases, RetreivedAt = DateTime.Now });
            else
            {
                cacheHit.Data = versionReleases;
                cacheHit.RetreivedAt = DateTime.Now;
            }

            GitHubAccessSettings.GitHubCachedReleases = allReleases.ToArray();
            GitHubAccessSettings.Save();

            return versionReleases;
        }
    }
}
