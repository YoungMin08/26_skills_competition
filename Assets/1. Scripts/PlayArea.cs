using UnityEngine;

public class PlayerArea : MonoBehaviour
{
    public float speed = 4f;
    public Vector3 sScale;
    public Camera cm;

    private void Awake()
    {
        sScale = transform.localScale;
        cm = Camera.main;
    }
    private void Update()
    {
        Vector3 scale = transform.localScale;
        if (StageManager.I.isBoss) scale.y = Mathf.MoveTowards(scale.y, cm.orthographicSize * 2f, speed * Time.deltaTime);
        else scale = sScale;
            transform.localScale = scale;
    }
}
