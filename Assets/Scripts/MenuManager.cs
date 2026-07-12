using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
    }
    
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(2);
    }
    
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
}
