namespace ArchStudy.Clean.UseCases.Common;

public abstract record UseCaseResult<T>
{
    public sealed record Ok(T Value) : UseCaseResult<T>;
    public sealed record NotFound(string Message) : UseCaseResult<T>;
    public sealed record Invalid(string Message) : UseCaseResult<T>;
}
