// UI 전체 관리 스크립트
// 텍스트 업데이트, 패널 전환, 버튼 기능(게임 시작/랭킹/상점 등) 처리 담당

using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text hpText;
    public TMP_Text bombText;
    public TMP_Text itemText;
    public TMP_Text slot1Text;
    public TMP_Text slot2Text;

    public TMP_Text stageText;
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public TMP_Text moneyText;

    public GameObject openingPanel;
    public GameObject helpPanel;
    public GameObject hudPanel;
    public GameObject shopPanel;
    public GameObject overPanel;
    public GameObject rankPanel;
    public GameObject endPanel;

    public GameObject RankPanel;
    public TMP_InputField rankInput;
    public TMP_Text rankText;

    public PlayerController p;

    // 시작 시 오프닝 패널 활성화
    private void Start()
    {
        if (GameManager.I.state != GameManager.State.Play) openingPanel.SetActive(true);
    }

    // 상태에 따른 UI 업데이트 및 텍스트 갱신
    private void Update()
    {
        GameManager gm = GameManager.I;
        if (p == null) p = FindObjectOfType<PlayerController>();
        if (Input.GetMouseButtonDown(0)) openingPanel.SetActive(false);

        hudPanel.SetActive(gm.state == GameManager.State.Play);
        shopPanel.SetActive(gm.state == GameManager.State.Shop);
        overPanel.SetActive(gm.state == GameManager.State.Over);
        rankPanel.SetActive(gm.state == GameManager.State.Rank);
        endPanel.SetActive(gm.state == GameManager.State.End);

        hpText.text = $"HP : {Mathf.CeilToInt(p.hp)} / {Mathf.CeilToInt(p.maxHp)}";
        bombText.text = $"BOMB [J] : {gm.bomb}";
        UpdateItem();
        UpdatePart();

        stageText.text = $"STAGE : {gm.stage}";
        timeText.text = StageManager.I.isBoss ? "TIME : BOSS" : $"TIME : {Mathf.CeilToInt(StageManager.I.leftTime)}";
        scoreText.text = $"SCORE : {gm.score}";
        moneyText.text = $"MONEY : {gm.money}";
    }

    // 현재 아이템 상태를 텍스트로 표시
    public void UpdateItem()
    {
        switch (p.item)
        {
            case ItemPickup.Item.Heal:
                itemText.text = "ITEM [F] : HEAL";
                break;
            case ItemPickup.Item.Invin:
                itemText.text = "ITEM [F] : INVIN";
                break;
            case ItemPickup.Item.Bomb:
                itemText.text = "ITEM [F] : BOMB";
                break;
            default:
                itemText.text = "ITEM [F] : NONE";
                break;
        }
    }

    // 파츠 슬롯 상태 표시
    public void UpdatePart()
    {
        slot1Text.text = PartManager.I.GetSlot(1);
        slot2Text.text = PartManager.I.GetSlot(2);
    }

    // 새 게임 시작 버튼
    public void NewGame() => GameManager.I.NewGame();

    // 이어하기 버튼
    public void ContinueGame() => GameManager.I.ContinueGame();

    // 게임 종료 버튼
    public void QuitGame() => GameManager.I.QuitGame();

    // 랭킹창 열기 및 데이터 표시
    public void OpenRank()
    {
        RankPanel.SetActive(true);
        rankText.text = RankingManager.I.GetRankText();
    }

    // 랭킹창 닫기
    public void CloseRank() => RankPanel.SetActive(false);

    // 도움말 열기
    public void Openhelp() => helpPanel.SetActive(true);

    // 도움말 닫기
    public void Closehelp() => helpPanel.SetActive(false);

    // 메뉴로 이동
    public void GoMenu() => GameManager.I.GoMenu();

    // 다음 스테이지 이동
    public void NextStage() => GameManager.I.NextStage();

    // 파츠 구매
    public void BuyPart(int v) => PartManager.I.BuyPart(v);

    // 슬롯1 설정
    public void Slot1(int v) => PartManager.I.SetSlot1(v);

    // 슬롯2 설정
    public void Slot2(int v) => PartManager.I.SetSlot2(v);

    // 랭킹 저장 후 엔딩 상태로 전환
    public void RankSave()
    {
        RankingManager.I.AddRank(rankInput.text, GameManager.I.score);
        GameManager.I.EndGame();
    }
}