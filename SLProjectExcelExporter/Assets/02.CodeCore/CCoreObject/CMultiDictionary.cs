using System.Collections.Generic;
using System;

public class CMultiDictionary<Key, Value>
{
    private SortedDictionary<Key, List<Value>> dic_ = null;

    public CMultiDictionary()
    {
        dic_ = new SortedDictionary<Key, List<Value>>();
    }

    public CMultiDictionary(IComparer<Key> comparer)
    {
        dic_ = new SortedDictionary<Key, List<Value>>(comparer);
    }

    public void Add(Key key, Value value)
    {
        List<Value> list = null;

        if (dic_.TryGetValue(key, out list))
        {
            list.Add(value);
        }
        else
        {
            list = new List<Value>();
            list.Add(value);
            dic_.Add(key, list);
        }
    }

    public List<Value> Add(Key key)
	{
        List<Value> pList = null;
        if (dic_.ContainsKey(key))
		{
            dic_.TryGetValue(key, out pList);
		}
        else
		{
            pList = new List<Value>();
            dic_.Add(key, pList);
		}

        return pList;
	}

    public bool ContainsKey(Key key)
    {
        return dic_.ContainsKey(key);
    }

	public bool Remove(Key key)
	{
		return dic_.Remove(key);
	}

    public void Clear()
    {
        dic_.Clear();
    }

    public int Count { get { return dic_.Count; } }

    public List<Value> this[Key key]
    {
        get
        {
            List<Value> list = null;
            if (!dic_.TryGetValue(key, out list))
            {
                list = new List<Value>();
                dic_.Add(key, list);
            }

            return list;
        }
    }

    public IEnumerable<Key> keys
    {
        get
        {
            return dic_.Keys;
        }
    }

    public IEnumerable<List<Value>> value
    {
        get
        {
            return dic_.Values;
        }        
    }

    public SortedDictionary<Key, List<Value>>.Enumerator GetEnumerator()
	{
        return dic_.GetEnumerator();
	}

}

public class CSortedListDictionaryCompare<TKey> : IComparer<TKey> where TKey : IComparable
{
    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return -1;
        else
            return result;
    }
}

public class CSortedListDictionaryCompareReverse<TKey> : IComparer<TKey> where TKey : IComparable
{
    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1;
        else
            return -result;
    }
}