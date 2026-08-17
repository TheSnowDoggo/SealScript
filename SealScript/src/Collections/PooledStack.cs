using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SealScript.Collections;

public class PooledStack<T> : IReadOnlyCollection<T>, IDisposable, ICloneable
{
    private T[] _array;
    private int _size;
    
    public PooledStack()
    {
        _array = [];
    }
    
    public PooledStack(int initialCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(initialCapacity, nameof(initialCapacity));
        _array = ArrayPool<T>.Shared.Rent(initialCapacity);
    }

    public int Count => _size;

    public int Capacity
    {
        get
        {
            ThrowIfDisposed();
            return _array.Length;
        }
    }

    public T Pop()
    {
        ThrowIfDisposed();
        
        if (_size == 0)
        {
            throw new InvalidOperationException("Stack is empty.");
        }

        _size--;

        T item = _array[_size];

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _array[_size] = default;
        }

        return item;
    }

    public T Peek()
    {
        ThrowIfDisposed();
        
        if (_size == 0)
        {
            throw new InvalidOperationException("Stack is empty.");
        }

        return _array[_size - 1];
    }

    public bool TryPeek([MaybeNullWhen(false)] out T item)
    {
        ThrowIfDisposed();

        if (_size == 0)
        {
            item = default;
            return false;
        }

        item = _array[_size - 1];
        return true;
    }

    public bool TryPop(out T item)
    {
        ThrowIfDisposed();
        
        if (_size == 0)
        {
            item = default;
            return false;
        }
        
        _size--;

        item = _array[_size];

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _array[_size] = default;
        }

        return true;
    }
    
    public void Push(T item)
    {
        ThrowIfDisposed();

        if (_size >= _array.Length)
        {
            Grow(_size + 1);
        }
        
        _array[_size] = item;
        _size++;
    }

    public void Clear()
    {
        ThrowIfDisposed();
        
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            Array.Clear(_array, 0, _size);
        }

        _size = 0;
    }
    
    public int EnsureCapacity(int capacity)
    {
        ThrowIfDisposed();
        
        ArgumentOutOfRangeException.ThrowIfNegative(capacity, nameof(capacity));

        if (_array.Length < capacity)
        {
            Grow(capacity);
        }
        
        return _array.Length;
    }
    
    public bool Contains(T item)
    {
        ThrowIfDisposed();
        
        return _size != 0 && Array.LastIndexOf(_array, item, _size - 1) != -1;
    }

    public T[] ToArray()
    {
        ThrowIfDisposed();
        
        if (_size == 0)
        {
            return [];
        }

        var array = new T[_size];

        for (int i = 0; i < _size; i++)
        {
            array[i] = _array[_size - 1 - i];
        }

        return array;
    }
    
    public void Dispose()
    {
        if (_array == null)
        {
            return;
        }

        if (_array.Length > 0)
        {
            ArrayPool<T>.Shared.Return(_array);
        }
        
        GC.SuppressFinalize(this);
        
        _array = null;
    }

    public object Clone()
    {
        ThrowIfDisposed();
        
        var stack = new PooledStack<T>(_size);
        stack._size = _size;
        
        Array.Copy(_array, stack._array, _size);

        return stack;
    }

    public IEnumerator<T> GetEnumerator()
    {
        ThrowIfDisposed();

        for (int i = 0; i < _size; i++)
        {
            yield return _array[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void Grow(int capacity)
    {
        int newSize = _array.Length == 0 ? 4 : _array.Length * 2;
        
        if (newSize > Array.MaxLength)
        {
            newSize = Array.MaxLength;
        }

        if (newSize < capacity)
        {
            newSize = capacity;
        }

        if (_array.Length != 0)
        {
            ArrayPool<T>.Shared.Return(_array);
        }
        
        var newArray = ArrayPool<T>.Shared.Rent(newSize);
        
        Array.Copy(_array, newArray, _size);
        
        _array = newArray;
    }
    
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_array == null, this);
    }
}