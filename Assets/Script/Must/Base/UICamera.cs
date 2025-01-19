using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// URP情况下把over相机设置到主相机的cameraStack里面
/// </summary>
public class UICamera : MonoBehaviour
{
    private void Awake()
    {
        Camera uiCamera = GetComponent<Camera>();
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(uiCamera);
    }
}
