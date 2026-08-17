using System;
using System.Buffers;

namespace SealScript.Collections;

public struct PooledBuffer<T> : IDisposable
{
    private T[] _array;
    private int _size;

    public PooledBuffer()
    {
        _array = [];
        _size = 0;
    }

    public PooledBuffer(int length)
    {
        _array = ArrayPool<T>.Shared.Rent(length);
        _size = length;
    }
    
    public int Length => _size;

    public int Capacity
    {
        get
        {
            ThrowIfDisposed();
            return _array.Length;
        }
    }

    public ref T this[int index] => ref _array[index];

    public static implicit operator Span<T>(PooledBuffer<T> buffer)
        => buffer.AsSpan();
    public static implicit operator ReadOnlySpan<T>(PooledBuffer<T> buffer)
        => buffer.AsReadonlySpan();

    public Span<T> AsSpan()
    {
        ThrowIfDisposed();
        return new Span<T>(_array, 0, _size);
    }

    public ReadOnlySpan<T> AsReadonlySpan()
    {
        ThrowIfDisposed();
        return new ReadOnlySpan<T>(_array, 0, _size);
    }

    public void Resize(int newLength)
    {
        ThrowIfDisposed();
        
        ArgumentOutOfRangeException.ThrowIfNegative(newLength, nameof(newLength));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(newLength, _array.Length, nameof(newLength));
        
        _size = newLength;
    }

    public void Dispose()
    {
        if (_array == null)
        {
            return;
        }

        if (_array.Length != 0)
        {
            ArrayPool<T>.Shared.Return(_array);
        }
        
        _array = null;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_array == null, this);
    }
}