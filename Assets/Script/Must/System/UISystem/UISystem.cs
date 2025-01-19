// using UnityEngine;
// using UnityEngine.Rendering.Universal;
//
// /// <summary>
// /// UI系统
// /// </summary>
// public class UISystem : SingletonMono<UISystem>
// {
//     public UnityEngine.Camera MainCamera;
//     public UnityEngine.Camera UICamera;
//
//
//     public void Init()
//     {
//         MainCamera = Camera.main;
//         UniversalAdditionalCameraData toBaseData = MainCamera.GetUniversalAdditionalCameraData();
//         if (toBaseData.renderType != CameraRenderType.Base)
//             toBaseData.renderType = CameraRenderType.Base;
//         UnityEngine.GameObject ui = new UnityEngine.GameObject("UI");
//
//         //EventSystem
//         UnityEngine.GameObject eventSystem = new UnityEngine.GameObject("EventSystem");
//         eventSystem.transform.SetParent(ui.transform);
//         eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
//         eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
//         eventSystem.AddComponent<UnityEngine.AudioListener>();
//         eventSystem.AddComponent<UnityEngine.EventSystems.BaseInput>();
//
//
//         //UICamera
//         UnityEngine.GameObject uiCamera = new UnityEngine.GameObject("UICamera");
//         uiCamera.transform.SetParent(ui.transform);
//         UICamera = eventSystem.AddComponent<UnityEngine.Camera>();
//         UniversalAdditionalCameraData toOverlayData = UICamera.GetUniversalAdditionalCameraData();
//         if (toOverlayData.renderType != CameraRenderType.Overlay)
//             toOverlayData.renderType = CameraRenderType.Overlay;
//         if (!toBaseData.cameraStack.Contains(UICamera))
//             toBaseData.cameraStack.Add(UICamera);
//
//         //Root
//         UnityEngine.GameObject root = new UnityEngine.GameObject("Root");
//     }
// }