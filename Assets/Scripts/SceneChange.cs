using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    // ボタンから呼び出す関数
    public void ChangeSceneToGame()
    {
        SceneManager.LoadScene("CoinTossSceneX");
    }
}
