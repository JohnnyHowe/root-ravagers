using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Persistence : MonoBehaviour
{
    [System.NonSerialized]
    public Data Data;
    // Start is called before the first frame update
    void Start()
    {
        Load();
        //Print();
    }

    private void Print()
    {
        foreach (var score in Data.Scores)
        {
            Debug.Log($"{score.Key}: {score.Value}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    Data.Scores.Add($"Bob{UnityEngine.Random.Range(1, 100)}", UnityEngine.Random.Range(1, 100));
        //    Save();
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Print();
        //}
    }

    public void Save()
    {
        File.WriteAllText(GetPath(), JsonUtility.ToJson(Data));
    }

    private static string GetPath()
    {
        return Path.Join(Application.persistentDataPath, "data.json");
    }

    public void Load()
    {
        try
        {
            string json = File.ReadAllText(GetPath());
            Data = JsonUtility.FromJson<Data>(json);
        } catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            Data = new Data();
        }
    }
}


public class Data
{
    public Dictionary<string, int> Scores = new();
}