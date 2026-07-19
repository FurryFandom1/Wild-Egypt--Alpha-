using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class NetworkScript : MonoBehaviourPunCallbacks
{
    void Start() 
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Trying to Connect...");
    }
    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene(1);
        Debug.Log("Connected");
    }

}
