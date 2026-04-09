// 스테이지 진행 관리 스크립트
// 웨이브 생성 → 시간 관리 → 보스 소환 → 클리어 처리까지 담당

using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager I;

    [System.Serializable]
    public class Stage
    {
        public float time = 60f;
        public GameObject[] waves;
        public GameObject boss;
    }
    public Stage[] stages;

    public Transform wavePos;
    public Transform bossPos;

    public float leftTime;
    public int waveIndex;
    public bool isBoss;

    public Stage curStage;
    public float curTime;
    public float nextTime;
    public GameObject curBoss;

    // 싱글톤 설정
    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    // 시작 시 스테이지 시작
    private void Start()
    {
        StartStage();
    }

    // 상태에 따라 웨이브 또는 보스 업데이트
    private void Update()
    {
        if (GameManager.I.state != GameManager.State.Play) return;
        if (isBoss) UpdateBoss();
        else UpdateWave();
    }

    // 스테이지 초기화 및 첫 웨이브 생성
    public void StartStage()
    {
        enabled = true;

        curStage = stages[GameManager.I.stage - 1];
        leftTime = curStage.time;
        waveIndex = 0;
        isBoss = false;
        curTime = 0f;
        nextTime = 0f;
        curBoss = null;

        Clear(wavePos);
        Clear(bossPos);

        if (curStage.waves.Length > 0)
        {
            SpawnWave();
            nextTime = GetWaveTime();
        }
    }

    // 웨이브 간격 계산
    public float GetWaveTime()
    {
        if (curStage.waves.Length == 0) return curStage.time;
        return curStage.time / curStage.waves.Length;
    }

    // 자식 오브젝트 전부 삭제
    public void Clear(Transform p)
    {
        for (int i = p.childCount - 1; i >= 0; i--) Destroy(p.GetChild(i).gameObject);
    }

    // 웨이브 진행 및 시간 감소 처리
    public void UpdateWave()
    {
        curTime += Time.deltaTime;
        leftTime = curStage.time - curTime;

        if (waveIndex < curStage.waves.Length && curTime >= nextTime)
        {
            SpawnWave();
            nextTime += GetWaveTime();
        }
        if (curTime >= curStage.time)
        {
            SpawnBoss();
        }
    }

    // 보스 상태 유지 및 클리어 체크
    public void UpdateBoss()
    {
        leftTime = 0f;
        if (curBoss == null) ClearStage();
    }

    // 다음 웨이브 생성
    public void SpawnWave()
    {
        Instantiate(curStage.waves[waveIndex++], wavePos.position, Quaternion.identity, wavePos);
    }

    // 보스 생성
    public void SpawnBoss()
    {
        if (isBoss) return;
        isBoss = true;
        curBoss = Instantiate(curStage.boss, bossPos.position, Quaternion.identity, bossPos);
    }

    // 스테이지 종료 및 GameManager로 전달
    public void ClearStage()
    {
        enabled = false;
        isBoss = false;
        GameManager.I.ClearStage();
    }
}