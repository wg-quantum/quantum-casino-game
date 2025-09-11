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
    private float stopThreshold = 5f;  // 誤差許容
    
    // 量子ゲート検出器への参照
    private QuantumGateDetector quantumGateDetector;
    
    // Gate_Uの確率計算用（停止要求時に一度だけ決定）
    private bool gate_U_Result = false;
    private bool gate_U_ResultCalculated = false;

    // UI Text への参照
    [SerializeField] private TextMeshProUGUI bubbleText; 

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

                // BubbleBody の文字列を変更
                if (bubbleText != null)
                {
                    if (result == "表 (Heads)")
                    {
                        bubbleText.text = "Congratulations!";
                        if (SceneManager.GetActiveScene().name != "CoinTossSceneU")
                        {
                        StartCoroutine(WaitAndLoadScene(2f, "CoinTossSceneU"));
                        }
                    }
                    else
                    {
                        bubbleText.text = "Try Again!";
                    }
                }

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
        gate_U_ResultCalculated = false; // 新しい停止要求時にリセット
    }

    public void RequestRestartOnTails()
    {
         Debug.Log("Restart requested");

        // 回転状態を初期化
        shouldStop = false;
        rotationSpeed = 360f; // 初期回転速度に戻す
        currentZRotation = 0f;
        gate_U_ResultCalculated = false;

        // BubbleTextも初期状態に戻す
        if (bubbleText != null)
        {
            bubbleText.text = "Adjust the circuit to make the coin face up!";
        }
        Time.timeScale = 1f;
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
                
            case "Gate_U":
                // Gate_Uの場合は50%の確率（停止要求時に一度だけ計算）
                if (!gate_U_ResultCalculated)
                {
                    gate_U_Result = Random.Range(0f, 1f) < 0.5f; // 50%の確率
                    gate_U_ResultCalculated = true;
                    
                    string resultText = gate_U_Result ? "表" : "裏";
                    Debug.Log($"Gate_U確率計算結果: {resultText}で停止");
                }
                
                return gate_U_Result ? 0f : 180f; // 表または裏
                
            default:
                Debug.Log("通常モード - 裏で停止");
                return 180f; // 裏
        }
    }

    // シーン遷移を待機してから実行するコルーチン
    private IEnumerator WaitAndLoadScene(float waitTime, string sceneName)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
}
