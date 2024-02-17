using FluentAssertions;
using PResult;

namespace Tests;

public class AsyncResultTests
{
    private static AsyncResult<Unit> OkAsyncResult() => ResultExt.OkAsync(Unit.Default);

    private static AsyncResult<Unit> ErrAsyncResult() =>
        ResultExt.ErrAsync<Unit>(new Exception("Some error"));

    [Fact]
    public async Task CanBeAwaited()
    {
        var asyncRes = OkAsyncResult();

        var res = await asyncRes;

        res.Should().BeOfType<Result<Unit>>();
    }

    [Fact]
    public async Task MatchWorksCorrect()
    {
        var asyncOk = OkAsyncResult();
        var asyncErr = ErrAsyncResult();

        var okVal = await asyncOk.Match(_ => 1, _ => 0);
        var errVal = await asyncErr.Match(_ => 1, _ => 0);

        okVal.Should().Be(1);
        errVal.Should().Be(0);
    }

    [Fact]
    public async Task MatchAsyncWorksCorrect()
    {
        var okAsync = OkAsyncResult();
        var errAsync = ErrAsyncResult();

        var okVal = await okAsync.MatchAsync(
            async _ =>
            {
                await Task.Delay(1);
                return 1;
            },
            async _ =>
            {
                await Task.Delay(1);
                return 0;
            }
        );

        var errVal = await errAsync.MatchAsync(
            async _ =>
            {
                await Task.Delay(1);
                return 1;
            },
            async _ =>
            {
                await Task.Delay(1);
                return 0;
            }
        );

        okVal.Should().Be(1);
        errVal.Should().Be(0);
    }

    [Fact]
    public async Task ThenWorksCorrect()
    {
        var okAsync = OkAsyncResult();
        var errAsync = ErrAsyncResult();

        var newOkAsync = okAsync.Then<int>(_ => 1);
        var newErrAsync = errAsync.Then<int>(_ => 1);

        newOkAsync.Should().BeOfType<AsyncResult<int>>();
        newErrAsync.Should().BeOfType<AsyncResult<int>>();

        var okRes = await newOkAsync;
        okRes.UnsafeValue.Should().Be(1);

        var errRes = await newErrAsync;
        errRes.UnsafeError.Message.Should().Be("Some error");
    }

    [Fact]
    public async Task ThenAsyncWorksCorrect()
    {
        var okAsync = OkAsyncResult();
        var errAsync = ErrAsyncResult();

        var newOkAsync = okAsync.ThenAsync<int>(async _ =>
        {
            await Task.Delay(1);
            return 1;
        });
        var newErrAsync = errAsync.ThenAsync<int>(async _ =>
        {
            await Task.Delay(1);
            return 1;
        });

        newOkAsync.Should().BeOfType<AsyncResult<int>>();
        newErrAsync.Should().BeOfType<AsyncResult<int>>();

        var okRes = await newOkAsync;
        okRes.UnsafeValue.Should().Be(1);

        var errRes = await newErrAsync;
        errRes.UnsafeError.Message.Should().Be("Some error");
    }

    [Fact]
    public async Task MapWorksCorrect()
    {
        var okAsync = OkAsyncResult();
        var errAsync = ErrAsyncResult();

        var newOkAsync = okAsync.Map(_ => 1);
        var newErrAsync = errAsync.Map(_ => 1);

        newOkAsync.Should().BeOfType<AsyncResult<int>>();
        newErrAsync.Should().BeOfType<AsyncResult<int>>();

        var okRes = await newOkAsync;
        okRes.UnsafeValue.Should().Be(1);

        var errRes = await newErrAsync;
        errRes.UnsafeError.Message.Should().Be("Some error");
    }

    [Fact]
    public async Task MapAsyncWorksCorrect()
    {
        var okAsync = OkAsyncResult();
        var errAsync = ErrAsyncResult();

        var newOkAsync = okAsync.MapAsync(async _ =>
        {
            await Task.Delay(1);
            return 1;
        });
        var newErrAsync = errAsync.MapAsync(async _ =>
        {
            await Task.Delay(1);
            return 1;
        });

        newOkAsync.Should().BeOfType<AsyncResult<int>>();
        newErrAsync.Should().BeOfType<AsyncResult<int>>();

        var okRes = await newOkAsync;
        okRes.UnsafeValue.Should().Be(1);

        var errRes = await newErrAsync;
        errRes.UnsafeError.Message.Should().Be("Some error");
    }

