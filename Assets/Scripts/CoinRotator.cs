using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    private float currentZRotation = 0f;
    private float rotationSpeed = 360f; // 初期回転速度
    private bool shouldStop = false;
    private float deceleration = 180f; // 減速率
    private float minSpeed = 180f;      // 最低速度
    private float stopThreshold = 5f;   // 誤差許容
    
    [Header("コイン状態")]
    [SerializeField] private bool isRotating = true; // コインが回転中かどうか
    [SerializeField] private bool isStopped = false; // コインが停止中かどうか
    
    // 量子ゲート検出器への参照
    private QuantumGateDetector quantumGateDetector;
    
    // Gate_Hの確率計算用（停止要求時に一度だけ決定）
    private bool Gate_H_Result = false;
    private bool Gate_H_ResultCalculated = false;

    void Start()
    {
        // 量子ゲート検出器を自動で見つける
        quantumGateDetector = FindObjectOfType<QuantumGateDetector>();
        
        // 初期状態は回転中
        isRotating = true;
        isStopped = false;
    }

    void Update()
    {
        // 停止中の場合は何もしない
        if (isStopped) return;
        
        if (shouldStop)
        {
            // 量子ゲートの状態に応じて目標回転を決定
            float targetRotation = GetTargetRotation();
            
            rotationSpeed = Mathf.Max(minSpeed, rotationSpeed - deceleration * Time.deltaTime);

            if (Mathf.Abs(Mathf.DeltaAngle(currentZRotation, targetRotation)) < stopThreshold && rotationSpeed <= minSpeed + 1f)
            {
                // コインを停止
                transform.rotation = Quaternion.Euler(90f, 0f, targetRotation);
                rotationSpeed = 0f;
                shouldStop = false;
                isRotating = false;
                isStopped = true;
                
                string result = (targetRotation == 0f) ? "表 (Heads)" : "裏 (Tails)";
                Debug.Log($"Coin stopped on {result}.");
                return;
            }
        }

        // 回転中の場合のみ回転処理
        if (isRotating)
        {
            float deltaRotation = rotationSpeed * Time.deltaTime;
            currentZRotation += deltaRotation;
            currentZRotation %= 360f;

            transform.rotation = Quaternion.Euler(90f, 0f, currentZRotation);
        }
    }

    public void RequestStopOnTails()
    {
        if (isStopped)
        {
            // 停止中の場合：回転を再開
            StartRotation();
        }
        else
        {
            // 回転中の場合：停止を要求
            Debug.Log("Stop requested");
            shouldStop = true;
            Gate_H_ResultCalculated = false; // 新しい停止要求時にリセット
        }
    }
    
    public void StartRotation()
    {
        Debug.Log("Coin rotation started");
        
        // 回転状態をリセット
        isRotating = true;
        isStopped = false;
        shouldStop = false;
        rotationSpeed = 360f; // 初期速度に戻す
        
        // Gate_Hの結果もリセット
        Gate_H_ResultCalculated = false;
    }
    
    public void ForceStop()
    {
        Debug.Log("Coin rotation force stopped");
        
        isRotating = false;
        isStopped = true;
        shouldStop = false;
        rotationSpeed = 0f;
    }
    
    private float GetTargetRotation()
    {
        if (quantumGateDetector == null || !quantumGateDetector.IsQuantumGateActive())
        {
            Debug.Log("通常モード - 裏で停止");
            return 180f; // 裏
        }
        
        string gateType = quantumGateDetector.GetActiveGateType();
        
        switch (gateType)
        {
            case "Gate_X":
                Debug.Log("Gate_X有効 - 表で停止");
                return 0f; // 表
                
            case "Gate_H":
                // Gate_Hの場合は50%の確率（停止要求時に一度だけ計算）
                if (!Gate_H_ResultCalculated)
                {
                    Gate_H_Result = Random.Range(0f, 1f) < 0.5f; // 50%の確率
                    Gate_H_ResultCalculated = true;
                    
                    string resultText = Gate_H_Result ? "表" : "裏";
                    Debug.Log($"Gate_H確率計算結果: {resultText}で停止");
                }
                
                return Gate_H_Result ? 0f : 180f; // 表または裏
                
            default:
                Debug.Log("通常モード - 裏で停止");
                return 180f; // 裏
        }
    }
    
    // 外部から状態を取得するためのプロパティ
    public bool IsRotating => isRotating;
    public bool IsStopped => isStopped;
}
