using System.Collections.Generic;
using UnityEngine;

public struct PlayerSkillData
{
    public int Key;
    public string Name;
    public string Description;
    public int MpCost;
    public int AttackPower;
    public int Duration;
    public int CoolTime;
    public int PlayerLevel;
    public int CurLevel;
    public int MaxLevel;
}

public class PlayerSkillDataManager : MonoBehaviour
{
    private static PlayerSkillDataManager _instance;
    public static PlayerSkillDataManager Instance
    {
        get { return _instance; }
    }

    private Dictionary<int, PlayerSkillData> _playerSkillDatas = new Dictionary<int, PlayerSkillData>();
    private void Awake()
    {
        _instance = this;

        LoadPlayerSkillData();
    }

    public PlayerSkillData GetPlayerSkillData(int key)
    {
        return _playerSkillDatas[key];
    }

    private void LoadPlayerSkillData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/KnightSkillTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1)
                return;

            PlayerSkillData data;
            data.Key = int.Parse(colData[0]);
            data.Name = colData[1];
            data.Description = colData[2];
            data.MpCost = int.Parse(colData[3]);
            data.AttackPower = int.Parse(colData[4]);
            data.Duration = int.Parse(colData[5]);
            data.CoolTime = int.Parse(colData[6]);
            data.PlayerLevel = int.Parse(colData[7]);
            data.CurLevel = int.Parse(colData[8]);
            data.MaxLevel = int.Parse(colData[9]);

            _playerSkillDatas.Add(data.Key, data);
        }
    }

    public void UpdateSkillLevel(int key, int newLevel)
    {
        if (_playerSkillDatas.ContainsKey(key))
        {
            PlayerSkillData data = _playerSkillDatas[key];
            data.CurLevel = newLevel;
            _playerSkillDatas[key] = data;
        }
    }

    public void UpdateSkillLevelAndAttackPower(int key, int newLevel, int newAttackPower)
    {
        if (_playerSkillDatas.ContainsKey(key))
        {
            PlayerSkillData data = _playerSkillDatas[key];
            data.CurLevel = newLevel;
            data.AttackPower = newAttackPower;
            _playerSkillDatas[key] = data;
        }
    }
}
