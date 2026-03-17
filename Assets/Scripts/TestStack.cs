using System;
using UnityEngine;

public class TestStack
{
    private object[] a;
    private int top;

    public TestStack(int capacity = 16)
    {
        a = new object[capacity];
        top = -1;
    }

    public void Push(object data)
    {
        if (top == a.Length - 1)
        {
            //throw 하거나 아랯처럼 배열 확장
            ResizeStack();
        }
    }

    private void ResizeStack()
    {
        int capacity = 2 * a.Length;

        var tempArray = new object[capacity];
        Array.Copy(a, tempArray, a.Length);
        a = tempArray;
    }

    public object Pop()
    {
        if (this.IsEmpty)
        {
            throw new ApplicationException("Empty");
        }

        return a[top--];
    }

    public object Peek()
    {
        if (this.IsEmpty)
        {
            throw new ApplicationException("Empty");
        }

        return a[top];
    }

    public bool IsEmpty
    {
        get { return top == -1; }
    }

    public int Capacity
    {
        get { return a.Length; }
    }
}
