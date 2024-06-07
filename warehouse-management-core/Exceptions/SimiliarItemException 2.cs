namespace warehouse_management_core.Exceptions;

public class SimilarItemTitleException(string newName) : Exception
{
    public override string Message => $"There is already Item title with {newName}";
}