    [Fact]
    public async Task ThenErrWorksCorrect()
    {
        var okAsync = OkAsyncResult();
        var errAsync = ErrAsyncResult();

        var newOkAsync = okAsync.ThenErr(_ => new Exception("Some new error"));
        var newErrAsync = errAsync.ThenErr(_ => Unit.Default);

        newOkAsync.Should().BeOfType<AsyncResult<Unit>>();
        newErrAsync.Should().BeOfType<AsyncResult<Unit>>();

        var okRes = await newOkAsync;
        okRes.UnsafeValue.Should().Be(Unit.Default);

        var errRes = await newErrAsync;
        errRes.UnsafeValue.Should().Be(Unit.Default);
    }

    [Fact]
    public async Task ThenErrAsyncWorksCorrect()
    {
        var okAsync = OkAsyncResult();
        var errAsync = ErrAsyncResult();

        var newOkAsync = okAsync.ThenErrAsync(async _ =>
        {
            await Task.Delay(1);
            return new Exception("Some new error");
        });
        var newErrAsync = errAsync.ThenErrAsync(async _ =>
        {
            await Task.Delay(1);
            return Unit.Default;
        });

        newOkAsync.Should().BeOfType<AsyncResult<Unit>>();
        newErrAsync.Should().BeOfType<AsyncResult<Unit>>();

        var okRes = await newOkAsync;
        okRes.UnsafeValue.Should().Be(Unit.Default);

        var errRes = await newErrAsync;
        errRes.UnsafeValue.Should().Be(Unit.Default);
    }

    [Fact]
    public async Task MapErrWorksCorrect()
    {
        var okAsync = OkAsyncResult();
        var errAsync = ErrAsyncResult();

        var newOkAsync = okAsync.MapErr(_ => new Exception("Some new error"));
        var newErrAsync = errAsync.MapErr(_ => new Exception("Some new error"));

        newOkAsync.Should().BeOfType<AsyncResult<Unit>>();
        newErrAsync.Should().BeOfType<AsyncResult<Unit>>();

        var okRes = await newOkAsync;
        okRes.UnsafeValue.Should().Be(Unit.Default);

        var errRes = await newErrAsync;
        errRes.UnsafeError.Message.Should().Be("Some new error");
    }

    [Fact]
    public async Task MapErrAsyncWorksCorrect()
    {
        var okAsync = OkAsyncResult();
        var errAsync = ErrAsyncResult();

        var newOkAsync = okAsync.MapErrAsync(async _ =>
        {
            await Task.Delay(1);
            return new Exception("Some new error");
        });
        var newErrAsync = errAsync.MapErrAsync(async _ =>
        {
            await Task.Delay(1);
            return new Exception("Some new error");
        });

        newOkAsync.Should().BeOfType<AsyncResult<Unit>>();
        newErrAsync.Should().BeOfType<AsyncResult<Unit>>();

        var okRes = await newOkAsync;
        okRes.UnsafeValue.Should().Be(Unit.Default);

        var errRes = await newErrAsync;
        errRes.UnsafeError.Message.Should().Be("Some new error");
    }

    [Fact]
    public async Task ToAsyncResultCastWorksCorrect()
    {
        var asyncFuncThatReturnsTaskResult = async () =>
        {
            await Task.Delay(1);
            return ResultExt.Ok(Unit.Default);
        };

        var asyncRes = asyncFuncThatReturnsTaskResult()
            .ToAsyncResult()
            .Then<int>(_ => 1)
            .MapAsync(async v =>
            {
                await Task.Delay(1);
                return v * 2.0;
            });

        asyncRes.Should().BeOfType<AsyncResult<double>>();

        var res = await asyncRes;
        res.Should().BeOfType<Result<double>>();

        res.UnsafeValue.Should().Be(2.0);
    }
}
