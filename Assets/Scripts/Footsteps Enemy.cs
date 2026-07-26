using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    public AudioSource src;
    public AudioClip[] clips;
    public float stepRate = 0.5f;

    private float timer;
    private Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        float speed = (transform.position - lastPos).magnitude / Time.deltaTime;
        lastPos = transform.position;

        bool moving = speed > 0.1f;

        if (!moving)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= stepRate)
        {
            timer = 0f;
            Play();
        }
    }

    void Play()
    {
        if (src == null || clips == null || clips.Length == 0) return;

        src.clip = clips[Random.Range(0, clips.Length)];
        src.Play();
    }
}