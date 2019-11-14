using System;
using System.Collections.Concurrent;

namespace Comm100.Framework.Security
{
    public class RequestsCounter
    {
        private readonly ConcurrentDictionary<string, NumOfRequests> _requests = new ConcurrentDictionary<string, NumOfRequests>();
        private readonly TimeSpan _timeout;

        public RequestsCounter(TimeSpan timeout)
        {
            this._timeout = timeout;
        }

        // increate number of requests
        // reset number of requests when after timeout
        // return current number of requests
        public int Increase(string key)
        {
            return _requests.AddOrUpdate(key,
                (_) => new NumOfRequests(),
                (_, item) =>
                    item.IsTimeout(_timeout)
                        ? new NumOfRequests()
                        : new NumOfRequests(item.cnt + 1, item.start)
                    ).cnt;
        }

        public void Clear()
        {
            foreach (var item in _requests)
            {
                if (item.Value.IsTimeout(_timeout))
                {
                    _requests.TryRemove(item.Key, out _);
                }
            }
        }

        // This is immutable, no need to lock
        class NumOfRequests
        {
            public NumOfRequests()
            {
                cnt = 1;
                start = DateTime.UtcNow;
            }

            public NumOfRequests(int cnt, DateTime start)
            {
                this.cnt = cnt;
                this.start = start;
            }

            public bool IsTimeout(TimeSpan timeout)
            {
                return DateTime.UtcNow - start > timeout;
            }

            public readonly int cnt;
            public readonly DateTime start;
        }
    }
}