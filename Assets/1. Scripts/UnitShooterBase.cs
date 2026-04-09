// 적/보스의 탄 발사 패턴과 조준, 발사 로직을 담당하는 슈터 베이스 클래스

using UnityEngine;

public class UnitShooterBase : UnitBase
{
    public enum Pattern { P1, P2, P3, P4, P5 }

    public GameObject prefab;
    public Transform firePos;
    public Transform target;
    public float shotDelay = 3f;
    public float shotT;

    public float turnSpeed = 240f;
    public float turnDelay = 3f;

    public float p1Speed = 1f;
    public float p2Speed = 1f;
    public float p3Speed = 1.7f;
    public float p4Speed = 1.3f;
    public float p5Speed = 0.7f;

    // 시작 시 타겟 설정 + 발사 딜레이 초기화
    public virtual void Start()
    {
        if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;
        shotT = shotDelay;
    }

    // 조준 + 쿨타임 감소 후 패턴에 따라 발사
    public void ShotUpdate()
    {
        Look();
        shotT -= Time.deltaTime;

        if (shotT > 0f) return;

        Fire(GetPattern());
        shotT = shotDelay;
    }

    // 플레이어 방향으로 회전
    public void Look()
    {
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, z);
    }

    // 현재 사용할 패턴 반환 (자식 클래스에서 override 가능)
    public virtual Pattern GetPattern()
    {
        return Pattern.P1;
    }

    // 패턴에 따라 다양한 탄 발사
    public void Fire(Pattern pattern)
    {
        Vector3 pos = firePos.position;
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;

        switch (pattern)
        {
            case Pattern.P1:
                Shot(pos, dir, Missile.Type.Straight, p1Speed, 0);
                break;
            case Pattern.P2:
                Shot(pos, Rot(dir, -12), Missile.Type.Spread, p2Speed, 0);
                Shot(pos, dir, Missile.Type.Spread, p2Speed, 0);
                Shot(pos, Rot(dir, 12), Missile.Type.Spread, p2Speed, 0);
                break;
            case Pattern.P3:
                Shot(pos, dir, Missile.Type.Fast, p3Speed, 3);
                break;
            case Pattern.P4:
                Shot(pos, Rot(dir, -24), Missile.Type.Homing, p4Speed, 0);
                Shot(pos, Rot(dir, -8), Missile.Type.Straight, p4Speed, 0);
                Shot(pos, Rot(dir, 8), Missile.Type.Straight, p4Speed, 0);
                Shot(pos, Rot(dir, 24), Missile.Type.Homing, p4Speed, 0);
                break;
            case Pattern.P5:
                Shot(pos, dir, Missile.Type.Turn, p5Speed, 0);
                break;
        }
    }

    // 실제 미사일 생성 및 초기 설정
    public void Shot(Vector3 p, Vector2 d, Missile.Type t, float sm, int bc)
    {
        Missile ms = Instantiate(prefab, p, Quaternion.identity).GetComponent<Missile>();
        ms.Look(d);
        ms.Setup(t, sm, bc, turnSpeed, turnDelay);
    }

    // 방향 벡터를 특정 각도로 회전
    public Vector2 Rot(Vector2 dir, float angle)
    {
        return (Vector2)(Quaternion.Euler(0f, 0f, angle) * dir);
    }
}