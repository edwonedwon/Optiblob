using System.Collections;
using System.Collections.Generic;

namespace Optiblob 
{
    public class TwoKeyDictionary<T1, T2, T3> where T2 : T1
    {
        // have to implement equatable and gethashcode to prevent memory-heavy boxing
        // when used as Dictionary key
        struct HashKeyPair : System.IEquatable<HashKeyPair>
        {
            public int first, second;

            public HashKeyPair(T1 a, T1 b)
            {
                int aHash = a.GetHashCode();
                int bHash = b.GetHashCode();

                if (aHash < bHash)
                {
                    first = aHash;
                    second = bHash;
                }
                else
                {
                    first = bHash;
                    second = aHash;
                }
            }

            public bool Equals(HashKeyPair other)
            {
                return first == other.first && second == other.second;
            }

            public override int GetHashCode()
            {
                return first ^ second;
            }
        }

        Dictionary<HashKeyPair, T3> internalDictionary;

        public TwoKeyDictionary()
        {
            internalDictionary = new Dictionary<HashKeyPair, T3>();
        }

        public void Add(T1 aKey, T2 bKey, T3 value)
        {
            HashKeyPair internalKey = new HashKeyPair(aKey, bKey);
            internalDictionary.Add(internalKey, value);
        }

        public T3 Get(T1 aKey, T2 bKey)
        {
            return internalDictionary[new HashKeyPair(aKey, bKey)];
        }
    }
}