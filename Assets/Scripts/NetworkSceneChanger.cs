using UnityEngine;
using Unity.Netcode;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneName;

    public void ChangeScene()
    {
        // Only the server/host can change the network scene
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                sceneName,
                UnityEngine.SceneManagement.LoadSceneMode.Single
            );
        }
    }
}