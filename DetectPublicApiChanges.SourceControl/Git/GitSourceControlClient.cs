﻿using System;
using System.IO;
using System.Linq;
using DetectPublicApiChanges.SourceControl.Common;
using DetectPublicApiChanges.SourceControl.Interfaces;
using DetectPublicApiChanges.SourceControl.Subversion;
using LibGit2Sharp;

namespace DetectPublicApiChanges.SourceControl.Git
{
    /// <summary>
    /// SubversionSourceControlClient encapsulates a git client to access git repositories
    /// </summary>
    /// <seealso cref="ISourceControlClient" />
    public class GitSourceControlClient : ISourceControlClient
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public SourceControlType Type => SourceControlType.Git;

        /// <summary>
        /// Checks out the source code by using credentials.
        /// </summary>
        /// <param name="repositoryUrl">The repository URL.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="revision">The revision.</param>
        /// <param name="credentials">The credentials.</param>
        public void CheckOut(Uri repositoryUrl, DirectoryInfo localFolder, string revision, ISourceControlCredentials credentials = null)
        {
            //Create repository
            Repository.Init(localFolder.FullName);

            //Fetch & Checkout
            using (var repo = new Repository(localFolder.FullName))
            {
                AddOrUpdateRemote(repo, "origin", repositoryUrl);

                var fetchOptions = new FetchOptions
                {
                    CredentialsProvider = (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials
                        {
                            Username = credentials.User,
                            Password = credentials.Password
                        }
                };

                foreach (var remote in repo.Network.Remotes)
                {
                    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                    Commands.Fetch(repo, remote.Name, refSpecs, fetchOptions, string.Empty);
                }

                var commit = repo.Lookup<Commit>(revision);
                Commands.Checkout(repo, commit);
            }
        }

        /// <summary>
        /// Gets the change log.
        /// </summary>
        /// <param name="repositoryUrl">The repository URL.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="startRevision">The start revision.</param>
        /// <param name="endRevision">The end revision.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        public ISourceControlChangeLog GetChangeLog(Uri repositoryUrl, DirectoryInfo localFolder, string startRevision, string endRevision,
            ISourceControlCredentials credentials = null)
        {
            CheckOut(repositoryUrl, localFolder, endRevision, credentials);

            var log = new SourceControlChangeLog(startRevision, endRevision);

            using (var repo = new Repository(localFolder.FullName))
            {
                var filter = new CommitFilter
                {
                    IncludeReachableFrom = endRevision,
                    ExcludeReachableFrom = startRevision
                };

                foreach (var commit in repo.Commits.QueryBy(filter))
                {
                    log.AddItem(new SourceControlChangeLogItem(commit.Author.Name, commit.Message, commit.Author.When.DateTime));
                }
            }

            return log;
        }

        /// <summary>
        /// Updates the remote if it  exists, or creates it
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="name">The name.</param>
        /// <param name="repositoryUrl">The repository URL.</param>
        private static void AddOrUpdateRemote(IRepository repository, string name, Uri repositoryUrl)
        {
            var remote = repository.Network.Remotes.FirstOrDefault(r => r.Name.Equals(name));

            if (remote == null)
            {
                repository.Network.Remotes.Add(name, repositoryUrl.OriginalString);
            }
            else
            {
                repository.Network.Remotes.Update(remote.Name, r => r.Url = repositoryUrl.OriginalString);
            }
        }
    }
}
