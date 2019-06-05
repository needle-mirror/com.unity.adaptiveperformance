using UnityEngine;

public class Move : MonoBehaviour
{
    Vector3 pos;
    public float speed = 10.0f;
    float min = -15f;
    float max = 15f;
    private float waitTime = 0;
    private float timer = 0;
    private float addedSpeed = 0;
    void Start()
    {
        pos = transform.position;
        waitTime = 1 / speed;
    }

    void Update()
    {
        addedSpeed = Mathf.Abs(Mathf.Sin(Time.time * 1.5f) * (max - min));
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = waitTime;
            var z = (Mathf.PingPong((addedSpeed), max - min) + min);
            transform.position = new Vector3(pos.x, pos.y, z);
        }
    }
}
