// 운석 장애물 스크립트
// 회전하면서 플레이어와 충돌 시 일정 간격으로 데미지를 줌

using UnityEngine;

public class MeteorObstacle : MonoBehaviour
{
    public float rot = 30f;
    public float hitDelay = 0.5f;
    public float hitT;

    // 회전 및 히트 쿨타임 감소
    public void Update()
    {
        if (GameManager.I.state != GameManager.State.Play) return;
        transform.Rotate(0f, 0f, rot * Time.deltaTime);
        hitT -= Time.deltaTime;
    }

    // 플레이어 접촉 시 일정 간격으로 데미지 처리
    private void OnTriggerStay2D(Collider2D c)
    {
        PlayerController p = c.GetComponent<PlayerController>();
        if (p != null)
        {
            if (hitT > 0f) return;
            p.Hit(5);
            hitT = hitDelay;
        }
    }
}