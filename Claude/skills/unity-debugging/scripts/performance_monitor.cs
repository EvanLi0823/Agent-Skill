using UnityEngine;
using UnityEngine.UI;
using System.Text;

/// <summary>
/// 性能监控覆盖层，显示详细的性能指标
/// </summary>
public class PerformanceMonitor : MonoBehaviour
{
    [Header("显示设置")]
    public bool showPerformanceOverlay = true;
    public Color normalColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color criticalColor = Color.red;

    [Header("性能阈值")]
    public float fpsWarningThreshold = 50f;
    public float fpsCriticalThreshold = 30f;
    public float memoryWarningThreshold = 500f; // MB
    public float memoryCriticalThreshold = 800f; // MB

    // 性能数据
    private float fps;
    private float minFps = float.MaxValue;
    private float maxFps = 0;
    private float avgFps;
    private float fpsSum;
    private int fpsCount;

    private float frameTime;
    private int drawCalls;
    private int triangles;
    private int vertices;

    private float totalMemory;
    private float gcMemory;
    private float textureMemory;
    private float meshMemory;

    // 更新间隔
    private float updateInterval = 0.5f;
    private float timeSinceUpdate = 0f;

    // UI
    private Text displayText;
    private StringBuilder sb = new StringBuilder();

    void Start()
    {
        CreateOverlayUI();
        InvokeRepeating(nameof(UpdateMemoryStats), 1f, 1f);
    }

    void CreateOverlayUI()
    {
        // 创建Canvas
        GameObject canvasGO = new GameObject("PerformanceOverlay");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();

        // 创建背景
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform, false);
        Image bg = bgGO.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 1);
        bgRect.anchorMax = new Vector2(0, 1);
        bgRect.pivot = new Vector2(0, 1);
        bgRect.anchoredPosition = new Vector2(10, -10);
        bgRect.sizeDelta = new Vector2(300, 200);

        // 创建文本
        GameObject textGO = new GameObject("PerformanceText");
        textGO.transform.SetParent(bgGO.transform, false);
        displayText = textGO.AddComponent<Text>();
        displayText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        displayText.fontSize = 12;
        displayText.color = Color.white;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 5);
        textRect.offsetMax = new Vector2(-5, -5);

        DontDestroyOnLoad(canvasGO);
    }

    void Update()
    {
        timeSinceUpdate += Time.unscaledDeltaTime;

        // 计算FPS
        float currentFps = 1f / Time.unscaledDeltaTime;
        fps = Mathf.Lerp(fps, currentFps, 0.1f);

        fpsSum += currentFps;
        fpsCount++;
        avgFps = fpsSum / fpsCount;

        minFps = Mathf.Min(minFps, currentFps);
        maxFps = Mathf.Max(maxFps, currentFps);

        frameTime = Time.unscaledDeltaTime * 1000f; // ms

        if (timeSinceUpdate > updateInterval)
        {
            UpdateDisplay();
            timeSinceUpdate = 0;
        }
    }

    void UpdateMemoryStats()
    {
        gcMemory = System.GC.GetTotalMemory(false) / 1048576f;
        totalMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
        textureMemory = UnityEngine.Profiling.Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576f;
    }

    void UpdateDisplay()
    {
        if (!showPerformanceOverlay || displayText == null) return;

        sb.Clear();

        // FPS部分
        Color fpsColor = GetColorByThreshold(fps, fpsCriticalThreshold, fpsWarningThreshold, true);
        sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(fpsColor)}>FPS: {fps:F1} ({frameTime:F1}ms)</color>");
        sb.AppendLine($"Min/Avg/Max: {minFps:F0}/{avgFps:F0}/{maxFps:F0}");

        // 内存部分
        Color memColor = GetColorByThreshold(gcMemory, memoryCriticalThreshold, memoryWarningThreshold, false);
        sb.AppendLine($"\n<color=#{ColorUtility.ToHtmlStringRGB(memColor)}>Memory:</color>");
        sb.AppendLine($"GC: {gcMemory:F1} MB");
        sb.AppendLine($"Total: {totalMemory:F1} MB");
        sb.AppendLine($"Texture: {textureMemory:F1} MB");

        // 质量设置
        sb.AppendLine($"\nQuality: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");
        sb.AppendLine($"VSync: {QualitySettings.vSyncCount}");
        sb.AppendLine($"Target FPS: {Application.targetFrameRate}");

        displayText.text = sb.ToString();
    }

    Color GetColorByThreshold(float value, float critical, float warning, bool inverse)
    {
        if (inverse)
        {
            if (value < critical) return criticalColor;
            if (value < warning) return warningColor;
        }
        else
        {
            if (value > critical) return criticalColor;
            if (value > warning) return warningColor;
        }
        return normalColor;
    }

    public void ResetStats()
    {
        minFps = float.MaxValue;
        maxFps = 0;
        avgFps = 0;
        fpsSum = 0;
        fpsCount = 0;
    }
}