using System;
using System.Collections;
using System.Collections.Generic;

namespace Ibra.Enumerables
{
    internal sealed class SingleEnumerable<T> : IEnumerable<T>, IEnumerator<T>
    {
        private class SingleEnumeratorOnly : IEnumerator<T>
        {
            private bool _traversed;
            private readonly SingleEnumerable<T> _enumerable;

            public T Current
            {
                get
                {
                    if (_traversed) return _enumerable._element;
                    else throw new NotSupportedException();
                }
            }

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

        public T Current
        {
            get
            {
                if (_state == State.MOVED) return _element;
                else throw new NotSupportedException();
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public IEnumerator<T> GetEnumerator()
        {
            if (_state == State.NEW) return this;
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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
