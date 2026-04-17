# 渲染调试清单

## Frame Debugger使用

### 启动Frame Debugger
1. Window > Analysis > Frame Debugger
2. 点击Enable开始捕获
3. 使用滑块逐步查看渲染过程

### 关键检查点
- Draw Call顺序和数量
- RenderTexture切换
- Shader状态变化
- 批处理中断原因

## Draw Call优化

### 合批失败常见原因
1. 材质不同
2. 贴图不同
3. 顶点数超限
4. 使用不同的Shader关键字
5. GameObject缩放不一致

### 诊断方法
```csharp
// Stats窗口查看
// Game视图 > Stats按钮
// Batches: 实际批次数
// Saved by batching: 节省的批次
```

## Overdraw检测

### Scene视图检查
1. Scene视图 > Shading Mode
2. 选择Overdraw模式
3. 红色区域表示过度绘制

### 优化策略
- UI Canvas分层
- 遮挡剔除（Occlusion Culling）
- 调整渲染顺序
- 减少透明物体重叠

## Shader问题排查

### 常见错误
```csharp
// Pink材质：Shader编译失败
// 黑色材质：贴图丢失
// 紫色材质：Shader引用错误
```

### Shader变体检查
```csharp
// Project Settings > Graphics
// Shader Stripping设置
// Always Included Shaders列表
```

## 光照调试

### 光照贴图问题
- 黑斑：UV重叠
- 漏光：Lightmap分辨率低
- 接缝：Padding不足

### 实时光照性能
```csharp
// 限制实时光源数量
QualitySettings.pixelLightCount = 2;

// 使用Light Layers分层照明
light.renderingLayerMask = 1 << 5;
```

## 后处理优化

### 性能影响排序
1. SSAO（屏幕空间环境光遮蔽）
2. Motion Blur（运动模糊）
3. DOF（景深）
4. Bloom（辉光）
5. Color Grading（色彩分级）

### 移动端建议
- 避免使用HDR
- 简化后处理栈
- 降低渲染分辨率

## UI渲染优化

### Canvas优化
```csharp
// 分离动静Canvas
// Static UI Canvas
// Dynamic UI Canvas（频繁更新）
```

### Raycast Target
- 关闭不需要交互的UI元素
- 减少射线检测开销

## 材质优化

### Shader选择
- Mobile/Diffuse（移动端）
- Standard（桌面端）
- URP/Lit（URP管线）

### 材质属性
```csharp
// 运行时修改优化
renderer.sharedMaterial // 共享材质
renderer.material     // 实例化材质（产生新实例）
```