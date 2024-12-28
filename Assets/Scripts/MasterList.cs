using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class MasterList
{
    [SerializeField] private List<string> playerList;

    public MasterList()
    {
        playerList = new List<string>();
    }

    public void addPlayer(string name)
    {
        playerList.Add(name);
    }

    public void removePlayer(string name)
    {
        playerList.Remove(name);
    }

    public bool contains(string name)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList.ElementAt(i) == name)
            {
                return true;
            }
        }
        return false;
    }
    public string getPlayer(string name)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList.ElementAt(i) == name)
            {
                return playerList.ElementAt(i);
            }
        }
        return null;
    }

    public List<string> getPlayerList()
    {
        return playerList;
    }
}