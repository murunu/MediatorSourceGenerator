namespace Mediator;

public class MediatorResult<TValue> : MediatorResult
{
    public TValue? Value { get; }
    
    private MediatorResult(TValue? value) : base(true)
    {
        Value = value;
    }
    
    private MediatorResult(Exception exception) : base(exception)
    {
    }
    
    public static implicit operator MediatorResult<TValue>(TValue value) => new(value);

    public static implicit operator MediatorResult<TValue> (Exception exception) => new(exception);
    
    public TResult Match<TResult>(Func<TValue, TResult> success, Func<List<Exception>, TResult> failure) =>
        IsSuccess ? success(Value) : failure(Exceptions);

    public async Task<TResult> Match<TResult>(Func<TValue, Task<TResult>> success, Func<List<Exception>, TResult> failure) =>
        IsSuccess ? await success(Value) : failure(Exceptions);
}

public class MediatorResult
{
    public List<Exception> Exceptions { get; } = [];
    public int Count = 1;
    
    public bool IsSuccess { get; protected init; }
    public bool IsFailure => !IsSuccess;

    protected MediatorResult(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }
    
    protected MediatorResult(bool isSuccess, int count)
    {
        IsSuccess = isSuccess;
        Count = count;
    }

    protected MediatorResult(Exception exception) : this(false)
    {
        Exceptions.Add(exception);
    }

    public static implicit operator MediatorResult (Exception exception) => new(exception);
    
    public static MediatorResult Success() => new(true);
    public static MediatorResult Success(int count) => new(true, count);
    public static MediatorResult Failure(Exception exception) => new(exception);
    public static MediatorResult Failure() => new(false);
    
    public static MediatorResult operator + (MediatorResult mediatorResult, MediatorResult other)
    {
        if (other.IsFailure)
        {
            mediatorResult.Exceptions.AddRange(other.Exceptions);
        }

        mediatorResult.Count += other.Count;
        return mediatorResult;
    }
    
    public TResult Match<TResult>(Func<TResult> success, Func<List<Exception>, TResult> failure) =>
        IsSuccess ? success() : failure(Exceptions);
    
    public void ThrowIfFailure()
    {
        if (IsFailure)
        {
            throw new AggregateException(Exceptions);
        }
    }
}