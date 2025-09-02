using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    private float currentZRotation = 0f;
    private float rotationSpeed = 360f; // 初期回転速度
    private bool shouldStop = false;
    private float deceleration = 180f; // 減速率
    private float minSpeed = 180f;      // 最低速度
    private float stopThreshold = 5f;  // 誤差許容
    
    // 量子ゲート検出器への参照
    private QuantumGateDetector quantumGateDetector;

    void Start()
    {
        // 量子ゲート検出器を自動で見つける
        quantumGateDetector = FindObjectOfType<QuantumGateDetector>();
    }

    void Update()
    {
        if (shouldStop)
        {
            // 量子ゲートの状態に応じて目標回転を決定
            float targetRotation = GetTargetRotation();
            
            rotationSpeed = Mathf.Max(minSpeed, rotationSpeed - deceleration * Time.deltaTime);

            if (Mathf.Abs(Mathf.DeltaAngle(currentZRotation, targetRotation)) < stopThreshold && rotationSpeed <= minSpeed + 1f)
            {
                transform.rotation = Quaternion.Euler(90f, 0f, targetRotation);
                rotationSpeed = 0f;
                shouldStop = false;
                Time.timeScale = 0f;
                
                string result = (targetRotation == 0f) ? "表 (Heads)" : "裏 (Tails)";
                Debug.Log($"Coin stopped on {result}. Game paused.");
                return;
            }
        }

        float deltaRotation = rotationSpeed * Time.deltaTime;
        currentZRotation += deltaRotation;
        currentZRotation %= 360f;

        transform.rotation = Quaternion.Euler(90f, 0f, currentZRotation);
    }

    public void RequestStopOnTails()
    {
        Debug.Log("Stop requested");
        shouldStop = true;
    }
    
    private float GetTargetRotation()
    {
        // 量子ゲートが有効な場合は表（0度）、そうでなければ裏（180度）
        if (quantumGateDetector != null && quantumGateDetector.IsQuantumGateActive())
        {
            Debug.Log("量子ゲート有効 - 表で停止");
            return 0f; // 表
        }
        else
        {
            Debug.Log("通常モード - 裏で停止");
            return 180f; // 裏
        }
    }
}
