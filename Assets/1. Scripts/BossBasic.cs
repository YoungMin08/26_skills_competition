// 보스 전용 공격 패턴 관리 스크립트
// 패턴 리스트를 순서대로 반복 실행하며, 대시로만 데미지를 받도록 처리

using System.Collections.Generic;
using UnityEngine;

public class BossBasic : UnitShooterBase
{
    [System.Serializable]
    public class Step
    {
        public Pattern pattern = Pattern.P1;
    }
    public List<Step> steps = new List<Step>();
    public int index = 0;

    // 매 프레임 공격 패턴 실행
    private void Update()
    {
        if (GameManager.I.state != GameManager.State.Play) return;
        ShotUpdate();
    }

    // 현재 사용할 패턴 반환 (순차 반복)
    public override Pattern GetPattern()
    {
        return NextPattern();
    }

    // 다음 패턴을 순서대로 가져오고 인덱스 순환
    public Pattern NextPattern()
    {
        if (steps == null || steps.Count <= 0) return Pattern.P1;
        Step step = steps[index];
        index = (index + 1) % steps.Count;
        return step.pattern;
    }

    // 일반 공격 데미지 무시 (총알 등 무효)
    public override void TakeDamage(int d)
    {
    }

    // 대시 공격으로만 데미지 처리
    public void TakeDashDamage(int d)
    {
        hp = Mathf.Max(0, hp - d);
        if (hp <= 0) Die();
    }
}