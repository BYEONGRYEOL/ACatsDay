using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace Isometric.Data
{
    // �ε��� �����Ϳ� ���Ͽ� 
    public interface ILoaderDict<Key, Value> 
    {
        Dictionary<Key, Value> MakeDict();
    }
    public interface ILoaderList<list> 
    {
        List<list> MakeList();
    }


    public class DataManager
    {
        
        // �÷��̾� �������� ���� �������� stat�� ���� ���� CSV���Ϸ� �����Ͽ� �����ϱ� �����ϰ� ����
        // �׷��� ���� ��  �÷��̾ ����־����� ���� ���� ������ �߻��� �Ͽ����Ͽ� save �ϴ°� Json ������ ����ϴ�.
        // �׷��� �⺻���� Stat, Ȥ�� ������ Quest, ������� ���� �����͸� CSV�� ���� �ۼ��� �� Json���� ��ȯ���ִ� Util �Լ��� �ʿ��ҰŰ���.
        // �׷��� ���� ù ���۽� CSV to json ��ȯ Ȥ�� �������� ������Ʈ �� ���� �����ϴϱ� ���� Util �Լ��� ������� ����~~


        // json������� save�ϱ����Ͽ� �ʿ��� json saver
        public JsonSaver jsonSaver;
        public Settings settings;

        // CSV���ҽ� �����͵��� �����ϱ� ����
        public Dictionary<int, PlayerStat> PlayerStatDict { get; private set; } = new Dictionary<int, PlayerStat>();
        public Dictionary<int, EnemyStat> EnemyStatDict { get; private set; } = new Dictionary<int, EnemyStat>();
        public Dictionary<int, ItemInfo> ItemInfoDict { get; private set; } = new Dictionary<int, ItemInfo>();


        public List<ItemDB> ItemDBList { get; private set; } = new List<ItemDB>();
        public void Init()
        {
            settings = new Settings();
            jsonSaver = new JsonSaver();
            //������ڸ��� CSV ������ �����Ͽ� Json ������ �ִ��� ������ üũ �� ������ ����

            //CSV ���Ϸ� �����ϴ� ���ҽ������Ϳ� ���Ͽ� ����
            MakeJson(Datas.EnemyStat.ToString());
            MakeJson(Datas.PlayerStat.ToString());
            MakeJson(Datas.ItemInfo.ToString());
            

            
            //json ������ �� ��������� ������ �ε� ������ �Ѿ�� �ʾ�������
            //�׷��� json ������ �� �ε� �Ǿ����� �ȵǾ����� UI_Loading���� �˻���
        }

        // CSV ������ Json���� ��ȯ �� Json ���ϵ��� �о� �����ͷ� �޾ƿ� �����ϱ� ������ Json ���� �� ������� �Ŀ� ������ ����ǰų� �ƴϸ�
        // �ش� ���������� �ʿ��� �ε��� �� �� �������� ���Ӿ����� �Ѿ������ �� ������ �Ѵ�.
        // �ڷ�ƾ���� ���� �׼� �������� �ƹ�ư �񵿱�� �� �ʿ��ѵ� �� ��Ƴ�

        public enum Datas
        {
            PlayerStat,
            EnemyStat,
            ItemInfo,
            ItemDB
        }
        
        //json ���� ������
        public bool MakeJson(string csvName)
        {
            return CSVtoJson.ConvertCsvFileToJsonObject(csvName);
        }

        public bool IsJsonLoaded()
        {
            List<bool> a = new List<bool>();
            foreach (Datas datas in Enum.GetValues(typeof(Datas)))
            {
                a.Add(MakeJson(datas.ToString()));
            }
            if (a.Contains(false))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void MakeJsontoDict()
        {
            Debug.Log("Datamanager : MakeJsontoDict ����");
            PlayerStatDict = LoadJson<PlayerStatData, int, PlayerStat>("PlayerStatjson").MakeDict();
            EnemyStatDict = LoadJson<EnemyStatData, int, EnemyStat>("EnemyStatjson").MakeDict();
            ItemInfoDict = LoadJson<ItemInfoData, int, ItemInfo>("ItemInfojson").MakeDict();
        }

        public void MakeJsontoList()
        {
            ItemDBList = LoadJson<ItemData, ItemDB>("ItemDBjson").MakeList();
        }
        //Resouce/Data/���⿡ �����͸� �޾ƿ� �ش� CSV ������ �־����
        Loader LoadJson<Loader, list>(string path) where Loader : ILoaderList<list>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
            return JsonUtility.FromJson<Loader>(textAsset.text);
        }
        Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoaderDict<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
            
            return JsonUtility.FromJson<Loader>(textAsset.text);
        }
        

        public void Save()
        {
            jsonSaver.Save(settings, "Settings");
        }
        public void Load()
        {
            jsonSaver.Load(settings, "Settings");
        }

    }

}