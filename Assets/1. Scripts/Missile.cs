// 다양한 탄환 타입(직선, 유도, 랜덤턴 등)을 처리하는 미사일 스크립트
// 이동, 방향 변경, 벽 반사, 충돌 처리까지 담당

using UnityEngine;

public class Missile : MonoBehaviour
{
    public enum Type { Straight, Spread, Fast, Homing, Turn }
    public Type type = Type.Straight;

    public float speed = 8f;
    public float speedMul = 1f;

    public float turnSpeed = 240f;
    public float turnDelay = 3f;

    public int bounce;
    public int maxBounce;

    public Vector2 dir = Vector2.up;
    public Transform target;
    public float turnTimer;

    // 시작 시 타겟 설정 (플레이어)
    private void Start()
    {
        if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // 매 프레임 이동 + 타입별 행동 처리
    private void Update()
    {
        if (GameManager.I.state != GameManager.State.Play) return;
        if (type == Type.Homing) Home();
        if (type == Type.Turn) Turn();
        transform.position += (Vector3)(dir * speed * speedMul * Time.deltaTime);
    }

    // 미사일 초기 설정값 적용
    public void Setup(Missile.Type t, float sm, int bc, float ts, float td)
    {
        type = t;
        speedMul = sm;
        maxBounce = bc;
        turnSpeed = ts;
        turnDelay = td;
        turnTimer = turnDelay;
    }

    // 플레이어 방향으로 점점 유도
    public void Home()
    {
        Vector2 d = ((Vector2)target.position - (Vector2)transform.position).normalized;
        dir = Vector2.Lerp(dir, d, turnSpeed * Time.deltaTime).normalized;
        Look(dir);
    }

    // 일정 시간마다 랜덤 방향으로 회전
    public void Turn()
    {
        turnTimer -= Time.deltaTime;

        if (turnTimer > 0f) return;

        dir = (Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f)) * dir).normalized;
        Look(dir);
        turnTimer = turnDelay;
    }

    // 벽에 부딪히면 방향 반사 (횟수 제한 있음)
    public void Bounce(Collider2D c)
    {
        if (bounce >= maxBounce)
        {
            Destroy(gameObject);
            return;
        }
        Bounds b = c.bounds;
        Vector3 p = transform.position;

        float left = Mathf.Abs(p.x - b.min.x);
        float right = Mathf.Abs(p.x - b.max.x);
        float top = Mathf.Abs(p.y - b.max.y);
        float bottom = Mathf.Abs(p.y - b.min.y);

        float min = Mathf.Min(left, right, top, bottom);

        if (min == left || min == right) dir.x = -dir.x;
        else dir.y = -dir.y;
        Look(dir);
        bounce++;
    }

    // 방향에 맞게 회전값 적용
    public void Look(Vector2 d)
    {
        dir = d.normalized;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, z);
    }

    // 충돌 처리 (벽, 플레이어, 적 등)
    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Wall"))
        {
            if (maxBounce > 0) Bounce(c);
            else Destroy(gameObject);
            return;
        }
        if (c.TryGetComponent(out PlayerController p))
        {
            if (p.dashT > 0) return;
            p.Hit(1);
            Destroy(gameObject);
            return;
        }
        if (c.GetComponent<MeteorObstacle>() != null)
        {
            Destroy(gameObject);
            return;
        }
        EnemyBasic e = c.GetComponent<EnemyBasic>();
        if (e == null) return;
        e.TakeDamage(1);
        Destroy(gameObject);
    }
}