namespace Fayora.Domain.Common.Results;

public readonly record struct Success;

public class Result
{
    private readonly List<Error> _errors = [];

    public Error[] Errors => _errors.ToArray();

    public bool IsSuccess => _errors.Count == 0;
    public bool IsError => _errors.Count > 0;

    public static readonly Success Success = new();

    protected Result() { }

    protected Result(Error error)
    {
        _errors.Add(error);
    }

    protected Result(IEnumerable<Error> errors)
    {
        _errors.AddRange(errors);
    }
}

public class Result<TValue> : Result
{
    private readonly TValue _value = default!;

    public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access value when result is an error.");

    private Result(TValue value)
    {
        _value = value;
    }

    private Result(Error error) : base(error) { }

    private Result(IEnumerable<Error> errors) : base(errors) { }

    public static Result<TValue> CreateSuccess(TValue value) => new(value);
    public static Result<TValue> CreateFailure(Error error) => new(error);
    public static Result<TValue> CreateFailure(IEnumerable<Error> errors) => new(errors);

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Error error) => new(error);
    public static implicit operator Result<TValue>(Error[] errors) => new(errors);

    public TNextValue Match<TNextValue>(Func<TValue, TNextValue> onValue, Func<IEnumerable<Error>, TNextValue> onError)
    {
        if (IsError)
        {
            return onError(Errors);
        }

        return onValue(Value);
    }
}