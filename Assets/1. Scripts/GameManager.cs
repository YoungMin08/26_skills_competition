// 게임 전체 흐름을 관리하는 핵심 매니저
// (싱글톤, 상태 관리, 씬 전환, 저장/로드, 스테이지 진행, 치트 기능 등 담당)

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    public enum State { Menu, Play, Shop, Over, Rank, End }
    public State state = State.Menu;

    public int stage = 1;
    public int maxStage = 3;
    public int score, money = 0;
    public int bomb = 1;

    public bool pause;
    public bool debug;
    public bool invin;

    public GameObject healP;
    public GameObject invinP;
    public GameObject bombP;

    // 싱글톤 생성 + 씬 유지 + 씬 로드 이벤트 등록
    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 매 프레임 치트키 입력 체크
    private void Update()
    {
        CheatKeys();
    }

    // 씬 이벤트 해제
    private void OnDestroy()
    {
        if (I == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드 시 시간 초기화 + 데이터 로드
    public void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        Time.timeScale = 1f;
        SaveManager.I.LoadPlayer();
        SaveManager.I.LoadPart();
    }

    // 게임 상태 변경 + 시간 정지 여부 설정
    public void SetState(State s, bool p = false)
    {
        state = s;
        Time.timeScale = p ? 0f : 1f;
    }

    // 게임 기본값 초기화
    public void ResetGame()
    {
        stage = 1;
        score = money = 0;
        bomb = 1;
    }

    // 새 게임 시작
    public void NewGame()
    {
        ResetGame();
        PartManager.I.DeletePart();
        SaveManager.I.DeleteSave();

        SetState(State.Play);
        SceneManager.LoadScene("Game");
    }

    // 이어하기 (세이브 없으면 새 게임)
    public void ContinueGame()
    {
        if (!SaveManager.I.LoadGame())
        {
            NewGame();
            return;
        }
        SetState(State.Play);
        SceneManager.LoadScene("Game");
    }

    // 게임 오버 상태 전환
    public void OverGame() => SetState(State.Over, true);

    // 엔딩 상태 전환
    public void EndGame() => SetState(State.End, true);

    // 게임 종료
    public void QuitGame() => Application.Quit();

    // 플레이어 상태 초기화
    public void ResetStage() => FindObjectOfType<PlayerController>().ResetStage();

    // 스테이지 클리어 처리
    public void ClearStage()
    {
        if (stage >= maxStage)
        {
            SetState(State.Rank, true);
            return;
        }
        stage++;
        bomb++;
        ResetStage();
        SetState(State.Shop, true);
    }

    // 다음 스테이지 시작
    public void NextStage()
    {
        SaveManager.I.Save();
        StageManager.I.StartStage();
        SetState(State.Play);
    }

    // 메뉴로 이동 (저장/삭제 처리 포함)
    public void GoMenu()
    {
        if (state == State.Shop) SaveManager.I.Save();
        else SaveManager.I.DeleteSave();

        SetState(State.Menu);
        SceneManager.LoadScene("Menu");
    }

    // 폭탄 사용 (없으면 실패)
    public bool UseBomb()
    {
        if (bomb <= 0) return false;
        bomb--;
        return true;
    }

    // 아이템 랜덤 드랍
    public void ItemDrop(Vector3 p)
    {
        if (Random.value > 0.2f) return;
        GameObject prefab = null;
        int type = Random.Range(0, 3);

        if (type == 0) prefab = healP;
        else if (type == 1) prefab = invinP;
        else prefab = bombP;

        Instantiate(prefab, p, Quaternion.identity);
    }

    // 치트키 처리
    public void CheatKeys()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            pause = !pause;
            Time.timeScale = pause ? 0f : 1f;
        }
        if (Input.GetKeyDown(KeyCode.F2)) debug = !debug;
        if (Input.GetKeyDown(KeyCode.F3)) invin = !invin;
        if (Input.GetKeyDown(KeyCode.F4)) money += 1000;
        if (Input.GetKeyDown(KeyCode.F5)) AllKill();
        if (Input.GetKeyDown(KeyCode.F6)) StageManager.I.curTime = StageManager.I.nextTime;
        if (Input.GetKeyDown(KeyCode.F11)) RankingManager.I.AddRank("test", Random.Range(1, 100000));
        if (Input.GetKeyDown(KeyCode.F12)) RankingManager.I.DeleteSave();
    }

    // 모든 적 제거
    public void AllKill()
    {
        EnemyBasic[] e = FindObjectsOfType<EnemyBasic>();
        BossBasic[] b = FindObjectsOfType<BossBasic>();
        for (int i = 0; i < e.Length; i++) e[i].TakeDamage(9999);
        for (int i = 0; i < b.Length; i++) b[i].TakeDashDamage(9999);
    }
}