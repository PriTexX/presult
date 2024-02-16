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

        var okVal = await okAsync.MatchAsync(async _ =>
        {
            await Task.Delay(1);
            return 1;
        }, async _ =>
        {
            await Task.Delay(1);
            return 0;
        });
        
        var errVal = await errAsync.MatchAsync(async _ =>
        {
            await Task.Delay(1);
            return 1;
        }, async _ =>
        {
            await Task.Delay(1);
            return 0;
        });

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
}
