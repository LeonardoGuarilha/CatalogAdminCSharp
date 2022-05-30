namespace FC.Codeflix.Catalog.Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string? message) : base(message)
    { }
    
    // O @ é para poder usar palavras reservadas como nome de variáveis
    public static void ThrowIfNull(object? @object, string exceptionMessage)
    {
        if(@object == null) 
            throw new NotFoundException(exceptionMessage);
    }
}