using UnityEngine;

public abstract class InventoryViewBase : MonoBehaviour
{
    protected IItemContainer container;

    public virtual void Bind(IItemContainer container)
    {
        this.container = container;
        Refresh();
    }

    public abstract void Refresh();
}