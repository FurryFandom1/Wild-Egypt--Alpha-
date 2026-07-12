using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LoadScene();
        }
        
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(2);
    }
}



