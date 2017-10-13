using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.ComponentModel
{
    /// <summary>
    /// 提供了一种方便的方法实现锁定的资源访问
    /// </summary>
    public class WriteLockDisposable : IDisposable
    {
        private readonly ReaderWriterLockSlim _rwLock;

        public WriteLockDisposable(ReaderWriterLockSlim rwLock)
        {
            _rwLock = rwLock;
            _rwLock.EnterWriteLock();
        }

        public void Dispose()
        {
            _rwLock.ExitWriteLock();
        }
    }
}
