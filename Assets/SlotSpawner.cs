using UnityEngine;

public class SlotSpawner : MonoBehaviour
{
    public GameObject slotPrefab; // Slotのプレハブ
    public int qubits = 2;        // 行（量子ビット数）
    public int steps = 8;         // 列（回路のステップ数）
    public float spacingX = 70f;  // 横の間隔
    public float spacingY = 70f;  // 縦の間隔

    void Start()
    {
        for (int q = 0; q < qubits; q++)
        {
            for (int s = 0; s < steps; s++)
            {
                // Slot生成
                GameObject slot = Instantiate(slotPrefab, transform);
                
                // RectTransformで位置を決める
                RectTransform rt = slot.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(s * spacingX, -q * spacingY);
            }
        }
    }
}
