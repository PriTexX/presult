using FluentAssertions;
using PResult;

namespace Tests;

public class ResultExtTests
{
    [Fact]
    public void ReturnsOkResult()
    {
        var res = ResultExt.Ok(Unit.Default);

        res.IsOk.Should().Be(true);
    }

    [Fact]
    public void ReturnsErrResult()
    {
        var res = ResultExt.Err<Unit>(new Exception("Some error"));

        res.IsErr.Should().Be(true);
        res.Should().BeOfType<Result<Unit>>();
    }

    [Fact]
    public async Task ReturnsAsyncOkResult()
    {
        var asyncRes = ResultExt.OkAsync(Unit.Default);

        asyncRes.Should().BeOfType<AsyncResult<Unit>>();

        var res = await asyncRes;

        res.Should().BeOfType<Result<Unit>>();
        res.IsOk.Should().Be(true);
    }

    [Fact]
    public async Task ReturnsErrAsyncResult()
    {
        var asyncRes = ResultExt.ErrAsync<Unit>(new Exception("Some error"));

        asyncRes.Should().BeOfType<AsyncResult<Unit>>();

        var res = await asyncRes;

        res.IsErr.Should().Be(true);
        res.Should().BeOfType<Result<Unit>>();
    }

    [Fact]
    public void WrapsActionThatThrows()
    {
        var res = ResultExt.FromThrowable(() => throw new Exception("Some error"));

        res.IsErr.Should().Be(true);
        res.Should().BeOfType<Result<Unit>>();
    }

    [Fact]
    public void WrapsFuncThatThrows()
    {
        var res = ResultExt.FromThrowable(() =>
        {
            throw new Exception("Some error");
            return 1;
        });

        res.IsErr.Should().Be(true);
        res.Should().BeOfType<Result<int>>();
    }

    [Fact]
    public void WrapsActionThatNotThrows()
    {
        var res = ResultExt.FromThrowable(() => { });

        res.IsOk.Should().Be(true);
        res.Should().BeOfType<Result<Unit>>();
    }

    [Fact]
    public void WrapsFuncThatNotThrows()
    {
        var res = ResultExt.FromThrowable(() => 1);

        res.IsOk.Should().Be(true);
        res.Should().BeOfType<Result<int>>();
    }

    [Fact]
    public async Task WrapsTaskThatTrows()
    {
        var taskThatThrows = async () =>
        {
            await Task.Delay(1);
            throw new Exception("Some error");
        };

        var asyncRes = ResultExt.FromTask(taskThatThrows());

        asyncRes.Should().BeOfType<AsyncResult<Unit>>();

        var res = await asyncRes;

        res.Should().BeOfType<Result<Unit>>();
        res.IsErr.Should().Be(true);
    }

    [Fact]
    public async Task WrapsTaskWithReturnValueThatThrows()
    {
        var taskThatThrows = async () =>
        {
            await Task.Delay(1);
            throw new Exception("Some error");
            return 1;
        };

        var asyncRes = ResultExt.FromTask(taskThatThrows());

        asyncRes.Should().BeOfType<AsyncResult<int>>();

        var res = await asyncRes;

        res.Should().BeOfType<Result<int>>();
        res.IsErr.Should().Be(true);
    }

    [Fact]
    public async Task WrapsTaskThatNotTrows()
    {
        var taskThatThrows = async () =>
        {
            await Task.Delay(1);
        };

        var asyncRes = ResultExt.FromTask(taskThatThrows());

        asyncRes.Should().BeOfType<AsyncResult<Unit>>();

        var res = await asyncRes;

        res.Should().BeOfType<Result<Unit>>();
        res.IsOk.Should().Be(true);
    }

    [Fact]
    public async Task WrapsTaskWithReturnValueThatNotThrows()
    {
        var taskThatThrows = async () =>
        {
            await Task.Delay(1);
            return 1;
        };

        var asyncRes = ResultExt.FromTask(taskThatThrows());

        asyncRes.Should().BeOfType<AsyncResult<int>>();

        var res = await asyncRes;

        res.Should().BeOfType<Result<int>>();
        res.IsOk.Should().Be(true);
    }
}
