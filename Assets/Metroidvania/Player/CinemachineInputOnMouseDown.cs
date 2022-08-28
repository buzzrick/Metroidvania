using UnityEngine;
using Cinemachine;

public class CinemachineInputOnMouseDown : MonoBehaviour, AxisState.IInputAxisProvider
{
    public string HorizontalInput = "Mouse X";
    public string VerticalInput = "Mouse Y";
    public int MouseButton = 1;

    public float GetAxisValue(int axis)
    {
        // No input unless right mouse is down
        if (!Input.GetMouseButton(MouseButton))
            return 0;

        switch (axis)
        {
            case 0: return Input.GetAxis(HorizontalInput);
            case 1: return Input.GetAxis(VerticalInput);
            default: return 0;
        }
    }
}