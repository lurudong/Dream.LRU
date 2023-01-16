//LRUCache<int, int> lruCache = new LRUCache<int, int>(5);
//lruCache._callbackMethod += (k, v) => Console.WriteLine("淘汰" + k + "=" + v);
//lruCache.Put(1, 1);
//lruCache.Put(2, 2);
//lruCache.Put(3, 3);
//lruCache.Put(4, 4);
//lruCache.Put(5, 5);
//lruCache.Put(6, 6);

//foreach (var item in lruCache.List())
//{
//    Console.WriteLine($"{item.Key} = {item.Value}");
//}


using System.Collections.Concurrent;

Console.WriteLine("-------------------------------------------------------------------");
lrukCacheTest();
Console.ReadLine();

void lrukCacheTest()
{
    LRUKCache<int, int> lrukCache = new LRUKCache<int, int>(2, 3, 1);
    lrukCache.SetHistoryListCallback((o, k) => Console.WriteLine("记录队列淘汰" + o + "=" + k));
    lrukCache._callbackMethod += (k, v) => Console.WriteLine("淘汰" + k + "=" + v);
    lrukCache.Get(1);
    lrukCache.Get(1);
    lrukCache.Get(1);
    lrukCache.Get(2);
    lrukCache.Get(2);
    lrukCache.Get(2);
    lrukCache.Get(3);
    lrukCache.Get(3);
    lrukCache.Get(3);
    lrukCache.Get(4);
    lrukCache.Get(4);
    lrukCache.Get(4);
    lrukCache.Put(1, 2);
    lrukCache.Put(2, 2);
    lrukCache.Put(3, 2);
    lrukCache.Put(4, 2);
    foreach (var item in lrukCache.List())
    {
        Console.WriteLine($"{item.Key} = {item.Value}");
    }
}
public class LRUCache<K, V> : CacheStrategy<K, V>
{
    public LRUCache(int capacity)
    {
        this._capacity = capacity;
        this._map = new ConcurrentDictionary<K, V>();
        this._queue = new LinkedList<K>();
    }

    protected ConcurrentDictionary<K, V> _map;

    protected int _capacity;
    protected LinkedList<K> _queue;
    public delegate void Callback(K key, V value);
    public Callback _callbackMethod;



    public virtual V Put(K key, V value)
    {

        if (_map.ContainsKey(key))
        {
            _queue.Remove(key);
        }
        _queue.AddFirst(key);
        _map.AddOrUpdate(key, value, (key, oldVuale) => value);

        //上限
        if (_queue.Count > _capacity)
        {
            K last = _queue.Last();
            _queue.RemoveLast();

            //V removeValue = _map[last];
            _map.Remove(last, out V removeValue);

            _callbackMethod?.Invoke(last, removeValue);

        }
        return value;
    }

    public virtual V? Get(K key)
    {
        // 如果已经缓存过该数据
        if (_map.ContainsKey(key))
        {
            _queue.Remove(key);
            _queue.AddFirst(key);
            return _map[key];
        }
        return default;
    }

    public virtual void Remove(K key)
    {
        // 如果已经缓存过该数据
        if (_map.ContainsKey(key))
        {

            _map.Remove(key, out V removeValue);
        }
    }


    public virtual ConcurrentDictionary<K, V> List()
    {
        return _map;
    }
}

public class LRUKCache<K, V> : LRUCache<K, V>
{
    // 进入缓存队列的评判标准
    private int _putStandard;

    // 访问数据历史记录
    private LRUCache<object, int> _historyList;

    public LRUKCache(int cacheSize, int historyCapacity, int putStandard) : base(cacheSize)
    {
        this._putStandard = putStandard;
        this._historyList = new LRUCache<object, int>(historyCapacity);
    }

    public override V Get(K key)
    {
        // 记录数据访问次数
        int historyCount = _historyList.Get(key);
        historyCount = historyCount == null ? 0 : historyCount;
        _historyList.Put(key, ++historyCount);
        return base.Get(key);
    }


    public override V Put(K key, V value)
    {
        if (value == null)
        {
            return default;
        }
        // 如果已经在缓存里则直接返回
        if (base.Get(key) != null)
        {
            return base.Put(key, value);
        }
        // 如果数据历史访问次数达到上限，则加入缓存
        int historyCount = _historyList.Get(key);
        historyCount = (historyCount == null) ? 0 : historyCount;
        if (RemoveCache(historyCount))
        {
            // 移除历史访问记录，加入缓存
            _historyList.Remove(key);
            return base.Put(key, value);
        }

        return value;
    }

    private bool RemoveCache(int historyCount)
    {
        return historyCount >= _putStandard;
    }

    public void SetPutStandard(int putStandard)
    {
        this._putStandard = putStandard;
    }


    public void SetCallback(Callback callback)
    {

        base._callbackMethod = callback;
    }

    public void SetHistoryListCallback(LRUCache<object, int>.Callback callback)
    {
        _historyList._callbackMethod += (LRUCache<object, int>.Callback)callback;
    }
}

public interface CacheStrategy<K, V>
{
    public delegate void Callback(K key, V value);






    V Put(K key, V value);


    V? Get(K key);


    void Remove(K key);



    ConcurrentDictionary<K, V> List();

}