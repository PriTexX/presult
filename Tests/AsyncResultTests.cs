using FluentAssertions;
using PResult;

namespace Tests
{
    public class AsyncResultTests
    {
        [Fact]
        public async Task MatchTransformsOnSuccess()
        {
            AsyncResult<int, string> asyncOk = Result.Ok<int, string>(2);
            var result = await asyncOk.Match(v => v * 3, _ => -1);
            result.Should().Be(6);
        }

        [Fact]
        public async Task MatchReturnsFallbackOnError()
        {
            AsyncResult<int, string> asyncErr = Result.Err<int, string>("fail");
            var result = await asyncErr.Match(v => v * 3, _ => -1);
            result.Should().Be(-1);
        }

        [Fact]
        public async Task ThenChainsSynchronouslyOnSuccess()
        {
            AsyncResult<int, string> asyncOk = Result.Ok<int, string>(3);
            var chained = asyncOk.Then(v => Result.Ok<string, string>((v * 2).ToString()));
            var result = await chained;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be("6");
        }

        [Fact]
        public async Task ThenSkipsSynchronouslyOnError()
        {
            AsyncResult<int, string> asyncErr = Result.Err<int, string>("oops");
            var chained = asyncErr.Then(v => Result.Ok<string, string>((v * 2).ToString()));
            var result = await chained;
            result.IsErr().Should().BeTrue();
            result.UnsafeError.Should().Be("oops");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ThenAsyncChainsOrSkipsCorrectly(bool useTaskOverload)
        {
            AsyncResult<int, string> asyncSource = Result.Ok<int, string>(4);
            AsyncResult<string, string> asyncChained = useTaskOverload
                ? asyncSource.ThenAsync(v =>
                    Task.FromResult(Result.Ok<string, string>((v * 5).ToString()))
                )
                : asyncSource.ThenAsync(v => Result.OkAsync<string, string>((v * 5).ToString()));

            var result = await asyncChained;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be("20");

            // now test skip on error
            AsyncResult<int, string> asyncError = Result.Err<int, string>("err");
            asyncChained = useTaskOverload
                ? asyncError.ThenAsync(v =>
                    Task.FromResult(Result.Ok<string, string>((v * 5).ToString()))
                )
                : asyncError.ThenAsync(v => Result.OkAsync<string, string>((v * 5).ToString()));

            result = await asyncChained;
            result.IsErr().Should().BeTrue();
            result.UnsafeError.Should().Be("err");
        }

        [Fact]
        public async Task ThenErrHandlesSynchronouslyOnError()
        {
            AsyncResult<int, string> asyncErr = Result.Err<int, string>("fixme");
            var recovered = asyncErr.ThenErr(_ => Result.Ok<int, string>(99));
            var result = await recovered;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be(99);
        }

        [Fact]
        public async Task ThenErrSkipsSynchronouslyOnSuccess()
        {
            AsyncResult<int, string> asyncOk = Result.Ok<int, string>(7);
            var unchanged = asyncOk.ThenErr(_ => Result.Ok<int, string>(-1));
            var result = await unchanged;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be(7);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ThenErrAsyncHandlesOrSkipsCorrectly(bool useTaskOverload)
        {
            AsyncResult<int, string> asyncSource = Result.Err<int, string>("bad");
            AsyncResult<int, string> asyncHandled = useTaskOverload
                ? asyncSource.ThenErrAsync(_ => Task.FromResult(Result.Ok<int, string>(47)))
                : asyncSource.ThenErrAsync(_ => Result.OkAsync<int, string>(47));

            var result = await asyncHandled;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be(47);

            // now test skip on success
            AsyncResult<int, string> asyncOk = Result.Ok<int, string>(8);
            asyncHandled = useTaskOverload
                ? asyncOk.ThenErrAsync(_ => Task.FromResult(Result.Err<int, string>("ignored")))
                : asyncOk.ThenErrAsync(_ => Result.ErrAsync<int, string>("ignored"));

            result = await asyncHandled;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be(8);
        }

        [Fact]
        public async Task MapTransformsSynchronouslyOnSuccess()
        {
            AsyncResult<int, string> asyncOk = Result.Ok<int, string>(5);
            var mapped = asyncOk.Map(v => v + 2);
            var result = await mapped;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be(7);
        }

        [Fact]
        public async Task MapPreservesErrorSynchronously()
        {
            AsyncResult<int, string> asyncErr = Result.Err<int, string>("fail");
            var mapped = asyncErr.Map(v => v + 2);
            var result = await mapped;
            result.IsErr().Should().BeTrue();
            result.UnsafeError.Should().Be("fail");
        }

        [Fact]
        public async Task MapAsyncTransformsOnSuccess()
        {
            AsyncResult<int, string> asyncOk = Result.Ok<int, string>(6);
            var mapped = asyncOk.MapAsync(v => Task.FromResult(v * 3));
            var result = await mapped;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be(18);
        }

        [Fact]
        public async Task MapAsyncPreservesError()
        {
            AsyncResult<int, string> asyncErr = Result.Err<int, string>("oops");
            var mapped = asyncErr.MapAsync(v => Task.FromResult(v * 3));
            var result = await mapped;
            result.IsErr().Should().BeTrue();
            result.UnsafeError.Should().Be("oops");
        }

        [Fact]
        public async Task MapErrTransformsSynchronouslyOnError()
        {
            AsyncResult<int, string> asyncErr = Result.Err<int, string>("error");
            var mappedErr = asyncErr.MapErr(e => e.Length);
            var result = await mappedErr;
            result.IsErr().Should().BeTrue();
            result.UnsafeError.Should().Be(5);
        }

        [Fact]
        public async Task MapErrPreservesValueSynchronously()
        {
            AsyncResult<int, string> asyncOk = Result.Ok<int, string>(11);
            var mappedErr = asyncOk.MapErr(e => e.Length);
            var result = await mappedErr;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be(11);
        }

        [Fact]
        public async Task MapErrAsyncTransformsOnError()
        {
            AsyncResult<int, string> asyncErr = Result.Err<int, string>("fail");
            var mappedErr = asyncErr.MapErrAsync(e => Task.FromResult(e.Length));
            var result = await mappedErr;
            result.IsErr().Should().BeTrue();
            result.UnsafeError.Should().Be(4);
        }

        [Fact]
        public async Task MapErrAsyncPreservesValue()
        {
            AsyncResult<int, string> asyncOk = Result.Ok<int, string>(12);
            var mappedErr = asyncOk.MapErrAsync(e => Task.FromResult(e.Length));
            var result = await mappedErr;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be(12);
        }

        [Theory]
        [InlineData(true, 20, 0)]
        [InlineData(false, 0, 99)]
        public async Task ValueOrReturnsValueOrFallback(bool isSuccess, int value, int fallback)
        {
            AsyncResult<int, string> async = isSuccess
                ? Result.Ok<int, string>(value)
                : Result.Err<int, string>("err");
            var result = await async.ValueOr(fallback);
            result.Should().Be(isSuccess ? value : fallback);
        }

        [Fact]
        public async Task UnsafeValueThrowsOnError()
        {
            AsyncResult<int, string> asyncErr = Result.Err<int, string>("err");
            Func<Task> act = () => asyncErr.UnsafeValue;
            await act.Should().ThrowAsync<InvalidResultStateException>();
        }

        [Fact]
        public async Task UnsafeErrorThrowsOnSuccess()
        {
            AsyncResult<int, string> asyncOk = Result.Ok<int, string>(13);
            Func<Task> act = () => asyncOk.UnsafeError;
            await act.Should().ThrowAsync<InvalidResultStateException>();
        }

        [Fact]
        public async Task ConversionFromTaskCreatesAsyncResult()
        {
            var task = Task.FromResult(Result.Ok<int, string>(42));
            AsyncResult<int, string> asyncFromTask = task;
            var result = await asyncFromTask;
            result.IsOk().Should().BeTrue();
            result.UnsafeValue.Should().Be(42);
        }
    }
}
