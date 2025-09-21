using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CoinRotator : MonoBehaviour
{
    private float currentZRotation = 0f;
    private float rotationSpeed = 360f; // 初期回転速度
    private bool shouldStop = false;
    private float deceleration = 180f; // 減速率
    private float minSpeed = 180f;      // 最低速度
    private float stopThreshold = 5f;   // 誤差許容
    
    [Header("コイン設定")]
    [SerializeField] private float rotationTime = 1f; // 回転時間（秒）
    [SerializeField] private bool useTimeScale = true; // Time.timeScaleを使用するかどうか
    
    [Header("コイン状態")]
    [SerializeField] private bool isRotating = false; // コインが回転中かどうか
    [SerializeField] private bool isStopped = true; // コインが停止中かどうか（初期は停止）
    
    // 量子ゲート検出器への参照
    private QuantumGateDetector quantumGateDetector;
    
    // Gate_Hの確率計算用（停止要求時に一度だけ決定）
    private bool gateRandomResult = false;
    private bool gateRandomResultCalculated = false;
    
    // 回転時間管理用
    private float rotationTimer = 0f;

    // UI Text への参照
    [SerializeField] private TextMeshProUGUI bubbleText;

    void Start()
    {
        // 量子ゲート検出器を自動で見つける
        quantumGateDetector = FindObjectOfType<QuantumGateDetector>();
        
        // 初期状態：表（0度）で静止
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        currentZRotation = 0f;
        isRotating = false;
        isStopped = true;
        
        // 初期BubbleText
        if (bubbleText != null)
        {
            bubbleText.text = "Adjust the circuit to make the coin face up!";
        }
        
        Debug.Log($"コイン初期化：表で静止 (Scene: {SceneManager.GetActiveScene().name})");
    }

    void Update()
    {
        // 停止中の場合は何もしない
        if (isStopped) return;
        
        if (isRotating)
        {
            // 回転時間を管理
            rotationTimer += Time.deltaTime;
            
            // 指定時間が経過したら停止処理を開始
            if (rotationTimer >= rotationTime && !shouldStop)
            {
                shouldStop = true;
                Debug.Log("回転時間終了 - 停止処理開始");
            }
            
            // 停止処理
            if (shouldStop)
            {
                // 量子ゲートの状態に応じて目標回転を決定
                float targetRotation = GetTargetRotation();
                
                rotationSpeed = Mathf.Max(minSpeed, rotationSpeed - deceleration * Time.deltaTime);

                if (Mathf.Abs(Mathf.DeltaAngle(currentZRotation, targetRotation)) < stopThreshold && rotationSpeed <= minSpeed + 1f)
                {
                    // コインを停止
                    transform.rotation = Quaternion.Euler(90f, 0f, targetRotation);
                    StopCoin(targetRotation);
                    return;
                }
            }
            
            // 回転処理
            float deltaRotation = rotationSpeed * Time.deltaTime;
            currentZRotation += deltaRotation;
            currentZRotation %= 360f;

            transform.rotation = Quaternion.Euler(90f, 0f, currentZRotation);
        }
    }

    // Button用のpublicメソッド（CoinTossButtonから直接呼ばれる）
    public void OnCoinTossButtonPressed()
    {
        Debug.Log("OnCoinTossButtonPressed called");
        
        if (isStopped)
        {
            StartRotation();
        }
        else
        {
            Debug.Log("既に回転中です");
        }
    }
    
    // 旧メソッドとの互換性のため（既存のButton設定がある場合）
    public void RequestStopOnTails()
    {
        OnCoinTossButtonPressed();
    }

    // 旧メソッドとの互換性のため
    public void RequestRestartOnTails()
    {
        OnCoinTossButtonPressed();
    }
    
    public void StartRotation()
    {
        Debug.Log("コイン回転開始");
        
        // 回転状態をリセット
        isRotating = true;
        isStopped = false;
        shouldStop = false;
        rotationSpeed = 360f; // 初期速度に戻す
        rotationTimer = 0f;   // タイマーリセット
        
        // Gate_Hの結果もリセット
        gateRandomResultCalculated = false;
        
        // BubbleTextをリセット
        if (bubbleText != null)
        {
            bubbleText.text = "Coin is rotating...";
        }
        
        // Time.timeScaleを確実に1にする
        Time.timeScale = 1f;
    }
    
    private void StopCoin(float targetRotation)
    {
        // コインを停止
        rotationSpeed = 0f;
        shouldStop = false;
        isRotating = false;
        isStopped = true;
        rotationTimer = 0f;
        
        string result = (targetRotation == 0f) ? "表 (Heads)" : "裏 (Tails)";
        Debug.Log($"Coin stopped on {result}.");

        // Time.timeScaleを使用する場合
        if (useTimeScale)
        {
            Time.timeScale = 0f;
            Debug.Log("Game paused.");
        }

        // BubbleTextの更新とシーン遷移処理
        UpdateBubbleTextAndScene(result);
    }
    
    public void ForceStop()
    {
        Debug.Log("Coin rotation force stopped");
        
        isRotating = false;
        isStopped = true;
        shouldStop = false;
        rotationSpeed = 0f;
        rotationTimer = 0f;
    }
    
    private float GetTargetRotation()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        // シーン別の処理
        switch (currentScene)
        {
            case "CoinTossSceneX":
                return GetTargetRotationForSceneX();
                
            case "CoinTossSceneH":
                return GetTargetRotationForSceneH();
                
            default:
                // デフォルトの処理（従来の処理）
                return GetTargetRotationDefault();
        }
    }
    
    private float GetTargetRotationForSceneX()
    {
        // CoinTossSceneX: 最初は裏、Gate_Xで表
        if (quantumGateDetector == null || !quantumGateDetector.IsQuantumGateActive())
        {
            Debug.Log("CoinTossSceneX - 通常モード: 裏で停止");
            return 180f; // 裏
        }
        
        string gateType = quantumGateDetector.GetActiveGateType();
        
        if (gateType == "Gate_X")
        {
            Debug.Log("CoinTossSceneX - Gate_X有効: 表で停止");
            return 0f; // 表
        }
        else
        {
            // その他の場合：50%の確率
            Debug.Log("CoinTossSceneH - その他: 50%の確率");
            return GetRandomResult();
        }
    }
    
    private float GetTargetRotationForSceneH()
    {
        // CoinTossSceneH: 複数スロット対応の複雑なロジック
        if (quantumGateDetector == null)
        {
            Debug.Log("CoinTossSceneH - QuantumGateDetector未検出: 50%の確率");
            return GetRandomResult();
        }
        
        var gateInfo = quantumGateDetector.GetAllGateInfo();
        
        // ゲートが配置されていない場合：50%の確率
        if (gateInfo.Count == 0)
        {
            Debug.Log("CoinTossSceneH - ゲートなし: 50%の確率");
            return GetRandomResult();
        }
        
        // ゲートの組み合わせによる処理
        bool hasGateX = gateInfo.ContainsKey("Gate_X") && gateInfo["Gate_X"] > 0;
        bool hasGateH = gateInfo.ContainsKey("Gate_H") && gateInfo["Gate_H"] > 0;
        
        if (hasGateX && hasGateH)
        {
            // Gate_X と Gate_H が両方配置されている場合：表
            Debug.Log("CoinTossSceneH - Gate_X + Gate_H: 表で停止");
            return 0f; // 表
        }
        else if (hasGateX && !hasGateH)
        {
            // Gate_X のみ：変化なし（50%の確率）
            Debug.Log("CoinTossSceneH - Gate_Xのみ: 50%の確率");
            return GetRandomResult();
        }
        else if (!hasGateX && hasGateH)
        {
            // Gate_H のみ：裏
            Debug.Log("CoinTossSceneH - Gate_Hのみ: 裏で停止");
            return 180f; // 裏
        }
        else
        {
            // その他の場合：50%の確率
            Debug.Log("CoinTossSceneH - その他: 50%の確率");
            return GetRandomResult();
        }
    }
    
    private float GetTargetRotationDefault()
    {
        // デフォルトの処理（従来通り）
        if (quantumGateDetector == null || !quantumGateDetector.IsQuantumGateActive())
        {
            Debug.Log("Default - 通常モード: 裏で停止");
            return 180f; // 裏
        }
        
        string gateType = quantumGateDetector.GetActiveGateType();
        
        switch (gateType)
        {
            case "Gate_X":
                Debug.Log("Default - Gate_X有効: 表で停止");
                return 0f; // 表
                
            case "Gate_H":
                return GetRandomResult();
                
            default:
                Debug.Log("Default - 通常モード: 裏で停止");
                return 180f; // 裏
        }
    }
    
    private float GetRandomResult()
    {
        // 50%の確率計算（停止要求時に一度だけ計算）
        if (!gateRandomResultCalculated)
        {
            gateRandomResult = Random.Range(0f, 1f) < 0.5f; // 50%の確率
            gateRandomResultCalculated = true;
            
            string resultText = gateRandomResult ? "表" : "裏";
            Debug.Log($"50%確率計算結果: {resultText}で停止");
        }
        
        return gateRandomResult ? 0f : 180f; // 表または裏
    }

    private void UpdateBubbleTextAndScene(string result)
    {
        // BubbleBodyの文字列を変更
        if (bubbleText != null)
        {
            if (result == "表 (Heads)")
            {
                bubbleText.text = "Congratulations!";
                
                // シーン遷移の処理
                string currentScene = SceneManager.GetActiveScene().name;
                string nextScene = GetNextScene(currentScene);
                
                if (!string.IsNullOrEmpty(nextScene) && currentScene != nextScene)
                {
                    StartCoroutine(WaitAndLoadScene(2f, nextScene));
                }
            }
            else
            {
                bubbleText.text = "Try Again!";
            }
        }
    }
    
    private string GetNextScene(string currentScene)
    {
        switch (currentScene)
        {
            case "CoinTossSceneX":
                return "CoinTossSceneH";
            case "CoinTossSceneH":
                return ""; // 最終シーンなので遷移しない
            default:
                return "";
        }
    }

    // シーン遷移を待機してから実行するコルーチン
    private IEnumerator WaitAndLoadScene(float waitTime, string sceneName)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
    
    // 外部から状態を取得するためのプロパティ
    public bool IsRotating => isRotating;
    public bool IsStopped => isStopped;
    
    // 使用モードを切り替えるメソッド
    public void SetUseTimeScale(bool useScale)
    {
        useTimeScale = useScale;
        Debug.Log($"TimeScale使用モード: {useTimeScale}");
    }
    
    // 回転時間を設定するメソッド
    public void SetRotationTime(float time)
    {
        rotationTime = time;
        Debug.Log($"回転時間設定: {rotationTime}秒");
    }
    
    // デバッグ用：現在の状態を表示
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void LogCurrentState()
    {
        Debug.Log($"=== Coin State ===");
        Debug.Log($"Scene: {SceneManager.GetActiveScene().name}");
        Debug.Log($"Is Rotating: {isRotating}");
        Debug.Log($"Is Stopped: {isStopped}");
        Debug.Log($"Should Stop: {shouldStop}");
        Debug.Log($"Rotation Timer: {rotationTimer:F2}s");
        Debug.Log($"Current Z Rotation: {currentZRotation:F1}°");
        Debug.Log($"=================");
    }
}
