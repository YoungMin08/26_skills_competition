// 이펙트 생성 관리 스크립트
// 프리팹을 생성하고 일정 시간 후 자동 삭제 처리

using UnityEngine;

public class FxManager : MonoBehaviour
{
    public static FxManager I;

    // 싱글톤 설정 + 씬 유지
    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // 이펙트 생성 후 일정 시간 뒤 자동 제거
    public GameObject Spawn(GameObject prefab, Vector3 pos, float life = 1f)
    {
        if (prefab == null) return null;

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        Destroy(obj, life);
        return obj;
    }
}