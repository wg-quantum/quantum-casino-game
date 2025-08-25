using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    private float currentZRotation = 0f;
    private float rotationSpeed = 360f; // 初期回転速度
    private bool shouldStop = false;
    private float deceleration = 180f; // 減速率
    private float minSpeed = 180f;      // 最低速度
    private float stopThreshold = 5f;  // 180度との誤差許容

    void Update()
    {
        if (shouldStop)
        {
            rotationSpeed = Mathf.Max(minSpeed, rotationSpeed - deceleration * Time.deltaTime);

            if (Mathf.Abs(Mathf.DeltaAngle(currentZRotation, 180f)) < stopThreshold && rotationSpeed <= minSpeed + 1f)
            {
                transform.rotation = Quaternion.Euler(90f, 0f, 180f);
                rotationSpeed = 0f;
                shouldStop = false;
                Time.timeScale = 0f;
                Debug.Log("Coin stopped. Game paused.");
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
}
