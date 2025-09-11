using UnityEngine;

public class QuantumGateDetector : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private string gateSlotName = "GateSlot"; // スロットの名前
    [SerializeField] private string[] quantumGateNames = {"Gate_X", "Gate_U"}; // 対応する量子ゲートの名前
    [SerializeField] private bool showDebugInfo = true; // デバッグ情報の表示
    
    private DropSlot gateSlot;
    private string lastQuantumGateType = "";
    
    void Start()
    {
        // GateSlotを自動で見つける
        GameObject slotObject = GameObject.Find(gateSlotName);
        if (slotObject != null)
        {
            gateSlot = slotObject.GetComponent<DropSlot>();
        }
        
        if (gateSlot == null)
        {
            Debug.LogError($"GateSlot '{gateSlotName}' が見つかりません！");
        }
        else
        {
            Debug.Log("QuantumGateDetector初期化完了");
        }
    }
    
    void Update()
    {
        // フレームごとに量子ゲートの状態をチェック
        CheckQuantumGateStatus();
    }
    
    private void CheckQuantumGateStatus()
    {
        if (gateSlot == null) return;
        
        string currentGateType = GetQuantumGateType();
        
        // 状態変更があった場合のみログ出力
        if (currentGateType != lastQuantumGateType)
        {
            lastQuantumGateType = currentGateType;
            
            if (showDebugInfo)
            {
                if (currentGateType == "Gate_X")
                {
                    Debug.Log("🔬 Gate_X検出: コインは表で止まります");
                }
                else if (currentGateType == "Gate_U")
                {
                    Debug.Log("🎲 Gate_U検出: コインは50%の確率で表で止まります");
                }
                else
                {
                    Debug.Log("📀 量子ゲート未検出: コインは裏で止まります");
                }
            }
        }
    }
    
    private string GetQuantumGateType()
    {
        if (gateSlot == null || !gateSlot.HasItem()) return "";
        
        // スロット内のアイテムを検索
        Transform slotTransform = gateSlot.transform;
        
        for (int i = 0; i < slotTransform.childCount; i++)
        {
            GameObject child = slotTransform.GetChild(i).gameObject;
            
            // DropArea は除外
            if (child.name == "DropArea") continue;
            
            // 各量子ゲートをチェック
            foreach (string gateName in quantumGateNames)
            {
                if (child.name.Contains(gateName))
                {
                    if (showDebugInfo)
                    {
                        Debug.Log($"量子ゲート発見: {child.name} (タイプ: {gateName})");
                    }
                    return gateName;
                }
            }
        }
        
        return "";
    }
    
    // 外部から量子ゲートの状態を取得（CoinRotatorから呼ばれる）
    public bool IsQuantumGateActive()
    {
        string gateType = GetQuantumGateType();
        return !string.IsNullOrEmpty(gateType);
    }
    
    // 量子ゲートのタイプを取得
    public string GetActiveGateType()
    {
        return GetQuantumGateType();
    }
}
