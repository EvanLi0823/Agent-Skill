using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 运行时调试辅助组件，提供常用调试功能
/// </summary>
public class DebugHelper : MonoBehaviour
{
    [Header("调试设置")]
    public bool showDebugInfo = true;
    public bool enableFrameRateLimit = false;
    [Range(1, 120)]
    public int targetFrameRate = 60;

    [Header("性能监控")]
    public bool showFPS = true;
    public bool showMemory = true;
    public bool showDrawCalls = false;

    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private float memoryUsage = 0.0f;

    private GUIStyle guiStyle;
    private Rect debugRect;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeGUI();
    }

    void InitializeGUI()
    {
        guiStyle = new GUIStyle();
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = Color.green;
        guiStyle.fontStyle = FontStyle.Bold;

        debugRect = new Rect(10, 10, 300, 100);
    }

    void Update()
    {
        if (enableFrameRateLimit)
        {
            Application.targetFrameRate = targetFrameRate;
        }

        // 计算FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;

        // 计算内存使用
        if (showMemory)
        {
            memoryUsage = System.GC.GetTotalMemory(false) / 1048576f; // 转换为MB
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        string debugText = "";

        if (showFPS)
        {
            Color fpsColor = fps > 55 ? Color.green : (fps > 30 ? Color.yellow : Color.red);
            guiStyle.normal.textColor = fpsColor;
            debugText += $"FPS: {fps:F1}\n";
        }

        if (showMemory)
        {
            debugText += $"Memory: {memoryUsage:F1} MB\n";
        }

        if (showDrawCalls)
        {
            debugText += $"Draw Calls: {UnityEngine.Rendering.RenderPipelineManager.currentPipeline}\n";
        }

        GUI.Label(debugRect, debugText, guiStyle);
    }

    /// <summary>
    /// 截屏功能
    /// </summary>
    [ContextMenu("Take Screenshot")]
    public void TakeScreenshot()
    {
        string filename = $"Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        ScreenCapture.CaptureScreenshot(filename);
        Debug.Log($"Screenshot saved: {filename}");
    }

    /// <summary>
    /// 时间缩放调试
    /// </summary>
    public void SetTimeScale(float scale)
    {
        Time.timeScale = Mathf.Clamp(scale, 0f, 5f);
        Debug.Log($"Time Scale set to: {Time.timeScale}");
    }

    /// <summary>
    /// 清理内存
    /// </summary>
    [ContextMenu("Force GC")]
    public void ForceGarbageCollection()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        Debug.Log("Forced garbage collection completed");
    }
}