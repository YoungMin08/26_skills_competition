// 랭킹 데이터 관리 스크립트
// 점수 저장, 정렬, 상위 5개 유지, 텍스트 출력까지 담당

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class RankingManager : MonoBehaviour
{
    public static RankingManager I;

    [System.Serializable]
    public class Data
    {
        public string name;
        public int score;
    }
    [System.Serializable]
    public class RankSave
    {
        public List<Data> list = new List<Data>();
    }
    RankSave save = new RankSave();
    public string path;

    // 싱글톤 설정 + 저장 경로 설정 + 데이터 로드
    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
        path = Application.persistentDataPath + "/rank.json";
        Load();
    }

    // 랭킹 데이터를 파일로 저장
    public void Save()
    {
        File.WriteAllText(path, JsonUtility.ToJson(save, true));
    }

    // 파일에서 랭킹 데이터 불러오기
    public void Load()
    {
        if (!File.Exists(path)) return;
        save = JsonUtility.FromJson<RankSave>(File.ReadAllText(path));
        if (save == null) save = new RankSave();
        else if (save.list == null) save.list = new List<Data>();
    }

    // 랭킹 추가 + 점수 내림차순 정렬 + 상위 5개 유지
    public void AddRank(string n, int s)
    {
        save.list.Add(new Data { name = n, score = s });
        save.list.Sort((a, b) => b.score.CompareTo(a.score));
        if (save.list.Count > 5) save.list.RemoveRange(5, save.list.Count - 5);
        Save();
    }

    // 랭킹 데이터 초기화
    public void DeleteSave()
    {
        save.list.Clear();
    }

    // 랭킹 텍스트 생성 (UI 출력용)
    public string GetRankText()
    {
        if (save.list.Count == 0) return "Rank\n\nNO DATA";
        StringBuilder sb = new StringBuilder("RANK\n\n");
        for (int i = 0; i < save.list.Count; i++) sb.AppendLine($"{i + 1}. {save.list[i].name} {save.list[i].score}");
        return sb.ToString();
    }
}