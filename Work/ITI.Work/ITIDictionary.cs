using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ITI.Work
{
    public class ITIDictionary<TKey,TValue> : IEnumerable<KeyValuePair<TKey,TValue>>
    {
        int _count;
        Node[] _buckets;
        int _version;

        public ITIDictionary()
        {
            _count = 0;
            _buckets = new Node[7];
            _version = 0;
        }

        class Node
        {
            public readonly TKey Key;
            public TValue Value;
            public Node Next;

            public Node( TKey key, TValue value, Node newNode )
            {
                Key = key;
                Value = value;
                Next = newNode;
            }
        }

        public int Count => _count;

        public void Add( TKey key, TValue value )
        {
            var index = key.GetHashCode() % _buckets.Length;
            Node newNode = new Node(key, value, null);

            Node n = _buckets[index];
            for( ; n.Next != null; n = n.Next );
            _version++;
            n.Next = newNode;
        }

        public void Remove( TKey key )
        {
            var index = key.GetHashCode() % _buckets.Length;

            Node n = _buckets[index];
            for( ; !key.Equals( n.Next.Key ); n = n.Next );
            if ( n.Next != null)
            {
                _version++;
                Node prev = n;
                Node next = n.Next.Next;
                prev.Next = next;
            }

        }

        public TValue this[ TKey key ]
        {
            get
            {
                Node n = _buckets[Math.Abs(key.GetHashCode()) % _buckets.Length];

                for( ; n.Key.Equals( key ); n = n.Next ) ;
                if( n.Next == null )
                {
                    throw new ArgumentOutOfRangeException();
                }
                return n.Value;
            }
            set
            {
                var index = Math.Abs(key.GetHashCode()) % _buckets.Length;
                Node newNode = new Node( key, value, null );

                Node n = _buckets[index];
                for( ; n.Next != null; n = n.Next )
                {
                    if( key.Equals( n.Key ) )
                    {
                        n.Value = value;
                        return;
                    }
                }
                _version++;
                n.Next = newNode;
            }
        }

        private class E : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private ITIDictionary<TKey, TValue> _papa;
            private KeyValuePair<TKey, TValue> _currentValue;
            readonly int _papaVersion;
            private TKey _current;


            public E( ITIDictionary<TKey, TValue> papa )
            {
                this._papa = papa;
                _papaVersion = _papa._version;
                _current = _papa._buckets[0].Key;
            }

            public KeyValuePair<TKey, TValue> Current => _currentValue;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if( _papaVersion != _papa._version )
                {
                    throw new InvalidOperationException();
                }
                Node n = _papa._buckets[Math.Abs(_current.GetHashCode()) % _papa._buckets.Length];

                for( ; n.Key.Equals( _current ); n = n.Next );
                if(n.Next == null)
                {
                    return false;
                }
                _current = n.Key;
                return true;
            }

            public void Dispose()
            {
                throw new NotSupportedException();
            }
            public void Reset()
            {
                throw new NotSupportedException();
            }
        }


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new E( this );


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
