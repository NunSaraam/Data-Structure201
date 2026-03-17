using System;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TestLinkedList<T>
{
    public TestLinkedListNode<T> head;

    public void Add(TestLinkedListNode<T> newNode)
    {
        if (head == null)
        {

            head = newNode;
        }
        else
        {
            var current = head;
            while (current != null && current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }
    }

    public void AddAfter(TestLinkedListNode<T> current, TestLinkedListNode<T> newNode)
    {
        if (head == null || current == null || newNode == null)
        {
            throw new InvalidOperationException();
        }

        newNode.Next = current.Next;
        current.Next = newNode;
    }

    public void Remove(TestLinkedListNode<T> removeNode)
    {
        if (head == null || removeNode == null)
        {
            return;
        }

        if (head == removeNode)
        {
            head = head.Next;
            removeNode = null;
        }
        else
        {
            var current = head;
            while (current != null && current.Next != removeNode)
            {
                current = current.Next;
            }

            if (current != null)
            {
                current.Next = removeNode.Next;
                removeNode = null;
            }
        }
    }

    public TestLinkedListNode<T> GetNode(int index)
    {
        var current = head;
        for (int i = 0; i < index && current != null; i++)
        {
            current = current.Next;
        }

        //만약 index가 리스트 카운트보다 크면
        //null이 리턴됨
        return current;
    }

    public int Count()
    {
        int cnt = 0;
        var current = head;
        while (current != null)
        {
            cnt++;
            current = current.Next;
        }

        return cnt;
    }


    public class TestLinkedListNode<T>
    {
        public T Data { get; set; }
        
        public TestLinkedListNode<T> Next { get; set; }

        public TestLinkedListNode(T data)
        {
            this.Data = data;
            this.Next = null;
        }
    }


    /*
    public TestNode<T> First;
    public TestNode<T> Last;
    public class TestNode<T>
    {
        public TestNode<T> Next {  get; set; }

    }

    private TestNode<T> head;
    private int count;

    public void Add(T data)
    {

    }

    public void Remove(TestNode<T> node)
    {
        while ()
        {
            //next next
        }
    }

    public TestNode<T> Find(T value)
    {
        while ()
        {
            //next next
        }
        return new TestNode<T>();
    }*/
}
