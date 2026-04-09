// 카메라 흔들림 효과 스크립트
// 일정 시간 동안 랜덤 위치로 카메라를 흔들어 연출 효과 제공

using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager I;

    Vector3 oriPos;
    float t;
    float power;

    // 싱글톤 설정
    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    // 흔들림 시간 동안 랜덤 위치 적용, 끝나면 원위치
    void Update()
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
            Camera.main.transform.localPosition = oriPos + (Vector3)Random.insideUnitCircle * power;
        }
        else
        {
            Camera.main.transform.localPosition = oriPos;
        }
    }

    // 흔들림 시작 (강도, 지속시간 설정)
    public void Shake(float p, float time)
    {
        oriPos = Camera.main.transform.localPosition;
        power = p;
        t = time;
    }
}