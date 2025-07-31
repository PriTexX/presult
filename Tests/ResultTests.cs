using FluentAssertions;
using PResult;

namespace Tests;

public class ResultTests
{
    [Fact]
    public void CreatesSuccessResult()
    {
        var result = Result.Ok<int, string>(1);
        result.IsOk().Should().BeTrue();
        result.UnsafeValue.Should().Be(1);
    }

    [Fact]
    public void CreatesErrorResult()
    {
        var result = Result.Err<int, string>("error");
        result.IsErr().Should().BeTrue();
        result.UnsafeError.Should().Be("error");
    }

    [Fact]
    public void OkResultReportsCorrectFlags()
    {
        var ok = Result.Ok<int, string>(5);
        ok.IsOk().Should().BeTrue();
        ok.IsErr().Should().BeFalse();
    }

    [Fact]
    public void ErrorResultReportsCorrectFlags()
    {
        var err = Result.Err<int, string>("fail");
        err.IsErr().Should().BeTrue();
        err.IsOk().Should().BeFalse();
    }

    [Fact]
    public void MatchTransformsOnSuccess()
    {
        var success = Result.Ok<int, string>(2);
        var outValue = success.Match(v => v * 2, _ => -1);
        outValue.Should().Be(4);
    }

    [Fact]
    public void MatchReturnsFallbackOnError()
    {
        var failure = Result.Err<int, string>("bad");
        var outValue = failure.Match(v => v * 2, _ => -1);
        outValue.Should().Be(-1);
    }

    [Fact]
    public void ThenChainsOnSuccess()
    {
        var success = Result.Ok<int, string>(3);
        var chained = success.Then(v => Result.Ok<string, string>((v * 2).ToString()));
        chained.IsOk().Should().BeTrue();
        chained.UnsafeValue.Should().Be("6");
    }

    [Fact]
    public void ThenSkipsOnError()
    {
        var failure = Result.Err<int, string>("oops");
        var chained = failure.Then(v => Result.Ok<string, string>((v * 2).ToString()));
        chained.IsErr().Should().BeTrue();
        chained.UnsafeError.Should().Be("oops");
    }

    [Fact]
    public void ThenErrHandlesError()
    {
        var failure = Result.Err<int, string>("err");
        var recovered = failure.ThenErr(_ => Result.Ok<int, string>(-1));
        recovered.IsOk().Should().BeTrue();
        recovered.UnsafeValue.Should().Be(-1);
    }

    [Fact]
    public void ThenErrSkipsOnSuccess()
    {
        var success = Result.Ok<int, string>(7);
        var unchanged = success.ThenErr(_ => Result.Ok<int, string>(-1));
        unchanged.IsOk().Should().BeTrue();
        unchanged.UnsafeValue.Should().Be(7);
    }

    [Fact]
    public void MapTransformsOnSuccess()
    {
        var success = Result.Ok<int, string>(4);
        var mapped = success.Map(v => v + 1);
        mapped.IsOk().Should().BeTrue();
        mapped.UnsafeValue.Should().Be(5);
    }

    [Fact]
    public void MapPreservesError()
    {
        var failure = Result.Err<int, string>("fail");
        var mapped = failure.Map(v => v + 1);
        mapped.IsErr().Should().BeTrue();
        mapped.UnsafeError.Should().Be("fail");
    }

    [Fact]
    public void MapErrTransformsError()
    {
        var failure = Result.Err<int, string>("error");
        var mappedErr = failure.MapErr(e => e.Length);
        mappedErr.IsErr().Should().BeTrue();
        mappedErr.UnsafeError.Should().Be(5);
    }

    [Fact]
    public void MapErrPreservesValue()
    {
        var success = Result.Ok<int, string>(10);
        var mappedErr = success.MapErr(e => e.Length);
        mappedErr.IsOk().Should().BeTrue();
        mappedErr.UnsafeValue.Should().Be(10);
    }

    [Fact]
    public void TryPickValueOnSuccess()
    {
        var success = Result.Ok<int, string>(9);
        var ok = success.TryPickValue(out var value, out var error);
        ok.Should().BeTrue();
        value.Should().Be(9);
        error.Should().BeNull();
    }

    [Fact]
    public void TryPickValueOnError()
    {
        var failure = Result.Err<int, string>("fail");
        var ok = failure.TryPickValue(out var value, out var error);
        ok.Should().BeFalse();
        value.Should().Be(default);
        error.Should().Be("fail");
    }

    [Fact]
    public void TryPickErrorOnError()
    {
        var failure = Result.Err<int, string>("bad");
        var err = failure.TryPickError(out var error, out var value);
        err.Should().BeTrue();
        error.Should().Be("bad");
        value.Should().Be(default);
    }

    [Fact]
    public void TryPickErrorOnSuccess()
    {
        var success = Result.Ok<int, string>(11);
        var err = success.TryPickError(out var error, out var value);
        err.Should().BeFalse();
        error.Should().BeNull();
        value.Should().Be(11);
    }

    [Fact]
    public void ValueOrReturnsValueOrFallback()
    {
        var success = Result.Ok<int, string>(12);
        success.ValueOr(0).Should().Be(12);

        var failure = Result.Err<int, string>("nope");
        failure.ValueOr(0).Should().Be(0);
    }

