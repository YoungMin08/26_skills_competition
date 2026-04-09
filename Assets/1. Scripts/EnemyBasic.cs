// 기본 적 행동 스크립트
// 이동, 공격 패턴, 스턴 상태, 영역 진입 여부 등을 처리

using UnityEngine;

public class EnemyBasic : UnitShooterBase
{
    public Pattern pattern = Pattern.P1;
    public float speed = 2f;

    public Collider2D area;
    public Rigidbody2D rb;
    public float stunTimer;

    // 시작 시 Rigidbody 및 영역 설정
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        if (area == null) area = GameObject.FindGameObjectWithTag("Area").GetComponent<Collider2D>();
    }

    // 상태에 따라 이동/공격/스턴 처리
    private void Update()
    {
        if (GameManager.I.state != GameManager.State.Play) return;
        stunTimer -= Time.deltaTime;

        if (stunTimer > 0f)
        {
            shotT = shotDelay;
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 8f * Time.deltaTime);
            return;
        }
        ShotUpdate();
        if (StageManager.I.isBoss || IsArea())
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Move();
    }

    // 현재 사용할 공격 패턴 반환
    public override Pattern GetPattern()
    {
        return pattern;
    }

    // 목표 위치 방향으로 이동
    public void Move()
    {
        float y = GetTargetY() - rb.position.y;
        rb.velocity = new Vector2(0f, Mathf.Sign(y) * speed);
    }

    // 이동 목표 위치 계산 (위/아래 영역 기준)
    public float GetTargetY()
    {
        if (area == null) return rb.position.y;
        Bounds b = area.bounds;
        return rb.position.y >= 0f ? b.max.y - 0.8f : b.min.y + 0.8f;
    }

    // 현재 위치가 영역 내부인지 체크
    public bool IsArea()
    {
        return area.bounds.Contains(rb.position);
    }

    // 플레이어 대시 등에 밀려나며 스턴 상태 진입
    public void Shove(Vector2 dir)
    {
        stunTimer = Random.Range(1, 5);
        rb.AddForce(dir.normalized * 100f, ForceMode2D.Impulse);
    }
}