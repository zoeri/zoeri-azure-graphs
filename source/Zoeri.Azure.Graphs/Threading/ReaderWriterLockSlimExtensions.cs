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
    /// <summary>
    /// Provides extension methods to streamline the consumption of <see cref="ReaderWriterLockSlim" />.
    /// </summary>
    internal static class ReaderWriterLockSlimExtensions
    {
        /// <summary>
        /// Gets a <see cref="ReaderLock" /> tied to the specified <paramref name="readerWriterLock" />.
        /// </summary>
        /// <param name="readerWriterLock">The underlying <see cref="ReaderWriterLockSlim" /> implementing the actual lock.</param>
        /// <returns>A <see cref="ReaderLock" /> with the ability to exit the lock.</returns>
        public static ReaderLock GetReaderLock(this ReaderWriterLockSlim readerWriterLock)
        {
            if (readerWriterLock == null) throw new ArgumentNullException(nameof(readerWriterLock));

            return new ReaderLock(readerWriterLock);
        }

        /// <summary>
        /// Gets a <see cref="WriterLock" /> tied to the specified <paramref name="readerWriterLock" />.
        /// </summary>
        /// <param name="readerWriterLock">The underlying <see cref="ReaderWriterLockSlim" /> implementing the actual lock.</param>
        /// <returns>A <see cref="WriterLock" /> with the ability to exit the lock.</returns>
        public static WriterLock GeWriterLock(this ReaderWriterLockSlim readerWriterLock)
        {
            if (readerWriterLock == null) throw new ArgumentNullException(nameof(readerWriterLock));

            return new WriterLock(readerWriterLock);
        }

        /// <summary>
        /// Gets a <see cref="UpgradeableReaderLock" /> tied to the specified <paramref name="readerWriterLock" />.
        /// </summary>
        /// <param name="readerWriterLock">The underlying <see cref="ReaderWriterLockSlim" /> implementing the actual lock.</param>
        /// <returns>A <see cref="UpgradeableReaderLock" /> with the ability to upgrade and exit the lock.</returns>
        public static UpgradeableReaderLock GetUpgradeableReaderLock(this ReaderWriterLockSlim readerWriterLock)
        {
            if (readerWriterLock == null) throw new ArgumentNullException(nameof(readerWriterLock));

            return new UpgradeableReaderLock(readerWriterLock);
        }
    }
}