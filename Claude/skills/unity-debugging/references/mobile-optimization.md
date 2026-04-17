# 移动端优化指南

## Android调试

### ADB工具使用
```bash
# 查看Unity日志
adb logcat -s Unity

# 清除日志缓冲
adb logcat -c

# 保存日志到文件
adb logcat -s Unity > unity_log.txt

# 性能数据
adb shell dumpsys gfxinfo <package_name>
```

### Android Profiler连接
1. Build Settings > Development Build
2. 启用Autoconnect Profiler
3. USB调试或WiFi连接
4. Profiler选择AndroidPlayer

### 常见问题
- 黑屏：检查Graphics API顺序
- 崩溃：查看logcat的signal 11
- 发热：降低目标帧率到30FPS

## iOS调试

### Xcode配置
```bash
# 真机调试设置
1. 开发者证书配置
2. Build Settings > Development Build
3. Xcode > Product > Profile
```

### Instruments分析
- Time Profiler：CPU分析
- Allocations：内存分配
- GPU Frame Capture：GPU调试
- Energy Log：电池消耗

### 常见优化点
- Metal API优先
- 避免使用OnGUI
- 减少Transparent渲染

## 性能目标

### 推荐配置
```csharp
// 帧率设置
Application.targetFrameRate = 30; // 省电
Application.targetFrameRate = 60; // 流畅

// 垂直同步
QualitySettings.vSyncCount = 0;
```

### 内存限制
- 低端设备：<1GB RAM可用
- 中端设备：1-2GB RAM
- 高端设备：>2GB RAM

## 电池优化

### 降低功耗策略
1. 降低屏幕亮度提示
2. 减少网络请求频率
3. 后台暂停不必要的更新
4. 使用对象池减少GC

### 温度控制
```csharp
// 动态调整质量
if (deviceTemperature > threshold)
{
    QualitySettings.SetQualityLevel(0); // 最低质量
    Application.targetFrameRate = 30;
}
```

## 包体优化

### 资源压缩
- 纹理：ASTC/ETC2压缩
- 音频：MP3/Vorbis
- 模型：压缩顶点数据

### 代码裁剪
```csharp
// Player Settings
- Strip Engine Code
- IL2CPP优化级别
- Managed Stripping Level
```

### 分包策略
- AssetBundle按需下载
- Addressable系统
- 首包控制在100MB以内

## 适配问题

### 屏幕适配
```csharp
// 安全区域处理
Rect safeArea = Screen.safeArea;
// 刘海屏、挖孔屏适配
```

### 性能分级
```csharp
// 根据设备性能调整
SystemInfo.graphicsMemorySize
SystemInfo.processorCount
SystemInfo.systemMemorySize
```