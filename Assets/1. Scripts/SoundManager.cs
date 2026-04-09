// 사운드 관리 스크립트
// 배경음(BGM)과 효과음(SFX)을 재생/정지하는 역할

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager I;

    public AudioSource bgm;
    public AudioSource sfx;

    // 싱글톤 설정 + 씬 유지
    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // 배경음 재생 (같은 클립이면 재생 안함)
    public void Bgm(AudioClip clip)
    {
        if (!clip || bgm.clip == clip) return;

        bgm.clip = clip;
        bgm.loop = true;
        bgm.Play();
    }

    // 배경음 정지
    public void StopBgm()
    {
        bgm.Stop();
    }

    // 효과음 재생 (겹쳐서 재생 가능)
    public void Sfx(AudioClip clip)
    {
        if (!clip) return;
        sfx.PlayOneShot(clip);
    }
}