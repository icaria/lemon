namespace Lemon.Base
{
    ///<exception>This class will not throw database exceptions</exception>
    public interface IDisplayObject : IObjectWithId
    {
        string Name { get; }
    }
}
