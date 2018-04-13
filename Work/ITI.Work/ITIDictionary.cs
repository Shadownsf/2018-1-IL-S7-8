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

        public ITIDictionary()
        {
            _count = 0;
            _buckets = new Node[7];
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
            n.Next = newNode;
        }

        public void Remove( TKey key )
        {
            var index = key.GetHashCode() % _buckets.Length;

            Node n = _buckets[index];
            for( ; !key.Equals( n.Next.Key ); n = n.Next ) ;
            if ( n.Next != null)
            {
                Node prev = n;
                Node next = n.Next.Next;
                prev.Next = next;
            }

        }

        public TValue this[ TKey key ]
        {
            get { return default( TValue ); }
            set
            {
                var index = key.GetHashCode() % _buckets.Length;
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
                n.Next = newNode;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
