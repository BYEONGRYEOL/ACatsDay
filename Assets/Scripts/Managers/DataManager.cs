using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace Isometric.Data
{
    // 로드한 데이터에 대하여 
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
        
        // 플레이어 레벨업에 의한 고정적인 stat의 증가 등은 CSV파일로 관리하여 수정하기 용이하게 적용
        // 그런데 종료 시  플레이어가 어디있었는지 같은 게임 내에서 발생한 일에대하여 save 하는건 Json 만으로 충분하다.
        // 그래서 기본적인 Stat, 혹은 정해진 Quest, 잠금해제 같은 데이터를 CSV로 먼저 작성한 뒤 Json으로 변환해주는 Util 함수가 필요할거같아.
        // 그런데 게임 첫 시작시 CSV to json 변환 혹은 게임파일 업데이트 시 에만 적용하니까 굳이 Util 함수에 집어넣진 말자~~


        // json방식으로 save하기위하여 필요한 json saver
        public JsonSaver jsonSaver;
        public Settings settings;
        public GameData gameData;

        // CSV리소스 데이터들을 관리하기 위함
        public Dictionary<int, PlayerStatData> PlayerStatDict { get; private set; } = new Dictionary<int, PlayerStatData>();
        public Dictionary<int, EnemyStatData> EnemyStatDict { get; private set; } = new Dictionary<int, EnemyStatData>();
        public Dictionary<int, ItemInfo> ItemInfoDict { get; private set; } = new Dictionary<int, ItemInfo>();
        public Dictionary<int, WeaponInfo> WeaponInfoDict { get; private set; } = new Dictionary<int, WeaponInfo>();
        public Dictionary<int, ArmorInfo> ArmorInfoDict { get; private set; } = new Dictionary<int, ArmorInfo>();
        public Dictionary<int, ConsumableInfo> ConsumableInfoDict { get; private set; } = new Dictionary<int, ConsumableInfo>();
        public Dictionary<int, UseableInfo> UseableInfoDict { get; private set; } = new Dictionary<int, UseableInfo>();
        public Dictionary<int, ItemData> ItemDBDict { get; private set; } = new Dictionary<int, ItemData>();


        public void Init()
        {
            settings = new Settings();
            jsonSaver = new JsonSaver();
            gameData = new GameData();
            //실행되자마자 CSV 파일을 참고하여 Json 파일이 있는지 없는지 체크 후 없으면 생성

            //CSV 파일로 관리하는 리소스데이터에 대하여 실행
            MakeJson(Datas.EnemyStat.ToString());
            MakeJson(Datas.PlayerStat.ToString());
            MakeJson(Datas.ItemInfo.ToString());
            MakeJson(Datas.WeaponInfo.ToString());
            MakeJson(Datas.ArmorInfo.ToString());
            MakeJson(Datas.ConsumableInfo.ToString());
            MakeJson(Datas.UseableInfo.ToString());




            //json 파일이 다 만들어지기 전까진 로딩 씬에서 넘어가지 않았으면함
            //그래서 json 파일이 다 로딩 되엇는지 안되었는지 UI_Loading에서 검사함
        }

        // CSV 파일을 Json으로 변환 후 Json 파일들을 읽어 데이터로 받아와 관리하기 때문에 Json 파일 이 만들어진 후에 게임이 실행되거나 아니면
        // 해당 스테이지에 필요한 로딩을 할 때 스테이지 게임씬으로 넘어가기전에 꼭 만들어야 한다.
        // 코루틴으로 할지 액션 으로할지 아무튼 비동기식 이 필요한데 좀 어렵네

        public enum Datas
        {
            PlayerStat,
            EnemyStat,
            ItemInfo,
            WeaponInfo,
            ArmorInfo,
            ConsumableInfo,
            UseableInfo,
            ItemDB
        }
        
        //json 파일 생성기
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
            Debug.Log("Datamanager : MakeJsontoDict 실행");
            PlayerStatDict = LoadJson<PlayerStatLoader, int, PlayerStatData>("PlayerStatjson").MakeDict();
            EnemyStatDict = LoadJson<EnemyStatLoader, int, EnemyStatData>("EnemyStatjson").MakeDict();
            ItemInfoDict = LoadJson<ItemInfoData, int, ItemInfo>("ItemInfojson").MakeDict();
            WeaponInfoDict = LoadJson<WeaponInfoData, int, WeaponInfo>("WeaponInfojson").MakeDict();
            ArmorInfoDict = LoadJson<ArmorInfoData, int, ArmorInfo>("ArmorInfojson").MakeDict();
            ConsumableInfoDict = LoadJson<ConsumableInfoData, int, ConsumableInfo>("ConsumableInfojson").MakeDict();
            UseableInfoDict = LoadJson<UseableInfoData, int, UseableInfo>("UseableInfojson").MakeDict();
            ItemDBDict = LoadJson<ItemLoader, int, ItemData>("ItemDBjson").MakeDict();

            Debug.Log("ItemDBDict 로딩 :: " + ItemDBDict.Count);
        }

        public void MakeJsontoList()
        {
            // 리스트 형식의 게임 리소스데이터를 들고있으려면 이걸실행
        }

        //Resouce/Data/여기에 데이터를 받아올 해당 CSV 파일이 있어야함
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
            jsonSaver.Save(gameData, "GameData");

        }
        public void Load()
        {
            jsonSaver.Load(settings, "Settings");
            jsonSaver.Load(gameData, "GameData");

        }

    }

}