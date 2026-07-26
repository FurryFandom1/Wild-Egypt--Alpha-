using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] clips;
    public float interval = 0.45f;

    private float timer;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool moving = (h != 0 || v != 0);

        if (!moving)
        {
            timer = 0f;
            return;
        }

        float speed = Input.GetKey(KeyCode.LeftShift) ? interval / 1.4f : interval;

        timer += Time.deltaTime;
        if (timer >= speed)
        {
            timer = 0f;
            PlayStep();
        }
    }

    void PlayStep()
    {
        if (clips == null || clips.Length == 0 || source == null) return;

        source.clip = clips[Random.Range(0, clips.Length)];
        source.Play();
    }
}