    [Fact]
    public void UnsafeValueThrowsOnError()
    {
        var failure = Result.Err<int, string>("err");
        Action act = () => _ = failure.UnsafeValue;
        act.Should().Throw<InvalidResultStateException>();
    }

    [Fact]
    public void UnsafeErrorThrowsOnSuccess()
    {
        var success = Result.Ok<int, string>(13);
        Action act = () => _ = success.UnsafeError;
        act.Should().Throw<InvalidResultStateException>();
    }

    [Fact]
    public void ImplicitConversionFromValue()
    {
        Result<int, string> fromValue = 14;
        fromValue.IsOk().Should().BeTrue();
        fromValue.UnsafeValue.Should().Be(14);
    }

    [Fact]
    public void ImplicitConversionFromError()
    {
        Result<int, string> fromError = "oops";
        fromError.IsErr().Should().BeTrue();
        fromError.UnsafeError.Should().Be("oops");
    }

    [Fact]
    public void EqualityOperatorsBehaveCorrectly()
    {
        var a1 = Result.Ok<int, string>(1);
        var a2 = Result.Ok<int, string>(1);
        a1.Should().Be(a2);
        a1.GetHashCode().Should().Be(a2.GetHashCode());

        var e1 = Result.Err<int, string>("e");
        var e2 = Result.Err<int, string>("e");
        e1.Should().Be(e2);
        e1.GetHashCode().Should().Be(e2.GetHashCode());

        a1.Should().NotBe(e1);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ThenAsyncChainsOnSuccessForBothOverloads(bool useTaskOverload)
    {
        var success = Result.Ok<int, string>(3);

        AsyncResult<string, string> asyncResult = useTaskOverload
            // overload taking Func<T, Task<Result<K, TError>>>
            ? success.ThenAsync(v => Task.FromResult(Result.Ok<string, string>((v * 2).ToString())))
            // overload taking Func<T, AsyncResult<K, TError>>
            : success.ThenAsync(v => Result.OkAsync<string, string>((v * 2).ToString()));

        var final = await asyncResult;
        final.IsOk().Should().BeTrue();
        final.UnsafeValue.Should().Be("6");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ThenAsyncSkipsOnErrorForBothOverloads(bool useTaskOverload)
    {
        var failure = Result.Err<int, string>("oops");

        AsyncResult<string, string> asyncResult = useTaskOverload
            ? failure.ThenAsync(v => Task.FromResult(Result.Ok<string, string>((v * 2).ToString())))
            : failure.ThenAsync(v => Result.OkAsync<string, string>((v * 2).ToString()));

        var final = await asyncResult;
        final.IsErr().Should().BeTrue();
        final.UnsafeError.Should().Be("oops");
    }

    [Fact]
    public async Task MapAsyncTransformsOnSuccess()
    {
        var success = Result.Ok<int, string>(4);
        var mappedTask = success.MapAsync(v => Task.FromResult(v + 1));
        var mapped = await mappedTask;
        mapped.IsOk().Should().BeTrue();
        mapped.UnsafeValue.Should().Be(5);
    }

    [Fact]
    public async Task MapAsyncSkipsOnError()
    {
        var failure = Result.Err<int, string>("fail");
        var mappedTask = failure.MapAsync(v => Task.FromResult(v + 1));
        var mapped = await mappedTask;
        mapped.IsErr().Should().BeTrue();
        mapped.UnsafeError.Should().Be("fail");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ThenErrAsyncHandlesError(bool useTaskOverload)
    {
        var failure = Result.Err<int, string>("error");

        AsyncResult<int, string> asyncResult = useTaskOverload
            // overload taking Func<TError, Task<Result<T, TError>>>
            ? failure.ThenErrAsync(err => Task.FromResult(Result.Ok<int, string>(-1)))
            // overload taking Func<TError, AsyncResult<T, TError>>
            : failure.ThenErrAsync(err => Result.OkAsync<int, string>(-1));

        var final = await asyncResult;
        final.IsOk().Should().BeTrue();
        final.UnsafeValue.Should().Be(-1);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ThenErrAsyncSkipsOnSuccess(bool useTaskOverload)
    {
        var success = Result.Ok<int, string>(5);

        AsyncResult<int, string> asyncResult = useTaskOverload
            ? success.ThenErrAsync(_ => Task.FromResult(Result.Err<int, string>("ignored")))
            : success.ThenErrAsync(_ => Result.ErrAsync<int, string>("ignored"));

        var final = await asyncResult;
        final.IsOk().Should().BeTrue();
        final.UnsafeValue.Should().Be(5);
    }

    [Fact]
    public async Task MapErrAsyncTransformsError()
    {
        var failure = Result.Err<int, string>("failure");
        var mapped = await failure.MapErrAsync(err => Task.FromResult(err.Length));
        mapped.IsErr().Should().BeTrue();
        mapped.UnsafeError.Should().Be(7);
    }

    [Fact]
    public async Task MapErrAsyncSkipsOnSuccess()
    {
        var success = Result.Ok<int, string>(10);
        var mapped = await success.MapErrAsync(err => Task.FromResult(err.Length));
        mapped.IsOk().Should().BeTrue();
        mapped.UnsafeValue.Should().Be(10);
    }
}
