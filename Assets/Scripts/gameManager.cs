using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private Transform spawnPoint;
    
    private void Start() 
    {
        GameObject player = PhotonNetwork.Instantiate(
            spawnPrefab.name, 
            spawnPoint.position, 
            Quaternion.identity
        );
        
        // Принудительная инициализация для локального игрока
        if (player.GetComponent<PhotonView>().IsMine)
        {
            player.GetComponent<FirstPersonController>().enabled = true;
        }
    }
}
