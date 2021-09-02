using System;
using System.Collections;
using System.Collections.Generic;

namespace Ibra.Enumerables
{
    internal sealed class SingleEnumerable<T> : IEnumerable<T>, IEnumerator<T>, IReadOnlyList<T>, IList<T> where T : notnull
    {
        private class SingleEnumeratorOnly : IEnumerator<T>
        {
            private bool _traversed;
            private readonly SingleEnumerable<T> _enumerable;

            public T Current => _enumerable._element;
            object IEnumerator.Current => Current;

            public SingleEnumeratorOnly(SingleEnumerable<T> e)
            {
                _traversed = false;
                _enumerable = e;
            }

            public bool MoveNext()
            {
                if (_traversed) return false;
                _traversed = true;
                return true;
            }

            public void Reset()
            {
                _traversed = false;
            }

            public void Dispose() { }
        }

        private enum State { NEW, RESET, MOVED }
        private readonly T _element;
        private State _state;

        public SingleEnumerable(T element)
        {
            _element = element;
            _state = State.NEW;
        }

        public T Current => _element;

        public int Count => 1;

        public void Dispose()
        {
            _state = State.NEW;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_state == State.NEW)
            {
                _state = State.RESET;
                return this;
            }
            else return new SingleEnumeratorOnly(this);
        }

        public bool MoveNext()
        {
            if (_state == State.MOVED) return false;
            _state = State.MOVED;
            return true;
        }

        public void Reset()
        {
            _state = State.RESET;
        }

        #region ICollection Explicit Implementation
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T item) => item.Equals(_element);

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            array[arrayIndex] = _element;
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.IsReadOnly => true;
        #endregion

        #region IList Explicit Implementation
        T IList<T>.this[int index]
        {
            get
            {
                if (index == 0) return _element;
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        int IList<T>.IndexOf(T item) => item.Equals(_element) ? 0 : -1;

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IReadOnlyList Explicit Implementation
        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                if (index == 0) return _element;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
        #endregion

        #region IEnumerable Explicit Implementation
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region IEnumerator Explicit Implementation
        object IEnumerator.Current => Current;
        #endregion
    }
}
