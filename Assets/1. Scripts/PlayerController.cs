// 플레이어 조작 및 상태 관리 스크립트
// 이동, 회전, 대시, 아이템 사용, 피격, 충돌 처리까지 담당

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ItemPickup.Item item = ItemPickup.Item.None;

    public int hp = 100;
    public int maxHp = 100;

    public float speed;
    public float maxSpeed = 12f;
    public float turn = 240f;
    public float slow = 14f;

    public float dashSpeed = 14f;
    public float dashTime = 0.2f;
    public float dashDelay = 0.8f;
    public Vector2 dashDir;

    public float dashT;
    public float dashD;
    public float invinT;
    public float hitT;

    public Collider2D area;
    public Collider2D cd;
    public SpriteRenderer sr;
    public Color bc;

    // 컴포넌트 초기화 + 체력 설정
    private void Awake()
    {
        cd = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = sr.color;
        hp = maxHp;
    }

    // 플레이 영역 설정
    private void Start()
    {
        if (area == null) area = GameObject.FindGameObjectWithTag("Area").GetComponent<Collider2D>();
    }

    // 상태 체크 후 시간/입력/이동 처리
    private void Update()
    {
        if (GameManager.I.state != GameManager.State.Play) return;
        UpdateTime();
        UpdateInput();
        UpdateMove();
    }

    // 스테이지 시작 시 상태 초기화
    public void ResetStage()
    {
        hp = maxHp;
        speed = dashT = dashD = invinT = hitT = 0f;
        ClearMissile();
    }

    // 각종 타이머 감소 및 피격 색상 복구
    public void UpdateTime()
    {
        if (dashT > 0f) dashT -= Time.deltaTime;
        if (dashD > 0f) dashD -= Time.deltaTime;
        if (invinT > 0f) invinT -= Time.deltaTime;
        if (hitT > 0f) hitT -= Time.deltaTime;
        else sr.color = bc;
    }

    // 키 입력 처리 (이동, 회전, 대시, 폭탄, 아이템)
    public void UpdateInput()
    {
        if (Input.GetKey(KeyCode.W)) speed = maxSpeed;
        else if (Input.GetKey(KeyCode.S)) speed = Mathf.MoveTowards(speed, 0f, slow * 2 * Time.deltaTime);
        else speed = Mathf.MoveTowards(speed, 0f, slow * Time.deltaTime);

        if (Input.GetKey(KeyCode.A)) transform.Rotate(0f, 0f, turn * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) transform.Rotate(0f, 0f, -turn * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && dashD <= 0f) StartDash();
        if (Input.GetKeyDown(KeyCode.J) && GameManager.I.UseBomb()) ClearMissile();
        if (Input.GetKeyDown(KeyCode.F)) UseItem();
    }

    // 실제 이동 처리 (대시 여부 포함)
    public void UpdateMove()
    {
        float moveSpeed = dashT > 0f ? dashSpeed : speed;
        Vector2 dir = dashT > 0f ? dashDir : transform.up;
        Vector2 next = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;
        transform.position = Clamp(next);
    }

    // 플레이 영역 밖으로 못 나가게 제한
    public Vector2 Clamp(Vector2 p)
    {
        Bounds b = area.bounds;

        float x = cd.bounds.extents.x + 0.2f;
        float y = cd.bounds.extents.y + 0.2f;
        p.x = Mathf.Clamp(p.x, b.min.x + x, b.max.x - x);
        p.y = Mathf.Clamp(p.y, b.min.y + y, b.max.y - y);

        return p;
    }

    // 대시 시작
    public void StartDash()
    {
        dashT = dashTime;
        dashD = dashDelay;
        dashDir = transform.up;
    }

    // 화면 내 모든 미사일 제거
    public void ClearMissile()
    {
        Missile[] ms = FindObjectsOfType<Missile>();
        for (int i = 0; i < ms.Length; i++) Destroy(ms[i].gameObject);
    }

    // 아이템 사용 처리
    public void UseItem()
    {
        switch (item)
        {
            case ItemPickup.Item.Heal:
                hp = Mathf.Min(maxHp, hp + 25);
                break;
            case ItemPickup.Item.Invin:
                invinT += 10f;
                break;
            case ItemPickup.Item.Bomb:
                GameManager.I.bomb++;
                break;
        }
        item = ItemPickup.Item.None;
    }

    // 아이템 획득 처리 (이미 있으면 실패)
    public bool GetItem(ItemPickup.Item i)
    {
        if (item != ItemPickup.Item.None) return false;
        item = i;
        return true;
    }

    // 피격 처리 (무적 상태 체크 포함)
    public void Hit(int d)
    {
        if (GameManager.I.invin || invinT > 0f) return;
        hp = Mathf.Max(0, hp - d);
        sr.color = Color.red;
        hitT = 0.1f;
        if (hp <= 0) GameManager.I.OverGame();
    }

    // 대시 중 충돌 시 적 밀치기 또는 보스 데미지
    public void Dash(Collider2D c)
    {
        if (dashT <= 0f) return;
        EnemyBasic e = c.GetComponent<EnemyBasic>();
        BossBasic b = c.GetComponent<BossBasic>();

        if (e != null)
        {
            Vector2 dir = ((Vector2)e.transform.position - (Vector2)transform.position).normalized;
            e.Shove(dir);
            return;
        }
        if (b != null) b.TakeDashDamage(1);
    }

    // 물리 충돌 시 대시 처리
    private void OnCollisionEnter2D(Collision2D c)
    {
        Dash(c.collider);
    }

    // 트리거 충돌 시 대시 처리
    private void OnTriggerEnter2D(Collider2D c)
    {
        Dash(c);
    }
}