# Unity Profiler 使用指南

## 启动和连接

### 编辑器内Profile
1. Window > Analysis > Profiler (Ctrl/Cmd + 7)
2. 点击Record开始记录
3. 运行游戏进行性能采样

### 真机Profile
1. Build Settings启用Development Build和Autoconnect Profiler
2. 确保设备与开发机在同一网络
3. Profiler窗口选择设备连接

## 关键模块分析

### CPU Usage
关注指标：
- Main Thread时间（目标<16.67ms @60FPS）
- Render Thread时间
- Scripts执行时间

常见热点：
- Update/LateUpdate过重
- FindObjectOfType频繁调用
- 字符串拼接导致GC

### GPU Usage
关注指标：
- 渲染时间分布
- Vertex/Fragment处理
- Post-processing耗时

优化方向：
- 减少Overdraw
- 简化Shader计算
- 降低分辨率/质量

### Memory
关注指标：
- Total Allocated
- Texture Memory
- Mesh Memory
- GC.Alloc频率

内存泄漏征兆：
- Total Reserved持续增长
- Object数量异常增加
- Native Memory不释放

## Deep Profile模式

启用方法：
```csharp
// 代码启用
Profiler.enableBinaryLog = true;
Profiler.logFile = "profiler.log";
Profiler.enabled = true;
```

注意事项：
- 性能开销大（5-10倍）
- 仅用于精确定位
- 及时关闭避免内存溢出

## Timeline视图技巧

- 拖拽选择时间范围分析
- F键聚焦选中帧
- 右键标记感兴趣的帧
- Shift选择多帧对比

## 自定义采样

```csharp
using UnityEngine.Profiling;

Profiler.BeginSample("MyCustomOperation");
// 需要分析的代码
Profiler.EndSample();
```