// 콜라이더 디버그 표시 스크립트
// debug 모드일 때 콜라이더 영역을 라인으로 시각화

using UnityEngine;

public class ColliderDebugView : MonoBehaviour
{
    public Collider2D cd;
    public LineRenderer line;

    // 콜라이더와 라인렌더러 초기 설정
    private void Awake()
    {
        cd = GetComponent<Collider2D>();
        line = GetComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.loop = true;
        line.positionCount = 4;
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;
        line.enabled = false;
    }

    // debug 상태에 따라 콜라이더 영역을 선으로 그림
    private void Update()
    {
        bool show = GameManager.I.debug;
        if (!show)
        {
            line.enabled = false;
            return;
        }
        line.enabled = true;
        Bounds b = cd.bounds;

        Vector3 p1 = new Vector3(b.min.x, b.min.y, 0f);
        Vector3 p2 = new Vector3(b.min.x, b.max.y, 0f);
        Vector3 p3 = new Vector3(b.max.x, b.max.y, 0f);
        Vector3 p4 = new Vector3(b.max.x, b.min.y, 0f);

        line.SetPosition(0, p1);
        line.SetPosition(1, p2);
        line.SetPosition(2, p3);
        line.SetPosition(3, p4);
    }
}