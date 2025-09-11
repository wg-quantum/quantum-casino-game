using UnityEngine;

public class QuantumGateDetector : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private string gateSlotName = "GateSlot";
    [SerializeField] private bool showDebugInfo = true;
    
    private DropSlot gateSlot;
    private bool lastQuantumGateState = false;
    private string lastGateType = "";
    
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
        CheckQuantumGateStatus();
    }
    
    private void CheckQuantumGateStatus()
    {
        if (gateSlot == null) return;
        
        string currentGateType = GetActiveGateType();
        bool isQuantumGateActive = !string.IsNullOrEmpty(currentGateType);
        
        // 状態変更があった場合のみログ出力
        if (isQuantumGateActive != lastQuantumGateState || currentGateType != lastGateType)
        {
            lastQuantumGateState = isQuantumGateActive;
            lastGateType = currentGateType;
            
            if (showDebugInfo)
            {
                if (isQuantumGateActive)
                {
                    switch (currentGateType)
                    {
                        case "Gate_X":
                            Debug.Log("🔬 Gate_X検出: コインは表で止まります");
                            break;
                        case "Gate_H":
                            Debug.Log("🎲 Gate_H検出: コインは50%の確率で表/裏");
                            break;
                        default:
                            Debug.Log($"🔬 {currentGateType}検出: 量子ゲート有効");
                            break;
                    }
                }
                else
                {
                    Debug.Log("📀 量子ゲート未検出: コインは裏で止まります");
                }
            }
        }
    }
    
    public string GetActiveGateType()
    {
        if (gateSlot == null || !gateSlot.HasItem()) return "";
        
        // スロット内のアイテムを検索
        Transform slotTransform = gateSlot.transform;
                for (int i = 0; i < slotTransform.childCount; i++)
        {
            GameObject child = slotTransform.GetChild(i).gameObject;
            
            // DropArea は除外
            if (child.name == "DropArea") continue;
            
            // ゲートの種類を判定
            if (child.name.Contains("Gate_X"))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"Gate_X発見: {child.name}");
                }
                return "Gate_X";
            }
            else if (child.name.Contains("Gate_H"))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"Gate_H発見: {child.name}");
                }
                return "Gate_H";
            }
            // 他のゲートタイプもここに追加可能
            else if (child.name.Contains("Gate_"))
            {
                // 一般的なゲート
                if (showDebugInfo)
                {
                    Debug.Log($"量子ゲート発見: {child.name}");
                }
                return child.name;
            }
        }
        
        return "";
    }
    
    // 外部から量子ゲートの状態を取得
    public bool IsQuantumGateActive()
    {
        return !string.IsNullOrEmpty(GetActiveGateType());
    }
}
