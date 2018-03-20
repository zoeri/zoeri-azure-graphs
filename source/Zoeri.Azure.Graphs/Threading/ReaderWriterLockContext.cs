#region License

// The MIT License (MIT)
// 
// Copyright © 2018 Zoeri
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Threading;

namespace Zoeri.Azure.Graphs.Threading
{
    /// <inheritdoc />
    /// <summary>
    /// Provides common functionality for classes working with <see cref="ReaderWriterLockSlim" />.
    /// </summary>
    public abstract class ReaderWriterLockContext
        : IDisposable
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instances of a derived type using a new instance of <see cref="T:System.Threading.ReaderWriterLockSlim" /> created using
        /// <see cref="F:System.Threading.LockRecursionPolicy.SupportsRecursion" />.
        /// </summary>
        protected ReaderWriterLockContext()
            : this(new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion))
        {
        }

        /// <summary>
        /// Creates a new instances of a derived type using the specified <see cref="ReaderWriterLockSlim" />.
        /// </summary>
        /// <param name="readerWriterLock">The actual <see cref="ReaderWriterLockSlim" /> controlling the lock.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="readerWriterLock" /> an exception will be thrown.</exception>
        protected ReaderWriterLockContext(ReaderWriterLockSlim readerWriterLock)
        {
            Lock = readerWriterLock ?? throw new ArgumentNullException(nameof(readerWriterLock));
        }

        /// <summary>
        /// Gets the <see cref="ReaderWriterLockSlim" /> that underlies this instance.
        /// </summary>
        public ReaderWriterLockSlim Lock
        {
            get;
        }

        /// <inheritdoc />
        /// <summary>
        /// Releases the lock.
        /// </summary>
        public abstract void Dispose();
    }
}