// 게임 데이터 저장/로드 관리 스크립트
// JSON 파일로 스테이지, 플레이어, 파츠 상태를 저장하고 불러옴

using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager I;

    [System.Serializable]
    public class Data
    {
        public int stage;
        public int bomb;
        public int score;
        public int money;

        public int item;

        public bool ownReturn;
        public bool ownSlow;
        public bool ownStop;
        public int slot1;
        public int slot2;

    }
    public string path;
    public Data loaded;

    // 싱글톤 설정 + 저장 경로 초기화
    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
        path = Application.persistentDataPath + "/save.json";
    }

    // 현재 게임 상태를 JSON 파일로 저장
    public void Save()
    {
        Data data = new Data();

        data.stage = GameManager.I.stage;
        data.bomb = GameManager.I.bomb;
        data.score = GameManager.I.score;
        data.money = GameManager.I.money;

        data.ownReturn = PartManager.I.ownReturn;
        data.ownSlow = PartManager.I.ownSlow;
        data.ownStop = PartManager.I.ownStop;
        data.slot1 = (int)PartManager.I.slot1;
        data.slot2 = (int)PartManager.I.slot2;

        PlayerController p = FindObjectOfType<PlayerController>();
        data.item = (int)p.item;

        loaded = data;
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    // 게임 전체 데이터 로드 (스테이지, 점수 등)
    public bool LoadGame()
    {
        if (!File.Exists(path)) return false;
        loaded = JsonUtility.FromJson<Data>(File.ReadAllText(path));

        GameManager.I.stage = loaded.stage;
        GameManager.I.bomb = loaded.bomb;
        GameManager.I.score = loaded.score;
        GameManager.I.money = loaded.money;
        return true;
    }

    // 플레이어 데이터 로드 (아이템)
    public bool LoadPlayer()
    {
        if (!File.Exists(path)) return false;
        loaded = JsonUtility.FromJson<Data>(File.ReadAllText(path));

        PlayerController p = FindObjectOfType<PlayerController>();
        p.item = (ItemPickup.Item)loaded.item;
        return true;
    }

    // 파츠 데이터 로드 (보유 및 슬롯)
    public bool LoadPart()
    {
        if (!File.Exists(path)) return false;
        loaded = JsonUtility.FromJson<Data>(File.ReadAllText(path));

        PartManager.I.ownReturn = loaded.ownReturn;
        PartManager.I.ownSlow = loaded.ownSlow;
        PartManager.I.ownStop = loaded.ownStop;
        PartManager.I.slot1 = (PartManager.Type)loaded.slot1;
        PartManager.I.slot2 = (PartManager.Type)loaded.slot2;
        return true;
    }

    // 저장 파일 삭제
    public void DeleteSave()
    {
        loaded = null;
        File.Delete(path);
    }
}