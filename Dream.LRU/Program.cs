LRUCache<int, int> lruCache = new LRUCache<int, int>(5);
lruCache.callbackMethod += (k, v) => Console.WriteLine("淘汰" + k + "=" + v);
lruCache.Put(1, 1);
lruCache.Put(2, 2);
lruCache.Put(3, 3);
lruCache.Put(4, 4);
lruCache.Put(5, 5);
lruCache.Put(6, 6);

foreach (var item in lruCache.List())
{
    Console.WriteLine($"{item.Key} = {item.Value}");
}
Console.ReadLine();
public class LRUCache<K, V> : CacheStrategy<K, V>
{
    private Dictionary<K, V> map;


    private int capacity;
    private LinkedList<K> queue;
    public delegate void Callback(K key, V value);
    public Callback callbackMethod;

    public LRUCache(int capacity)
    {
        this.capacity = capacity;
        this.map = new Dictionary<K, V>();
        this.queue = new LinkedList<K>();
    }

    public void Put(K key, V value)
    {

        if (map.ContainsKey(key))
        {
            queue.Remove(key);
        }
        queue.AddFirst(key);
        map.Add(key, value);

        //上限
        if (queue.Count > capacity)
        {
            K last = queue.Last();
            queue.RemoveLast();

            V removeValue = map[last];
            map.Remove(last);

            callbackMethod?.Invoke(last, removeValue);

        }
    }

    public V? Get(K key)
    {
        // 如果已经缓存过该数据
        if (map.ContainsKey(key))
        {
            queue.Remove(key);
            queue.AddFirst(key);
            return map[key];
        }
        return default;
    }

    public Dictionary<K, V> List()
    {
        return map;
    }



}

public abstract class CacheStrategy<K, V>
{


}