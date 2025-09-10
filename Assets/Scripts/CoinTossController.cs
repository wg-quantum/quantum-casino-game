using UnityEngine;
using UnityEngine.UI;

public class CoinTossController : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private CoinRotator coinRotator;
    [SerializeField] private Button coinTossButton;
    
    [Header("ボタンテキスト")]
    [SerializeField] private Text buttonText; // ボタンのテキスト（オプション）
    
    void Start()
    {
        // 自動で参照を取得
        if (coinRotator == null)
            coinRotator = FindObjectOfType<CoinRotator>();
            
        if (coinTossButton == null)
            coinTossButton = GetComponent<Button>();
            
        // ボタンのクリックイベントを設定
        if (coinTossButton != null)
        {
            coinTossButton.onClick.RemoveAllListeners(); // 既存のリスナーを削除
            coinTossButton.onClick.AddListener(OnCoinTossButtonClick);
        }
        
        // 初期ボタンテキストを設定
        UpdateButtonText();
    }
    
    void Update()
    {
        // ボタンテキストを更新
        UpdateButtonText();
    }
    
    private void OnCoinTossButtonClick()
    {
        if (coinRotator == null)
        {
            Debug.LogError("CoinRotator が見つかりません！");
            return;
        }
        
        if (coinRotator.IsStopped)
        {
            // コインが停止中の場合：回転を開始
            Debug.Log("コイン回転開始");
            coinRotator.StartRotation();
        }
        else
        {
            // コインが回転中の場合：停止を要求
            Debug.Log("コイン停止要求");
            coinRotator.RequestStopOnTails();
        }
    }
    
    private void UpdateButtonText()
    {
        if (buttonText != null && coinRotator != null)
        {
            if (coinRotator.IsStopped)
            {
                buttonText.text = "Start Coin Toss";
            }
            else
            {
                buttonText.text = "Stop Coin";
            }
        }
    }
}
