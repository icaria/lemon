using System.Collections;

namespace Winterspring.Lemon.Base
{
    public interface IWinterspringBusinessListBase : IControlPropertyChangedActionMode
    {
        void InsertItem(int index);
        void SwapItem(int index1, int index2);        
    }
}