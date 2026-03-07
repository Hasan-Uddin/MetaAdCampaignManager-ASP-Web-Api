namespace SharedKernel;

public class Result
{
    // Only subclasses (and this class itself) can call this.
    public Result(bool isSuccess, Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        // If success, error must be None; if failure, error must NOT be None.
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error for the given success state.", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        value is null
            ? Failure<TValue>(Error.NullValue)
            : new Result<TValue>(value, true, Error.None);

    public static Result Failure(Error error) =>
        new(false, error);

    public static Result Failure(string errorMessage) =>
        Failure(Error.Failure("General.Error", errorMessage));

    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);

    public static Result<TValue> Failure<TValue>(string errorMessage) =>
        Failure<TValue>(Error.Failure("General.Error", errorMessage));
}

public class Result<TValue> : Result
{
    private TValue? _value { get; init; }

    // Internal: only code in this assembly can construct directly.
    internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        if (isSuccess && value is null)
        {
            throw new ArgumentNullException(
                nameof(value),
                "The value of a successful result cannot be null.");
        }

        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null
            ? Success(value)
            : Failure<TValue>(Error.NullValue);
}
