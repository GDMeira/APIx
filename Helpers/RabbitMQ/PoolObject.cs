using Microsoft.Extensions.ObjectPool;

namespace APIx.Helpers.RabbitMQ;

public class PoolObject<T> : IDisposable where T : class
{
    private DefaultObjectPool<T> _pool;
    public T Item { get; set; }

    public PoolObject(DefaultObjectPool<T> pool)
    {
        _pool = pool;
        this.Item = _pool.Get();
    }

    public void Dispose()
    {
        _pool.Return(this.Item);
    }
}

