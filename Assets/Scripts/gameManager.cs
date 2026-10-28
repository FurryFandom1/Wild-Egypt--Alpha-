using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class gameManager : MonoBehaviourPunCallbacks
{
    public GameObject spawnPrefab;
    public Transform spawnPoint;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom("Room1", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        if (spawnPrefab != null && spawnPoint != null)
        {
            PhotonNetwork.Instantiate(spawnPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
    }
}