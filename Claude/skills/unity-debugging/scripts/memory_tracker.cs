using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Profiling;

/// <summary>
/// 内存泄漏检测工具，追踪对象的生命周期
/// </summary>
public class MemoryTracker : MonoBehaviour
{
    [Header("追踪设置")]
    public bool enableTracking = true;
    public bool trackGameObjects = true;
    public bool trackTextures = true;
    public bool trackMeshes = true;
    public bool trackMaterials = true;
    public bool trackAudioClips = false;

    [Header("报告设置")]
    public float reportInterval = 10f; // 秒
    public int suspiciousThreshold = 100; // 可疑的对象数量阈值

    // 追踪数据
    private Dictionary<System.Type, List<Object>> trackedObjects = new Dictionary<System.Type, List<Object>>();
    private Dictionary<System.Type, int> previousCounts = new Dictionary<System.Type, int>();
    private float lastReportTime;

    // 内存快照
    private long lastTotalMemory;
    private long memoryDelta;
    private List<MemorySnapshot> snapshots = new List<MemorySnapshot>();

    [System.Serializable]
    public class MemorySnapshot
    {
        public float time;
        public long totalMemory;
        public long gcMemory;
        public Dictionary<string, int> objectCounts;

        public MemorySnapshot()
        {
            time = Time.realtimeSinceStartup;
            totalMemory = Profiler.GetTotalAllocatedMemoryLong();
            gcMemory = System.GC.GetTotalMemory(false);
            objectCounts = new Dictionary<string, int>();
        }
    }

    void Start()
    {
        if (enableTracking)
        {
            InvokeRepeating(nameof(CollectMemoryData), 1f, reportInterval);
        }
    }

    void CollectMemoryData()
    {
        if (!enableTracking) return;

        MemorySnapshot snapshot = new MemorySnapshot();

        // 追踪不同类型的对象
        if (trackGameObjects)
            TrackObjectType<GameObject>(snapshot);

        if (trackTextures)
            TrackObjectType<Texture2D>(snapshot);

        if (trackMeshes)
            TrackObjectType<Mesh>(snapshot);

        if (trackMaterials)
            TrackObjectType<Material>(snapshot);

        if (trackAudioClips)
            TrackObjectType<AudioClip>(snapshot);

        snapshots.Add(snapshot);

        // 分析内存变化
        AnalyzeMemoryTrends();

        // 生成报告
        if (Time.realtimeSinceStartup - lastReportTime > reportInterval)
        {
            GenerateReport();
            lastReportTime = Time.realtimeSinceStartup;
        }

        // 限制快照数量
        if (snapshots.Count > 100)
        {
            snapshots.RemoveAt(0);
        }
    }

    void TrackObjectType<T>(MemorySnapshot snapshot) where T : Object
    {
        T[] objects = Resources.FindObjectsOfTypeAll<T>();
        int count = objects.Length;
        string typeName = typeof(T).Name;

        snapshot.objectCounts[typeName] = count;

        // 检测潜在泄漏
        if (previousCounts.ContainsKey(typeof(T)))
        {
            int delta = count - previousCounts[typeof(T)];
            if (delta > suspiciousThreshold)
            {
                Debug.LogWarning($"[MemoryTracker] 可疑的{typeName}增长: +{delta} (当前总数: {count})");

                // 记录新增对象的详细信息
                if (delta > 0 && delta < 10) // 只记录少量新增对象的详情
                {
                    var newObjects = objects.OrderBy(o => o.GetInstanceID()).TakeLast(delta);
                    foreach (var obj in newObjects)
                    {
                        Debug.Log($"  新增{typeName}: {obj.name} (ID: {obj.GetInstanceID()})");
                    }
                }
            }
        }

        previousCounts[typeof(T)] = count;

        // 更新追踪列表
        if (!trackedObjects.ContainsKey(typeof(T)))
        {
            trackedObjects[typeof(T)] = new List<Object>();
        }
        trackedObjects[typeof(T)].Clear();
        trackedObjects[typeof(T)].AddRange(objects);
    }

    void AnalyzeMemoryTrends()
    {
        if (snapshots.Count < 2) return;

        var current = snapshots[snapshots.Count - 1];
        var previous = snapshots[snapshots.Count - 2];

        long memDelta = current.totalMemory - previous.totalMemory;
        float timeDelta = current.time - previous.time;

        if (memDelta > 0)
        {
            float rate = memDelta / 1048576f / timeDelta; // MB/s
            if (rate > 0.5f) // 每秒增长超过0.5MB
            {
                Debug.LogWarning($"[MemoryTracker] 内存快速增长: {rate:F2} MB/s");
            }
        }
    }

    void GenerateReport()
    {
        Debug.Log("========== Memory Tracker Report ==========");

        if (snapshots.Count > 0)
        {
            var latest = snapshots[snapshots.Count - 1];
            Debug.Log($"总内存: {latest.totalMemory / 1048576f:F2} MB");
            Debug.Log($"GC内存: {latest.gcMemory / 1048576f:F2} MB");

            Debug.Log("\n对象计数:");
            foreach (var kvp in latest.objectCounts)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value}");
            }
        }

        // 查找未释放的对象
        FindLeakedObjects();

        Debug.Log("==========================================");
    }

    void FindLeakedObjects()
    {
        foreach (var kvp in trackedObjects)
        {
            var type = kvp.Key;
            var objects = kvp.Value;

            // 过滤出可能泄漏的对象（例如：隐藏但未销毁的GameObject）
            if (type == typeof(GameObject))
            {
                var leaked = objects.Where(o => o != null &&
                    !((GameObject)o).activeInHierarchy &&
                    ((GameObject)o).transform.parent == null).ToList();

                if (leaked.Count > 0)
                {
                    Debug.LogWarning($"[MemoryTracker] 发现{leaked.Count}个可能泄漏的GameObject");
                    foreach (var obj in leaked.Take(5)) // 只显示前5个
                    {
                        Debug.Log($"  可能泄漏: {obj.name}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 手动触发内存快照
    /// </summary>
    [ContextMenu("Take Memory Snapshot")]
    public void TakeSnapshot()
    {
        CollectMemoryData();
        Debug.Log("[MemoryTracker] 内存快照已保存");
    }

    /// <summary>
    /// 清理追踪数据
    /// </summary>
    [ContextMenu("Clear Tracking Data")]
    public void ClearTrackingData()
    {
        snapshots.Clear();
        trackedObjects.Clear();
        previousCounts.Clear();
        Debug.Log("[MemoryTracker] 追踪数据已清理");
    }

    void OnDestroy()
    {
        CancelInvoke();
    }
}