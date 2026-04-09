// HP 바 UI 업데이트 스크립트
// 타겟의 체력 비율에 따라 바의 길이와 위치를 조정

using UnityEngine;

public class HpBarView : MonoBehaviour
{
    public UnitBase target;
    public Transform fill;

    public Vector3 bScale;
    public Vector3 bPos;

    // 초기 스케일과 위치 저장
    private void Awake()
    {
        bScale = fill.localScale;
        bPos = fill.localPosition;
    }

    // 체력 비율에 따라 바 크기 및 위치 업데이트
    private void Update()
    {
        float ratio = target.GetHpRatio();

        Vector3 scale = bScale;
        scale.x = bScale.x * ratio;
        fill.localPosition = scale;

        Vector3 pos = bPos;
        pos.x = bPos.x - (bScale.x - scale.x) * 0.5f;
        fill.localPosition = pos;
    }
}