using System;
using System.Collections;
using System.Collections.Generic;

public class CustomList<T> : IEnumerable<T>             //2025137064 윤동근    List구현
{
    private T[] _items;
    private int _size;
    private int _version; // foreach 도중 리스트가 변경되었는지 추적하기 위한 변수

    private static readonly T[] _emptyArray = new T[0];

    /// <summary> 
    /// 리스트에 포함된 요소의 실제 개수를 반환한다.
    /// 시간 복잡도: O(1)
    /// </summary>
    public int Count => _size;

    /// <summary> 
    /// 내부 배열의 전체 크기를 반환한다.
    /// 시간 복잡도: O(1)
    /// </summary>
    public int Capacity => _items.Length;

    /// <summary> 
    /// 지정된 인덱스에 있는 요소를 가져오거나 설정한다.
    /// 시간 복잡도: O(1)
    /// </summary>
    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_size)
                ThrowHelper.ThrowArgumentOutOfRangeException();
            return _items[index];
        }
        set
        {
            if ((uint)index >= (uint)_size)
                ThrowHelper.ThrowArgumentOutOfRangeException();
            _items[index] = value;
            _version++;
        }
    }

    // 생성자

    /// <summary> 
    /// 비어 있는 새 CustomList를 초기화한다.
    /// 시간 복잡도: O(1)
    /// </summary>
    public CustomList()
    {
        _items = _emptyArray;
    }

    /// <summary> 
    /// 지정된 초기 용량(Capacity)을 갖는 새 CustomList를 초기화한다.
    /// 시간 복잡도: O(1)
    /// </summary>
    public CustomList(int capacity)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _items = capacity == 0 ? _emptyArray : new T[capacity];
    }

    // 핵심 기능 (추가, 삭제, 초기화)

    /// <summary> 
    /// 리스트의 끝에 객체를 추가한다.
    /// 시간 복잡도: 분할 상환 분석 기준 O(1)
    /// 배열이 가득차서 늘릴 때는 O(n))
    /// </summary>
    public void Add(T item)
    {
        if (_size == _items.Length)
            EnsureCapacity(_size + 1);

        _items[_size++] = item;
        _version++;
    }

    /// <summary> 
    /// 지정된 인덱스에 있는 요소를 제거한다.
    /// 시간 복잡도: 제거한 후 앞으로 땡겨와야해서 O(n)
    /// </summary>
    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        _size--;
        if (index < _size)
        {
            // 삭제된 요소의 뒤쪽 데이터들을 앞으로 한 칸씩 덮어씌움
            Array.Copy(_items, index + 1, _items, index, _size - index);
        }

        _items[_size] = default(T);
        _version++;
    }

    /// <summary> 
    /// 리스트에서 특정 객체가 처음으로 나타나는 위치를 찾아 제거한다.
    /// 시간 복잡도: O(n)
    /// </summary>
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false; // 해당 요소가 리스트에 없으면 false 반환
    }

    /// <summary> 
    /// 리스트의 모든 요소를 제거한다.
    /// 시간 복잡도: O(n)
    /// </summary>
    public void Clear()
    {
        if (_size > 0)
        {
            // 배열 내용을 초기화하여 객체 참조를 해제함
            Array.Clear(_items, 0, _size);
            _size = 0;
        }
        _version++;
    }

    // 검색 기능

    /// <summary> 
    /// 리스트 전체에서 지정된 객체를 검색하여 처음으로 나타나는 인덱스를 반환합니다.
    /// 시간 복잡도: 처음부터 끝까지 순차 탐색해서 O(n)
    /// </summary>
    public int IndexOf(T item)
    {
        return Array.IndexOf(_items, item, 0, _size);
    }

    /// <summary> 
    /// 리스트에 특정 요소가 있는지 여부를 확인한다.
    /// 시간 복잡도: IndexOf를 호출하여 탐색 O(n)
    /// </summary>
    public bool Contains(T item)
    {
        return IndexOf(item) >= 0;
    }

    // 내부 유틸리티 (용량 확보, 예외 처리)

    /// <summary> 
    /// 리스트 내부 배열의 용량이 최소 요구량 이상이 되도록 보장한다.
    /// 시간 복잡도: 새 배열을 할당하고 기존 배열의 모든 요소를 복사해서 O(n)
    /// </summary>
    private void EnsureCapacity(int min)
    {
        if (_items.Length < min)
        {
            // 용량이 부족할 경우 기존 크기의 2배로 늘림
            int newCapacity = _items.Length == 0 ? 4 : _items.Length * 2;

            // 최대 배열 크기 제한
            if ((uint)newCapacity > 2146435071) newCapacity = 2146435071;
            if (newCapacity < min) newCapacity = min;

            T[] newItems = new T[newCapacity];
            if (_size > 0)
            {
                // 기존 배열 요소들을 새로운 배열로 복사 O(n)
                Array.Copy(_items, 0, newItems, 0, _size);
            }
            _items = newItems;
        }
    }

    private static class ThrowHelper
    {
        /// <summary> 
        /// 인덱스 범위를 벗어났을 때 발생하는 예외를 처리한다.
        /// 시간 복잡도: O(1)
        /// </summary>
        public static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException("index", "인덱스가 리스트의 범위를 벗어났습니다.");
        }
    }

    // IEnumerable<T> 구현 (foreach 사용을 위해 필수)

    /// <summary> 
    /// 컬렉션을 반복하는 열거자를 반환한다.
    /// 시간 복잡도: 호출 자체는 O(1), 전체 순회는 O(n)
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        int startVersion = _version;
        for (int i = 0; i < _size; i++)
        {
            // foreach 도중에 외부에서 Add나 Remove가 호출되면 예외 발생
            if (_version != startVersion)
            {
                throw new InvalidOperationException("열거하는 동안 컬렉션이 수정되었습니다.");
            }
            yield return _items[i];
        }
    }

    /// <summary> 
    /// 비제네릭 IEnumerator를 반환한다.
    /// 시간 복잡도: O(1)
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}