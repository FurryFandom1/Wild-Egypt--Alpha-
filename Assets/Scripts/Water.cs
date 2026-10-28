using UnityEngine;

public class WaterZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        FirstPersonController controller = other.GetComponentInParent<FirstPersonController>();

        switch (true)
        {
            case bool _ when controller != null:
                controller.SetWaterState(true);
                Debug.Log("Игрок зашел в воду. Замедляем!");
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        FirstPersonController controller = other.GetComponentInParent<FirstPersonController>();

        switch (true)
        {
            case bool _ when controller != null:
                controller.SetWaterState(false);
                Debug.Log("Игрок вышел из воды. Ускоряем!");
                break;
        }
    }
}