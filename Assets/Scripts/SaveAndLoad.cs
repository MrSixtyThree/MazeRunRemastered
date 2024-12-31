using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveAndLoad : MonoBehaviour
{
    string SaveLocation;
    public MasterList masterList;

    public void setSaveLocation()
    {
        SaveLocation = Application.persistentDataPath;
        SaveLocation += "/SaveFiles";

        // Ensure the directory exists
        if (!Directory.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }
    }

    public void saveMasterList()
    {
        string masterListText = JsonUtility.ToJson(masterList, true);
        File.WriteAllText(SaveLocation + "/masterList.json", masterListText);
    }

    public void loadMasterList()
    {
        if (File.Exists(SaveLocation + "/masterList.json"))
        {
            string masterListText = File.ReadAllText(SaveLocation + "/masterList.json");
            masterList = JsonUtility.FromJson<MasterList>(masterListText);
        }
        else
        {
            masterList = new MasterList();
        }
    }

    public void Save(UserData user)
    {
        if(!masterList.contains(user.getPlayerName()))
        {
            masterList.addPlayer(user.getPlayerName());
            saveMasterList();
        }
        string jsonSave = JsonUtility.ToJson(user, true);

        File.WriteAllText(SaveLocation + "/" + user.getPlayerName() + ".json", jsonSave);
    }

    public UserData Load(string name)
    {
        string filePath = SaveLocation + "/" + name + ".json";
        if (masterList.contains(name) && File.Exists(filePath))
        {
            string userText = File.ReadAllText(filePath);
            return JsonUtility.FromJson<UserData>(userText);
        }
        else
        {
            return null;
        }
    }
}
