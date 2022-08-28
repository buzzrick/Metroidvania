using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// turns on / off camera rotation based of CinemaMachine based on whether the right mouse button is held down.
/// </summary>
public class CameraRotateToggle : MonoBehaviour
{
    private CinemachineFreeLook _cameraBrain;
    public int MouseButton = 1;

    private bool _isButtonDown;

    private string _inputXAxis;
    private string _inputYAxis;

    void Awake()
    {
        _cameraBrain = GetComponent<CinemachineFreeLook>();
        _inputXAxis = _cameraBrain.m_XAxis.m_InputAxisName;
        _inputYAxis = _cameraBrain.m_YAxis.m_InputAxisName;
    }

    // Update is called once per frame
    void Update()
    {
        bool isButtonDown = Input.GetMouseButton(MouseButton);
        if (_isButtonDown != isButtonDown)
        {
            _isButtonDown = isButtonDown;
            _cameraBrain.m_XAxis.m_InputAxisName = _isButtonDown ? _inputXAxis : "";
            _cameraBrain.m_YAxis.m_InputAxisName = _isButtonDown ? _inputYAxis : "";
        }

        _cameraBrain.enabled = isButtonDown;
    }
}
