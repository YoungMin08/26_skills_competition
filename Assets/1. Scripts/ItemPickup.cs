// 아이템 드랍 오브젝트 처리 스크립트
// 일정 시간 후 삭제, 영역 밖이면 안쪽으로 이동, 플레이어가 먹으면 적용

using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum Item { None, Heal, Invin, Bomb }
    public Item item = Item.None;

    public float speed = 2f;
    public float lifeTime = 10f;

    public Collider2D area;

    // 시작 시 영역 설정
    private void Start()
    {
        if (area == null) area = GameObject.FindGameObjectWithTag("Area").GetComponent<Collider2D>();
    }

    // 수명 감소 + 영역 밖이면 안쪽으로 이동
    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
            return;
        }
        if (area.OverlapPoint(transform.position)) return;
        transform.position = Vector3.MoveTowards(transform.position, area.bounds.center, speed * Time.deltaTime);
    }

    // 플레이어가 먹으면 아이템 지급 후 삭제
    private void OnTriggerEnter2D(Collider2D c)
    {
        PlayerController p = c.GetComponent<PlayerController>();
        if (p.GetItem(item)) Destroy(gameObject);
    }
}