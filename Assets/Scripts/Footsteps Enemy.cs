using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    public AudioSource src;
    public AudioClip[] clips;
    public float rate = 0.7f;
    public float range = 15f;

    private float timer;
    private Vector3 lastPos;
    private Transform player;

    private static int active;
    private static readonly int limit = 6;

    void Start()
    {
        lastPos = transform.position;
        player = Camera.main?.transform;

        if (src)
        {
            src.spatialBlend = 1f;
            src.minDistance = 1.5f;
            src.maxDistance = range;
            src.rolloffMode = AudioRolloffMode.Logarithmic;
            src.volume = 0.3f;
        }
    }

    void Update()
    {
        float speed = (transform.position - lastPos).magnitude / Time.deltaTime;
        lastPos = transform.position;

        if (speed < 0.1f)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= rate)
        {
            timer = 0f;
            Play();
        }
    }

    void Play()
    {
        if (src == null || clips == null || clips.Length == 0) return;
        if (src.isPlaying) return;

        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > range) return;
            src.volume = Mathf.Lerp(0.5f, 0.1f, dist / range);
        }

        if (active >= limit) return;

        active++;
        src.clip = clips[Random.Range(0, clips.Length)];
        src.Play();
        StartCoroutine(Release());
    }

    private System.Collections.IEnumerator Release()
    {
        yield return new WaitForSeconds(src.clip.length + 0.05f);
        active--;
    }
}