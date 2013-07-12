namespace Lemon.Common
{
    public interface IEditableCache<in TValue>
    {
        void AddTemporaryValue(TValue value);        
    }
}