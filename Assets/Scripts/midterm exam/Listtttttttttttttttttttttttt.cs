using System;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Listtttttttttttttttttttttttt<T> : MonoBehaviour
{
    private T[] _items;
    private int _size;
    private int _version;

    private static readonly T[] _emptyArray = new T[0];

    private void Awake()
    {
        _items = _emptyArray;
    }

    private void Start()
    {
    }

    private void EnsureCapacity(int min)
    {
        if (_items.Length < min)
        {
            int newCapacity = _items.Length == 0 ? 4 : _items.Length * 2;

            if ((uint)newCapacity > 2146435071) newCapacity = 2146435071;
            if (newCapacity < min) newCapacity = min;

            T[] newItems = new T[newCapacity];
            if (_size > 0)
            {
                Array.Copy(_items, 0, newItems, 0, _size);
            }
            _items = newItems;
        }
    }

    private static class ThrowHelper
    {
        public static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException("index", "인덱스가 리스트의 범위를 벗어났습니다.");
        }
    }

    public void Add(T item)
    {
        if (_size == _items.Length) EnsureCapacity(_size + 1);
        _items[_size++] = item;
        _version++;
    }

    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }
        Contract.EndContractBlock();

        _size--;
        if (index < _size)
        {
            Array.Copy(_items, index + 1, _items, index, _size - index);
        }
        _items[_size] = default(T);
        _version++;
    }

    public void Clear()
    {
        if (_size > 0)
        {
            Array.Clear(_items, 0, _size);
            _size = 0;
        }
        _version++;
    }
}