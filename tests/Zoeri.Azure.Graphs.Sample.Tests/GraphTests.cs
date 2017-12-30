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
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Zoeri.Azure.Graphs.Sample.Model;

namespace Zoeri.Azure.Graphs.Sample.Tests
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void UpdateDeletedVertexThrowsException()
        {
            UpdateDeletedVertexThrowsExceptionAsync().Wait();
        }

        public async Task UpdateDeletedVertexThrowsExceptionAsync()
        {
            using (var context = new TestContext())
            {
                var originalUser = context.GenerateTestUser();

                await context.Client.AddVertexAsync(context.Collection, originalUser,
                    context.CancellationTokenSource.Token);
                await context.Client.DeleteVertexAsync(context.Collection, originalUser.Id,
                    context.CancellationTokenSource.Token);
                var updatedUser = await context.Client.UpdateVertexAsync(context.Collection, originalUser,
                    context.CancellationTokenSource.Token);

                //This should be unreachable code in practice.
                Assert.IsNull(updatedUser);
            }
        }

        [TestMethod]
        public void GetNonExistentUserIsNull()
        {
            GetNonExistentUserIsNullAsync().Wait();
        }

        public async Task GetNonExistentUserIsNullAsync()
        {
            using (var context = new TestContext())
            {
                var nonExistentUserId = Guid.NewGuid().ToString().ToLower();
                var readUser = await context.Client.GetVertexAsync<User>(context.Collection, nonExistentUserId,
                    context.CancellationTokenSource.Token);

                Assert.IsNull(readUser);
            }
        }

        [TestMethod]
        public void ReadUserEmailEqualsInserted()
        {
            ReadUserEmailEqualsInsertedAsync().Wait();
        }

        public async Task ReadUserEmailEqualsInsertedAsync()
        {
            using (var context = new TestContext())
            {
                var originalUser = context.GenerateTestUser();

                await context.Client.AddVertexAsync(context.Collection, originalUser,
                    context.CancellationTokenSource.Token);
                var readUser = await context.Client.GetVertexAsync<User>(context.Collection, originalUser.Id,
                    context.CancellationTokenSource.Token);

                Assert.AreEqual(originalUser.Email, readUser.Email);
            }
        }

        [TestMethod]
        public void ReadUserBalanceEqualsUpdated()
        {
            ReadUserBalanceEqualsUpdatedAsync().Wait();
        }

        public async Task ReadUserBalanceEqualsUpdatedAsync()
        {
            using (var context = new TestContext())
            {
                var originalUser = context.GenerateTestUser();

                await context.Client.AddVertexAsync(context.Collection, originalUser,
                    context.CancellationTokenSource.Token);
                var expectedBalance = originalUser.Balance += 70;
                await context.Client.UpdateVertexAsync(context.Collection, originalUser,
                    context.CancellationTokenSource.Token);
                var readUser = await context.Client.GetVertexAsync<User>(context.Collection, originalUser.Id,
                    context.CancellationTokenSource.Token);

                Assert.AreEqual(expectedBalance, readUser.Balance);
            }
        }

        [TestMethod]
        public void ReadDeletedVertexIsNull()
        {
            ReadDeletedVertexIsNullAsync().Wait();
        }

        public async Task ReadDeletedVertexIsNullAsync()
        {
            using (var context = new TestContext())
            {
                var originalUser = context.GenerateTestUser();

                await context.Client.AddVertexAsync(context.Collection, originalUser,
                    context.CancellationTokenSource.Token);
                await context.Client.DeleteVertexAsync(context.Collection, originalUser.Id,
                    context.CancellationTokenSource.Token);
                var readUser = await context.Client.GetVertexAsync<User>(context.Collection, originalUser.Id,
                    context.CancellationTokenSource.Token);

                Assert.IsNull(readUser);
            }
        }
    }
}