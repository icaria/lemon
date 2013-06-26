namespace Winterspring.Lemon.Base
{
    //This is a snapshot that reflects the state of the object in the database, as far as we know.
    //It can be used to figure out the difference between what's in memory and what's in the database, e.g. to see how many extra items
    //will be pulled out of inventory.
    public interface ISnapshot<in T>
    {
        void Initialize(T bo);
    }
}