using System.Diagnostics;

namespace ExtractOfficialAssets;

public class MObjectPool<T> 
{
    private readonly Stack<T> _stack = new Stack<T>();
    private readonly Action<T> _actionOnGet;
    private readonly Action<T> _actionOnRelease;
    private readonly Action<T> _actionOnDestroy;
    private MObjectPool<T>.CreateObj _objCreater = (MObjectPool<T>.CreateObj)null;
    private HashSet<T> _set = new HashSet<T>();

    public int countAll { get; private set; }

    public int countActive => this.countAll - this.countInactive;

    public int countInactive => this._stack.Count;

    public MObjectPool(
      MObjectPool<T>.CreateObj creater,
      Action<T> actionOnGet,
      Action<T> actionOnRelease,
      Action<T> actionOnDestroy = null)
    {
        this._objCreater = creater;
        this._actionOnGet = actionOnGet;
        this._actionOnRelease = actionOnRelease;
        this._actionOnDestroy = actionOnDestroy;
    }

    public T Get()
    {
        T obj;
        if (this._stack.Count == 0)
        {
            obj = this._objCreater();
            ++this.countAll;
        }
        else
        {
            obj = this._stack.Pop();
            this._set.Remove(obj);
        }
        Action<T> actionOnGet = this._actionOnGet;
        if (actionOnGet != null)
            actionOnGet(obj);
        return obj;
    }

    public void Release(T element)
    {
        if (this._set.Contains(element))
        {
           throw new Exception(element.GetType().ToString() + " release twice!");
        }
        else
        {
            this._set.Add(element);
            Action<T> actionOnRelease = this._actionOnRelease;
            if (actionOnRelease != null)
                actionOnRelease(element);
            this._stack.Push(element);
        }
    }

    public void Clear()
    {
        if (this._actionOnDestroy != null)
        {
            while (this._stack.Count > 0)
                this._actionOnDestroy(this._stack.Pop());
        }
        this._stack.Clear();
    }

    public delegate T CreateObj();
}