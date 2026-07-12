using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject player;
    public float minX, minY, maxX, maxY; 
    void Start()
    {
        Vector2 randomPosition = new Vector2(Random.Range(minX, minY), Random.Range(maxX, maxY));
        PhotonNetwork.Instantiate(player.name, randomPosition, quaternion.identity);
    }
}
