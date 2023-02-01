﻿// Copyright 2021 The NATS Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NATS.Client.KeyValue
{
    public interface IKeyValueAsync
    {
        /// <summary>
        /// The name of the bucket
        /// </summary>
        string BucketName { get; }

        /// <summary>
        /// Get the entry for a key
        /// when the key exists and is live (not deleted and not purged)
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The entry</returns>
        Task<KeyValueEntry> GetAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the specific revision of an entry for a key
        /// when the key exists and is live (not deleted and not purged)
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="revision">the specific revision</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The entry</returns>
        Task<KeyValueEntry> GetAsync(string key, ulong revision, CancellationToken cancellationToken = default);

        /// <summary>
        /// Put a byte[] as the value for a key
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the bytes of the value</param>
        /// <returns>the revision number for the key</returns>
        Task<ulong> PutAsync(string key, byte[] value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Put a string as the value for a key
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the UTF-8 string</param>
        /// <returns>the revision number for the key</returns>
        Task<ulong> PutAsync(string key, string value, CancellationToken cancellationToken = default);

        /// <summary>
        ///Put a long as the value for a key
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the number</param>
        /// <returns>the revision number for the key</returns>
        Task<ulong> PutAsync(string key, long value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Put as the value for a key iff the key does not exist (there is no history)
        /// or is deleted (history shows the key is deleted)
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the bytes of the value</param>
        /// <returns>the revision number for the key</returns>
        Task<ulong> CreateAsync(string key, byte[] value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Put as the value for a key iff the key exists and its last revision matches the expected
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the bytes of the value</param>
        /// <param name="expectedRevision"></param>
        /// <returns>the revision number for the key</returns>
        Task<ulong> UpdateAsync(string key, byte[] value, ulong expectedRevision, CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft deletes the key by placing a delete marker. 
        /// </summary>
        /// <param name="key">the key</param>
        Task DeleteAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Purge all values/history from the specific key. 
        /// </summary>
        /// <param name="key">the key</param>
        Task PurgeAsync(string key, CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Watch updates for a specific key
        ///// </summary>
        ///// <param name="key">the key</param>
        ///// <param name="watcher">the watcher</param>
        ///// <param name="watchOptions">the watch options to apply. If multiple conflicting options are supplied, the last options wins.</param>
        ///// <returns></returns>
        //KeyValueWatchSubscription Watch(string key, IKeyValueWatcher watcher, params KeyValueWatchOption[] watchOptions);

        ///// <summary>
        ///// Watch updates for all keys
        ///// </summary>
        ///// <param name="watcher">the watcher</param>
        ///// <param name="watchOptions">the watch options to apply. If multiple conflicting options are supplied, the last options wins.</param>
        ///// <returns>The KeyValueWatchSubscription</returns>
        //KeyValueWatchSubscription WatchAll(IKeyValueWatcher watcher, params KeyValueWatchOption[] watchOptions);

        ///// <summary>
        ///// Get a list of the keys in a bucket.
        ///// </summary>
        ///// <returns>The list of keys</returns>
        //Task<IList<string>> KeysAsync(CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Get the history (list of KeyValueEntry) for a key
        ///// </summary>
        ///// <param name="key">the key</param>
        ///// <returns>The list of KeyValueEntry</returns>
        //Task<IList<KeyValueEntry>> HistoryAsync(string key, CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Remove history from all keys that currently are deleted or purged,
        ///// using a default KeyValuePurgeOptions
        ///// </summary>
        //Task PurgeDeletesAsync(CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Remove history from all keys that currently are deleted or purged
        ///// </summary>
        //Task PurgeDeletesAsync(KeyValuePurgeOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the KeyValueStatus object
        /// </summary>
        /// <returns>the status object</returns>
        Task<KeyValueStatus> StatusAsync(CancellationToken cancellationToken = default);
    }
}