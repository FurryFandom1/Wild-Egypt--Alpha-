using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private FirstPersonController fpsController;
    [SerializeField] private Camera playerCamera;

    void Start() 
{
    if (photonView.IsMine) 
    {
        // Активируем камеру только для локального игрока
        if (playerCamera != null) 
        {
            playerCamera.gameObject.SetActive(true);
            playerCamera.enabled = true;
        }
        else
        {
            Debug.LogError("Player camera is missing!");
        }
        
        // Включаем FPS контроллер
        if (fpsController != null) 
        {
            fpsController.enabled = true;
        }
        
        // Настройки курсора
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    else 
    {
        // Для удалённых игроков отключаем всё
        if (playerCamera != null) 
        {
            playerCamera.gameObject.SetActive(false);
        }
        if (fpsController != null) 
        {
            fpsController.enabled = false;
        }
    }
}
}