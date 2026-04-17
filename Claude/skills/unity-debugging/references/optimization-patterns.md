# Unity优化设计模式

## 对象池模式（Object Pooling）

避免频繁实例化和销毁：
```csharp
public class ObjectPool<T> where T : Component
{
    Queue<T> pool = new Queue<T>();

    public T Get()
    {
        return pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

## LOD（Level of Detail）

根据距离切换模型精度：
```csharp
// LOD Group组件配置
LOD[] lods = new LOD[]
{
    new LOD(0.6f, highDetail),   // 60%屏幕高度
    new LOD(0.3f, mediumDetail),  // 30%屏幕高度
    new LOD(0.1f, lowDetail)      // 10%屏幕高度
};
```

## 批处理优化

### 静态批处理
- 标记Static的GameObject
- 相同材质自动合批
- 注意内存占用增加

### 动态批处理
- 顶点数<900（根据shader）
- 相同材质和贴图
- 避免缩放不一致

### GPU Instancing
```csharp
MaterialPropertyBlock props = new MaterialPropertyBlock();
props.SetColor("_Color", color);
renderer.SetPropertyBlock(props);
```

## 纹理优化

### 图集打包
```csharp
// Sprite Atlas配置
// 减少Draw Call
// 注意2的幂次方尺寸
```

### 压缩格式选择
- iOS: ASTC 4x4/6x6
- Android: ETC2/ASTC
- PC: DXT5/BC7

## 脚本优化

### Update优化
```csharp
// 避免每帧调用
void Update()
{
    if (Time.frameCount % 30 == 0) // 每30帧执行
    {
        SlowOperation();
    }
}
```

### 缓存组件引用
```csharp
Transform cached;
void Awake()
{
    cached = GetComponent<Transform>();
}
```

## 物理优化

- Fixed Timestep调整（0.02-0.04）
- Layer碰撞矩阵优化
- 简化碰撞体形状
- 避免Mesh Collider