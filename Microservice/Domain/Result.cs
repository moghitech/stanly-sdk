namespace Fusion.Microservice.Domain;

public class Result(bool isSuccess, Exception exception, params string[] messages)
{
    public bool IsSuccess { get; } = isSuccess;
    public bool IsFailed => !IsSuccess;
    public IEnumerable<string> Messages { get; } = messages;
    public Exception Exception { get; set; } = exception;

    public static Result Success()
    {
        return new Result(true, default!);
    }

    public static Result Success(params string[] messages)
    {
        return new Result(true, null!, messages);
    }

    public static Result Failed(params string[] messages)
    {
        return new Result(false, null!, messages);
    }

    public static Result Failed(Exception exception)
    {
        return new Result(false, exception);
    }

    public static Result Failed(Exception exception, params string[] messages)
    {
        return new Result(false, null!, messages);
    }
    
    // wrap Exceptions into Result
    public static implicit operator Result(Exception exception)
    {
       return new(false, default!)
       {
           Exception = exception
       };
    }
}

public class Result<TSuccess> : Result
{
    public TSuccess Data { get; }

    private Result(bool isSuccess, TSuccess value, Exception exception, params string[] messages)
        : base(isSuccess, exception, messages)
    {
        Data = value;
    }

    public static Result<TSuccess> Success(TSuccess value)
    {
        return new Result<TSuccess>(true, value, null!);
    }

    public static Result<TSuccess> Success(TSuccess value, params string[] messages)
    {
        return new Result<TSuccess>(true, value, null!, messages);
    }

    public new static Result<TSuccess> Failed(params string[] messages)
    {
        return new Result<TSuccess>(false, default!, null!, messages);
    }

    public new static Result<TSuccess> Failed(Exception exception)
    {
        return new Result<TSuccess>(false, default!, exception);
    }

    public new static Result<TSuccess> Failed(Exception exception, params string[] messages)
    {
        return new Result<TSuccess>(false, default!, null!, messages);
    }
    
    // Implicit Operators

    // convert success TSuccess to Result<TSuccess> 
    public static implicit operator Result<TSuccess>(TSuccess value) => new(true, value, null!);

    // wrap Exceptions into Result
    public static implicit operator Result<TSuccess>(Exception exception) =>
        new(false, default!, exception);
}