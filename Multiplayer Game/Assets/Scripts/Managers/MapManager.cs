using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [SerializeField] Map[] maps;

    public Scene currentPrepedScene;

    public int currentMapNumber;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentMapNumber = 2;
    }

    void Update()
    {
        if (!MenuManager.Instance.ReturnIsOpenMenuName("room"))
            return;

    }

    //public int ReturnSelectedMapInt()
    //{
    //    foreach (Map _map in maps)
    //    {
    //        if (_map.mapSelected && _map.selectionMarker.activeSelf)
    //        {
    //            return _map.mapIndexNumber;
    //        }
    //    }
    //    return 1;
    //}

    public bool ReturnSelectedMap(int mapBuildIndexNumber)
    {
        foreach (Map _map in maps)
        {
            if (_map.mapIndexNumber == mapBuildIndexNumber && _map.selectionMarker.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public void SelectMap(int mapBuildIndexNumber)
    {
        for (int i = 0; i < maps.Length; i++)
        {
            if (maps[i].mapIndexNumber == mapBuildIndexNumber)
            {
                maps[i].Select();
                currentMapNumber = maps[i].mapIndexNumber;
            }
            else if (maps[i].mapSelected)
            {
                DeSelectMap(maps[i]);
            }
        }

    }

    public void DeSelectMap(Map map)
    {
        map.DeSelect();
    }
}
