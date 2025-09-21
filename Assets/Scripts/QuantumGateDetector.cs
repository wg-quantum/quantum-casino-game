using UnityEngine;
using System.Collections.Generic;

public class QuantumGateDetector : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private bool showDebugInfo = true;
    
    private DropSlot[] gateSlots;
    private bool lastQuantumGateState = false;
    private string lastGateInfo = "";
    
    void Start()
    {
        // 全てのGateSlotを検索
        FindAllGateSlots();
        
        if (gateSlots == null || gateSlots.Length == 0)
        {
            Debug.LogError("GateSlot が見つかりません！");
        }
        else
        {
            Debug.Log($"QuantumGateDetector初期化完了 - {gateSlots.Length}個のスロット検出");
        }
    }
    
    void Update()
    {
        CheckQuantumGateStatus();
    }
    
    private void FindAllGateSlots()
    {
        // シーン内の全てのDropSlotを検索し、名前に"GateSlot"が含まれるものを取得
        DropSlot[] allSlots = FindObjectsOfType<DropSlot>();
        List<DropSlot> gateSlotList = new List<DropSlot>();
        
        foreach (DropSlot slot in allSlots)
        {
            if (slot.gameObject.name.Contains("GateSlot"))
            {
                gateSlotList.Add(slot);
                Debug.Log($"GateSlot発見: {slot.gameObject.name}");
            }
        }
        
        gateSlots = gateSlotList.ToArray();
    }
    
    private void CheckQuantumGateStatus()
    {
        if (gateSlots == null || gateSlots.Length == 0) return;
        
        string currentGateInfo = GetGateInfoString();
        bool isQuantumGateActive = IsQuantumGateActive();
        
        // 状態変更があった場合のみログ出力
        if (isQuantumGateActive != lastQuantumGateState || currentGateInfo != lastGateInfo)
        {
            lastQuantumGateState = isQuantumGateActive;
            lastGateInfo = currentGateInfo;
            
            if (showDebugInfo)
            {
                if (isQuantumGateActive)
                {
                    Debug.Log($"🔬 量子ゲート検出: {currentGateInfo}");
                }
                else
                {
                    Debug.Log("📀 量子ゲート未検出");
                }
            }
        }
    }
    
    private string GetGateInfoString()
    {
        var gateInfo = GetAllGateInfo();
        List<string> gateList = new List<string>();
        
        foreach (var kvp in gateInfo)
        {
            if (kvp.Value > 0)
            {
                gateList.Add($"{kvp.Key}×{kvp.Value}");
            }
        }
        
        return string.Join(" + ", gateList);
    }
    
    public Dictionary<string, int> GetAllGateInfo()
    {
        Dictionary<string, int> gateInfo = new Dictionary<string, int>();
        
        if (gateSlots == null) return gateInfo;
        
        foreach (DropSlot slot in gateSlots)
        {
            if (slot == null || !slot.HasItem()) continue;
            
            // スロット内のアイテムを検索
            Transform slotTransform = slot.transform;
            
            for (int i = 0; i < slotTransform.childCount; i++)
            {
                GameObject child = slotTransform.GetChild(i).gameObject;
                
                // DropArea は除外
                if (child.name == "DropArea") continue;
                
                // ゲートの種類を判定
                string gateType = GetGateTypeFromName(child.name);
                if (!string.IsNullOrEmpty(gateType))
                {
                    if (gateInfo.ContainsKey(gateType))
                    {
                        gateInfo[gateType]++;
                    }
                    else
                    {
                        gateInfo[gateType] = 1;
                    }
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"ゲート発見: {gateType} in {slot.gameObject.name}");
                    }
                }
            }
        }
        
        return gateInfo;
    }
    
    private string GetGateTypeFromName(string objectName)
    {
        if (objectName.Contains("Gate_X"))
        {
            return "Gate_X";
        }
        else if (objectName.Contains("Gate_H"))
        {
            return "Gate_H";
        }
        else if (objectName.Contains("Gate_"))
        {
            // 他のゲートタイプの場合
            return objectName;
        }
        
        return "";
    }
    
    // 旧メソッドとの互換性のため
    public string GetActiveGateType()
    {
        var gateInfo = GetAllGateInfo();
        
        // 最初に見つかったゲートを返す（旧システムとの互換性）
        foreach (var kvp in gateInfo)
        {
            if (kvp.Value > 0)
            {
                return kvp.Key;
            }
        }
        
        return "";
    }
    
    // 外部から量子ゲートの状態を取得
    public bool IsQuantumGateActive()
    {
        var gateInfo = GetAllGateInfo();
        return gateInfo.Count > 0;
    }
    
    // デバッグ用：現在のゲート状態を表示
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void LogGateStatus()
    {
        Debug.Log("=== Gate Status ===");
        Debug.Log($"Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log($"Gate Slots: {gateSlots?.Length ?? 0}");
        
        var gateInfo = GetAllGateInfo();
        foreach (var kvp in gateInfo)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}個");
        }
        Debug.Log("==================");
    }
}
