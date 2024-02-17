using FluentAssertions;
using PResult;

namespace Tests;

public class ResultTests
{
    [Fact]
    public void CreatesOkResult()
    {
        var res = new Result<Unit>(Unit.Default);

        var resValue = res.UnsafeValue;

        resValue.Should().Be(Unit.Default);
    }

    [Fact]
    public void CreatesErrorResult()
    {
        var error = new Exception("Some error");
        var res = new Result<Unit>(error);

        var resValue = res.UnsafeError;

        resValue.Message.Should().Be("Some error");
    }

    [Fact]
    public void UnsafeValueThrowsExceptionWhenAccessedInErrState()
    {
        var res = new Result<Unit>(new Exception());

        var action = () => res.UnsafeValue;

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void UnsafeErrorThrowsExceptionWhenAccessedInOkState()
    {
        var res = new Result<Unit>(Unit.Default);

        var action = () => res.UnsafeError;

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void StateCheckWorksCorrect()
    {
        var okRes = new Result<Unit>(Unit.Default);
        var errRes = new Result<Unit>(new Exception());

        okRes.IsOk.Should().Be(true);
        okRes.IsErr.Should().Be(false);

        errRes.IsOk.Should().Be(false);
        errRes.IsErr.Should().Be(true);
    }

    [Fact]
    public void MatchWorksCorrect()
    {
        var okRes = new Result<Unit>(Unit.Default);
        var errRes = new Result<Unit>(new Exception());

        okRes.Match(_ => 1, _ => 0).Should().Be(1);
        errRes.Match(_ => 1, _ => 0).Should().Be(0);
    }

    [Fact]
    public void MatchAsyncWorksCorrect()
    {
        var okRes = new Result<Unit>(Unit.Default);

        var asyncVal = okRes.MatchAsync(async _ => 1, async _ => 0);

        asyncVal.Should().BeOfType<Task<int>>();
    }

    [Fact]
    public void ThenWorksCorrectInOkState()
    {
        var res = new Result<Unit>(Unit.Default);

        var newRes = res.Then<bool>(_ => true);

        newRes.UnsafeValue.Should().Be(true);
    }

    [Fact]
    public void ThenWorksCorrectInErrState()
    {
        var res = new Result<Unit>(new Exception("Some error"));

        var newRes = res.Then<bool>(_ => true);

        newRes.UnsafeError.Message.Should().Be("Some error");
    }

    [Fact]
    public void ThenReturnsNewError()
    {
        var res = new Result<Unit>(Unit.Default);

        var newRes = res.Then<bool>(_ => new Exception("Some error"));

        newRes.UnsafeError.Message.Should().Be("Some error");
    }

    [Fact]
    public void MapWorksCorrectInOkState()
    {
        var res = new Result<Unit>(Unit.Default);

        var newRes = res.Map(_ => true);

        newRes.UnsafeValue.Should().Be(true);
    }

    [Fact]
    public void MapWorksCorrectInErrState()
    {
        var res = new Result<Unit>(new Exception("Some error"));

        var newRes = res.Map(_ => true);

        newRes.UnsafeError.Message.Should().Be("Some error");
    }

    [Fact]
    public void MapErrWorksCorrectInErrState()
    {
        var res = new Result<Unit>(new Exception("Some error"));

        var newRes = res.MapErr(_ => new Exception("Some new error"));

        newRes.UnsafeError.Message.Should().Be("Some new error");
    }

    [Fact]
    public void MapErrWorksCorrectInOkState()
    {
        var res = new Result<Unit>(Unit.Default);

        var newRes = res.MapErr(_ => new Exception("Some new error"));

        newRes.UnsafeValue.Should().Be(Unit.Default);
    }

    [Fact]
    public void ThenErrWorksCorrectInErrState()
    {
        var res = new Result<Unit>(new Exception("Some error"));

        var newRes = res.ThenErr(_ => Unit.Default);

        newRes.UnsafeValue.Should().Be(Unit.Default);
    }

    [Fact]
    public void ThenErrReturnsNewError()
    {
        var res = new Result<Unit>(new Exception("Some error"));

        var newRes = res.ThenErr(_ => new Exception("Some new error"));

        newRes.UnsafeError.Message.Should().Be("Some new error");
    }

    [Fact]
    public async Task ThenAsyncWorksCorrectInOkState()
    {
        var res = new Result<Unit>(Unit.Default);

        var newRes = await res.ThenAsync<bool>(async _ =>
        {
            await Task.Delay(1);
            return true;
        });

        newRes.UnsafeValue.Should().Be(true);
    }

    [Fact]
    public async Task ThenAsyncWorksCorrectInErrState()
    {
        var res = new Result<Unit>(new Exception("Some error"));

        var wasCalled = false;
        var newRes = await res.ThenAsync<bool>(async _ =>
        {
            wasCalled = true;
            await Task.Delay(1);
            return true;
        });

        wasCalled.Should().Be(false);
        newRes.UnsafeError.Message.Should().Be("Some error");
    }

    [Fact]
    public async Task ThenAsyncWorksReturnsNewError()
    {
        var res = new Result<Unit>(Unit.Default);

        var newRes = await res.ThenAsync<bool>(async _ =>
        {
            await Task.Delay(1);
            return new Exception("Some new error");
        });

        newRes.UnsafeError.Message.Should().Be("Some new error");
    }

    [Fact]
    public async Task MapAsyncWorksCorrectInOkState()
    {
        var res = new Result<Unit>(Unit.Default);

        var newRes = await res.MapAsync(async _ =>
        {
            await Task.Delay(1);
            return true;
        });

        newRes.UnsafeValue.Should().Be(true);
    }

    [Fact]
    public async Task MapAsyncWorksCorrectInErrState()
    {
        var res = new Result<Unit>(new Exception("Some error"));

        var wasCalled = false;
        var newRes = await res.MapAsync(async _ =>
        {
            wasCalled = true;
            await Task.Delay(1);
            return true;
        });

        wasCalled.Should().Be(false);
        newRes.UnsafeError.Message.Should().Be("Some error");
    }

    [Fact]
    public async Task MapErrAsyncWorksCorrectInOkState()
    {
        var res = new Result<Unit>(Unit.Default);

        var newRes = await res.MapErrAsync(async _ =>
        {
            await Task.Delay(1);
            return new Exception("Some new error");
        });

        newRes.UnsafeValue.Should().Be(Unit.Default);
    }

    [Fact]
    public async Task MapErrAsyncWorksCorrectInErrState()
    {
        var res = new Result<Unit>(new Exception("Some error"));

        var newRes = await res.MapErrAsync(async _ =>
        {
            await Task.Delay(1);
            return new Exception("Some new error");
        });

        newRes.UnsafeError.Message.Should().Be("Some new error");
    }

    [Fact]
    public void ChainMethodsWorksCorrect()
    {
        var res = new Result<Unit>(Unit.Default)
            .Map(_ => 1)
            .Then<double>(prev =>
            {
                prev.Should().Be(1);
                return prev * 2.0;
            })
            .Then<int>(prev =>
            {
                prev.Should().Be(2.0);
                return new Exception("Some error");
            })
            .MapErr(prevErr =>
            {
                prevErr.Message.Should().Be("Some error");
                return new Exception("Some new error");
            })
            .Match(
                val => val,
                err =>
                {
                    err.Message.Should().Be("Some new error");
                    return 0;
                }
            );

        res.Should().Be(0);
    }
}
