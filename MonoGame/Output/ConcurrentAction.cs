using System;
using System.Threading;

namespace MonoGame.Output;

public class ConcurrentAction<T>
{
    private readonly Thread _requestThread;
    private T _data;

    public ConcurrentAction(Func<T> request)
    {
        _requestThread = new Thread(() => _data = request());
        _requestThread.Start();
    }
    
    public T Result => GetResult();

    private T GetResult()
    {
        _requestThread.Join();
        return _data;
    }

    public ConcurrentAction<T2> Then<T2>(Func<T, T2> then)
    {
        return new ConcurrentAction<T2>(() => then(GetResult()));
    }
    
    public ConcurrentAction Then(Action<T> then)
    {
        return new ConcurrentAction(() =>
        {
            then(GetResult());
        });
    }
}

public class ConcurrentAction
{
    private readonly Thread _requestThread;

    public ConcurrentAction(ThreadStart request)
    {
        _requestThread = new Thread(request);
        _requestThread.Start();
    }

    public ConcurrentAction<T> Then<T>(Func<T> then)
    {
        return new ConcurrentAction<T>(() =>
        {
            _requestThread.Join();
            return then();
        });
    }
    
    public ConcurrentAction Then(ThreadStart then)
    {
        return new ConcurrentAction(() =>
        {
            _requestThread.Join();
            then();
        });
    }
}