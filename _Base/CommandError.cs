namespace  plataforma.ofertas._Base;

public class CommandError(string code, string message)
{
    public string Code { get; set; } = code;
    public string Message { get; set; } = message;
}