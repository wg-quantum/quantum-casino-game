using UnityEngine;

public class GateSlot : MonoBehaviour
{
    // ここに「置かれたゲート」を記録できるようにしておく
    public DraggableGate currentGate;

    public bool IsEmpty()
    {
        return currentGate == null;
    }

    public void SetGate(DraggableGate gate)
    {
        currentGate = gate;
    }

    public void ClearGate()
    {
        currentGate = null;
    }
}
