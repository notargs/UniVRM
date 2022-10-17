using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniVRM10.FastSpringBones.Utilities
{
    public struct NativeList<T> : IDisposable, IEnumerable<T> where T : unmanaged
    {
        private readonly Allocator _allocator;
        private readonly NativeArrayOptions _options;

        private NativeArray<T> _buffer;

        public int Length { get; private set; }
        public int Capacity { get; private set; }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                return _buffer[index];
            }
            set
            {
                if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                _buffer[index] = value;
            }
        }

        public NativeList(
            int capacity,
            Allocator allocator,
            NativeArrayOptions options = NativeArrayOptions.ClearMemory
        )
        {
            _allocator = allocator;
            _options = options;
            Capacity = CalcNearPow2(capacity);
            _buffer = new NativeArray<T>(Capacity, _allocator, _options);
            Length = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Length; ++i)
            {
                yield return _buffer[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public NativeSlice<T> GetNativeSlice()
        {
            return new NativeSlice<T>(_buffer, 0, Length);
        }

        public unsafe void Add(T item)
        {
            if (Length >= Capacity)
            {
                Capacity <<= 1;
                var newBuffer = new NativeArray<T>(Capacity, _allocator, _options);
                UnsafeUtility.MemCpy(newBuffer.GetUnsafePtr(), _buffer.GetUnsafePtr(),
                    Marshal.SizeOf<T>() * _buffer.Length);
                _buffer.Dispose();
                _buffer = newBuffer;
            }

            _buffer[Length] = item;
            Length++;
        }

        public unsafe void Remove(int index)
        {
            UnsafeUtility.MemCpy((T*) _buffer.GetUnsafePtr() + index, (T*) _buffer.GetUnsafePtr() + index + 1,
                Marshal.SizeOf<T>() * (Length - index));
            Length--;
        }

        private static int CalcNearPow2(int n)
        {
            if (n <= 0) return 0;
            if ((n & (n - 1)) == 0) return n;
            var ret = 1;
            while (n > 0)
            {
                ret <<= 1;
                n >>= 1;
            }

            return ret;
        }

        public void Dispose()
        {
            _buffer.Dispose();
        }
    }
}