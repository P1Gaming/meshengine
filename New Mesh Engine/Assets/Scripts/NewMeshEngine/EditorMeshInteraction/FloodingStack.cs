using System.Collections;
using System.Collections.Generic;

public class FloodingStack<T> : IEnumerable<T>
{
    int maxFill;
    List<T> itemList;

    public int Count => itemList.Count;

    /// <summary>
    /// Create a stack with a maximum quantity. If the stack is full, the oldest item will be removed.
    /// </summary>
    /// <param name="maxFill"></param>
    public FloodingStack(int maxFill)
    {
        this.maxFill = maxFill;
        itemList = new List<T>();
    }

    /// <summary>
    /// Add a new item to the stack.
    /// </summary>
    /// <param name="newItem"></param>
    public void Push(T newItem)
    {
        itemList.Add(newItem);
        if (itemList.Count > maxFill)
        {
            itemList.RemoveAt(0);
        }
    }

    /// <summary>
    /// Get the last item and remove it from the stack
    /// </summary>
    /// <returns></returns>
    public T Pop()
    {
        int count = itemList.Count;
        T item = itemList[count - 1];
        itemList.RemoveAt(count - 1);
        return item;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return itemList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}