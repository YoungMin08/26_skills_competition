// 스테이지3 화면 효과 스크립트
// 플레이 상태 + 스테이지3일 때 비네트 효과 활성화

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Stage3Vignette : MonoBehaviour
{
    public Volume volume;
    public Vignette vignette;

    // Volume에서 Vignette 컴포넌트 가져오기
    private void Awake()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
    }

    // 스테이지3 플레이 중일 때만 비네트 활성화
    private void Update()
    {
        vignette.active = GameManager.I.state == GameManager.State.Play && GameManager.I.stage == 3;
    }
}