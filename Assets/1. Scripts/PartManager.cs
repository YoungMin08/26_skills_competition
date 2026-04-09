// 파츠(능력) 관리 스크립트
// 구매, 슬롯 장착, 사용, 쿨타임, 미사일 효과(느림/정지/반사) 처리 담당

using UnityEngine;

public class PartManager : MonoBehaviour
{
    public static PartManager I;

    public enum Type { None, Return, Slow, Stop }
    public Type slot1 = Type.None;
    public Type slot2 = Type.None;

    public bool ownReturn;
    public bool ownSlow;
    public bool ownStop;

    public int partPrice = 250;

    public float returnC = 5f;
    public float slowC = 5f;
    public float stopC = 5f;

    public float slowDelay = 3f;
    public float stopDelay = 3f;

    public float slowTimer;
    public float stopTimer;
    public float slot1Timer;
    public float slot2Timer;

    public bool wasEffect;

    // 싱글톤 설정 + 씬 유지
    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // 입력 처리 + 타이머 감소 + 미사일 효과 갱신
    private void Update()
    {
        if (GameManager.I.state != GameManager.State.Play) return;

        if (Input.GetKeyDown(KeyCode.K)) UseSlot(1);
        if (Input.GetKeyDown(KeyCode.L)) UseSlot(2);

        if (slowTimer > 0f) slowTimer -= Time.deltaTime;
        if (stopTimer > 0f) stopTimer -= Time.deltaTime;
        if (slot1Timer > 0f) slot1Timer -= Time.deltaTime;
        if (slot2Timer > 0f) slot2Timer -= Time.deltaTime;

        bool effect = slowTimer > 0f || stopTimer > 0f;
        if (wasEffect || effect) UpdateMissile();
        wasEffect = effect;
    }

    // 모든 파츠 초기화
    public void DeletePart()
    {
        slot1 = Type.None;
        slot2 = Type.None;
        ownReturn = false;
        ownSlow = false;
        ownStop = false;
        slowTimer = 0f;
        stopTimer = 0f;
        slot1Timer = 0f;
        slot2Timer = 0f;
        wasEffect = false;
    }

    // 슬롯 사용 (쿨타임 체크 후 효과 실행)
    public void UseSlot(int v)
    {
        Type type = v == 1 ? slot1 : slot2;
        float timer = v == 1 ? slot1Timer : slot2Timer;
        float cool = 0f;

        if (type == Type.None || timer > 0f) return;

        if (type == Type.Return)
        {
            Return();
            cool = returnC;
        }
        if (type == Type.Slow)
        {
            slowTimer = slowDelay;
            cool = slowC;
        }
        if (type == Type.Stop)
        {
            stopTimer = stopDelay;
            cool = stopC;
        }

        if (v == 1) slot1Timer = cool;
        else slot2Timer = cool;
    }

    // 모든 미사일 방향 반전
    public void Return()
    {
        Missile[] ms = FindObjectsOfType<Missile>();
        for (int i = 0; i < ms.Length; i++) ms[i].Look(-ms[i].dir);
    }

    // 미사일 속도 변경 (느림/정지 효과)
    public void UpdateMissile()
    {
        Missile[] ms = FindObjectsOfType<Missile>();
        float sm = 1f;
        if (stopTimer > 0f) sm = 0f;
        else if (slowTimer > 0f) sm = 0.5f;

        for (int i = 0; i < ms.Length; i++) ms[i].speedMul = sm;
    }

    // 파츠 구매 처리
    public void BuyPart(int v)
    {
        Type type = (Type)v;

        if (IsOwned(type) || GameManager.I.money < partPrice) return;
        GameManager.I.money -= partPrice;

        if (type == Type.Return) ownReturn = true;
        if (type == Type.Slow) ownSlow = true;
        if (type == Type.Stop) ownStop = true;
    }

    // 슬롯1 설정
    public void SetSlot1(int v)
    {
        slot1 = (Type)v;
    }

    // 슬롯2 설정
    public void SetSlot2(int v)
    {
        slot2 = (Type)v;
    }

    // 해당 파츠 보유 여부 확인
    public bool IsOwned(Type type)
    {
        if (type == Type.Return) return ownReturn;
        if (type == Type.Slow) return ownSlow;
        if (type == Type.Stop) return ownStop;
        return false;
    }

    // 슬롯 상태 문자열 반환 (UI용)
    public string GetSlot(int v)
    {
        Type type = v == 1 ? slot1 : slot2;
        float timer = v == 1 ? slot1Timer : slot2Timer;
        string name = "NONE";
        if (type == Type.Return) name = "RETURN";
        if (type == Type.Slow) name = "SLOW";
        if (type == Type.Stop) name = "STOP";

        if (type == Type.None) return "NONE";
        return timer > 0f ? $"{Mathf.CeilToInt(timer)}" : $"{name}";
    }
}