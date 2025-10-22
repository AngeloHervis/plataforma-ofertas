using plataforma.ofertas.Dto.Constantes;

namespace  plataforma.ofertas._Base;

public class CommandResult<TValue> : ICommandResult<TValue>
{
    public bool IsError { get; private init; }
    public int StatusCode { get; private init; }
    public List<CommandError> Errors { get; private init; }
    public TValue Data { get; private init; }
    
    public static CommandResult<TValue> Success(TValue data)
        => new ()
        {
            IsError = false,
            StatusCode = StatusCodes.Status200OK,
            Data = data
        };
    
    public static CommandResult<TValue> ValidationFailure(List<CommandError> errors)
        => new()
        {
            IsError = true,
            StatusCode = StatusCodes.Status422UnprocessableEntity,
            Errors = errors
        };
    
    public static CommandResult<TValue> NotFound(string message)
        => new ()
        {
            IsError = true,
            StatusCode = StatusCodes.Status404NotFound,
            Errors = [new CommandError(CodigosErro.NaoEncontrado, message)]
        };

    public static CommandResult<TValue> Forbidden(string message)
        => new ()
        {
            IsError = true,
            StatusCode = StatusCodes.Status403Forbidden,
            Errors = [new CommandError(CodigosErro.NaoAutorizado, message)]
        };
    
    public static CommandResult<TValue> InvalidRequest(string message)
        => new ()
        {
            IsError = true,
            StatusCode = StatusCodes.Status422UnprocessableEntity,
            Errors = [new CommandError(CodigosErro.ErroDeValidacao, message)]
        };
    
    public static CommandResult<TValue> DuplicateError(string message)
        => new()
        {
            IsError = true,
            StatusCode = StatusCodes.Status409Conflict,
            Errors = [new CommandError(CodigosErro.EntradaDuplicada, message)]
        };
    
    public static CommandResult<TValue> InternalError(string message)
        => new ()
        {
            IsError = true,
            StatusCode = StatusCodes.Status500InternalServerError,
            Errors = [new CommandError(CodigosErro.ErroInterno, message)]
        };
    
    public static CommandResult<TValue> ExternalError(string message)
        => new ()
        {
            IsError = true,
            StatusCode = StatusCodes.Status503ServiceUnavailable,
            Errors = [new CommandError(CodigosErro.FalhaDeConexaoComApiExterna, message)]
        };
}