---
name: unity-debugging
description: Unity游戏引擎调试技能。提供性能分析、内存优化、渲染调试和移动端优化的系统化方法。适用于解决帧率问题、内存泄漏、Draw Call优化、shader错误、空引用异常等Unity开发中的常见问题。包含Profiler使用、Frame Debugger分析、Memory Profiler检查等Unity特定工具的实战指南。
version: 1.0.0
license: MIT
---

# Unity Debugging

## Overview

系统化的Unity调试方法论，通过Unity专属工具链快速定位并解决性能、内存、渲染和逻辑问题。

## 核心调试流程

### 1. 问题识别（Identify）
使用Unity Console和日志系统定位问题：
- 检查Console窗口的Error/Warning
- 添加`Debug.Log()`跟踪执行流程
- 启用Development Build查看详细堆栈
- 使用`UnityEngine.Profiling.Profiler.BeginSample()`标记关键代码段

### 2. 性能分析（Analyze）
通过Unity Profiler深度分析：
```csharp
// Window > Analysis > Profiler
// 关注CPU Usage、GPU Usage、Memory模块
// 详细指南见：references/profiler-guide.md
```

### 3. 问题隔离（Isolate）
二分法排查和最小复现：
- GameObject逐个禁用测试
- Scene简化到最小复现条件
- Deep Profile模式精确定位
- 创建独立测试场景验证

### 4. 解决实施（Fix）
根据问题类型应用对应方案：
- 性能问题：`references/optimization-patterns.md`
- 内存问题：`scripts/memory_tracker.cs`
- 渲染问题：`references/rendering-debug.md`

### 5. 验证测试（Verify）
确保修复有效且无回归：
- Profiler数据对比验证
- 真机测试（尤其移动端）
- 性能基准测试：`scripts/performance_monitor.cs`

## 快速诊断工具

### Unity内置工具
```csharp
// Profiler快捷键
// Ctrl/Cmd + 7: 打开Profiler
// Window > Analysis > Frame Debugger
// Window > Analysis > Memory Profiler (需安装Package)
```

### Inspector调试技巧
- 切换到Debug模式查看私有变量
- 运行时修改参数实时测试
- 锁定Inspector观察特定对象

### 控制台增强
```csharp
// 条件日志
[System.Diagnostics.Conditional("DEBUG")]
void DebugLog(string msg) => Debug.Log(msg);

// 富文本日志
Debug.Log("<color=red>Error:</color> <b>Critical issue</b>");
```

## 常见问题速查

### 性能问题
- FPS低于30：检查Update/FixedUpdate中的循环
- CPU峰值：查看Profiler的GC.Collect
- GPU瓶颈：Frame Debugger检查Overdraw

### 内存问题
- 持续增长：Memory Profiler对比快照
- 纹理过大：检查Import Settings的Max Size
- 未释放资源：查找Destroy后的引用链

### 渲染问题
- Draw Call过多：检查动态批处理条件
- Shader错误：查看Frame Debugger的shader状态
- 材质丢失：确认材质球的shader引用

## 移动端调试

### Android
```bash
# ADB连接调试
adb logcat -s Unity
# 性能监控
adb shell dumpsys gfxinfo <package_name>
```

### iOS
- Xcode Instruments配置
- 真机Profiler连接设置
详见：`references/mobile-optimization.md`

## Resources

### scripts/
- `debug_helper.cs` - 运行时调试辅助组件
- `performance_monitor.cs` - FPS和内存监控覆盖层
- `memory_tracker.cs` - 自动内存泄漏检测

### references/
- `profiler-guide.md` - Unity Profiler完整使用指南
- `optimization-patterns.md` - 性能优化设计模式
- `rendering-debug.md` - 渲染问题调试清单
- `mobile-optimization.md` - 移动端特定优化

### assets/
- `debug-ui-prefab/` - 调试UI预制体（FPS计数器、内存显示等）
