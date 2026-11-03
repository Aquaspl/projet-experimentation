using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

/// <summary>
/// Detects at startup whether a VR headset is available and activates
/// the matching player setup (VR rig or PC player).
/// 
/// IMPORTANT: In Project Settings > XR Plug-in Management, turn OFF
/// "Initialize XR on Startup" so this script controls when XR starts.
/// </summary>
public class PlayerModeSelector : MonoBehaviour
{
    [Header("Player setups (both start disabled or enabled, this script decides)")]
    public GameObject vrRig;     // e.g. the OVRCameraRig / Camera Rig building block
    public GameObject pcPlayer;  // your CharacterController + Move + camera with CameraLook

    [Header("Debug")]
    [Tooltip("Force a mode in the editor for testing, ignoring the headset.")]
    public bool forcePC = false;

    public static bool IsVR { get; private set; }

    IEnumerator Start()
    {
        if (vrRig != null) vrRig.SetActive(false);
        if (pcPlayer != null) pcPlayer.SetActive(false);

        bool vrStarted = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        // Standalone Quest build: always VR
        vrStarted = true;
#else
        if (!forcePC)
        {
            XRManagerSettings manager = XRGeneralSettings.Instance != null
                ? XRGeneralSettings.Instance.Manager
                : null;

            if (manager != null)
            {
                // Tries to start the XR runtime. Fails (activeLoader == null) if no headset/runtime is available.
                yield return manager.InitializeLoader();

                if (manager.activeLoader != null)
                {
                    manager.StartSubsystems();
                    vrStarted = true;
                }
            }
        }
#endif

        IsVR = vrStarted;
        Debug.Log(IsVR ? "Mode: VR" : "Mode: PC");

        if (vrRig != null) vrRig.SetActive(IsVR);
        if (pcPlayer != null) pcPlayer.SetActive(!IsVR);
    }

    void OnDestroy()
    {
        // Cleanly shut XR down when leaving play mode / closing the game
        if (IsVR && XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null
            && XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }
}