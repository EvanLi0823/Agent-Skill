# Debug UI Prefabs

此目录包含Unity调试UI预制体。

## 使用方法

1. 将此目录下的预制体文件导入Unity项目
2. 将预制体拖入场景即可使用
3. 可通过Inspector面板调整参数

## 包含的预制体

- **PerformanceOverlay.prefab** - 性能监控覆盖层
- **DebugConsole.prefab** - 运行时调试控制台
- **MemoryMonitor.prefab** - 内存监控面板

## 快速集成

```csharp
// 代码中动态加载
GameObject debugUI = Resources.Load<GameObject>("DebugUI/PerformanceOverlay");
Instantiate(debugUI);
```

注意：预制体文件需要在Unity编辑器中创建，这里只是占位说明。