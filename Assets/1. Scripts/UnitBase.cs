// 모든 유닛(플레이어, 적)의 기본 스탯과 데미지/죽음 처리를 담당하는 베이스 클래스

using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public int hp = 3;
    public int maxHp = 3;
    public int score = 100;
    public int money = 20;

    // 시작 시 체력을 최대값으로 초기화
    public virtual void Awake()
    {
        hp = maxHp;
    }

    // 데미지를 받아 체력 감소 및 사망 처리
    public virtual void TakeDamage(int d)
    {
        hp = Mathf.Max(0, hp - d);
        if (hp <= 0) Die();
    }

    // 사망 시 점수/돈 지급 + 아이템 드랍 + 오브젝트 삭제
    public virtual void Die()
    {
        GameManager.I.score += score;
        GameManager.I.money += money;
        GameManager.I.ItemDrop(transform.position);
        Destroy(gameObject);
    }

    // 현재 체력 비율 반환 (HP바용)
    public float GetHpRatio()
    {
        if (maxHp <= 0) return 0f;
        return Mathf.Clamp01((float)hp / maxHp);
    }
}