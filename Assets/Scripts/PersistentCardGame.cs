using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PersistentCardGame {

    #region WinLose
    public static void AddWin()
    {
        SetTotalWins(GetTotalWins() + 1);

        CreditBalance(3);


        if (PhotonNetwork.offlineMode && WifiNetworkManager.instance != null)
        {
            SetWifiTotalWins(GetWifiTotalWins() + 1);
        }
        else
        {
            if (PhotonNetwork.offlineMode && WifiNetworkManager.instance == null)
            {
                SetSingleplayerTotalWins(GetSingleplayerTotalWins() + 1);
            }
            else
            {
                if (PhotonNetwork.inRoom && PhotonNetwork.connected)
                {
                    SetMultiplayerTotalWins(GetMultiplayerTotalWins() + 1);

                }
                else
                {
                    Debug.Log("________ Error: Not identified game mode to set score");
                }
            }
        }
    }

    public static void AddLose()
    {
        SetTotalLoses(GetTotalLoses() + 1);
        //CreditBalance(2);

        if (PhotonNetwork.offlineMode && WifiNetworkManager.instance != null)
        {
            SetWifiTotalLoses(GetWifiTotalLoses() + 1);
        }
        else
            if (PhotonNetwork.offlineMode && WifiNetworkManager.instance == null)
        {
            SetSingleplayerTotalLoses(GetSingleplayerTotalLoses() + 1);
        }
        else
                if (PhotonNetwork.inRoom && PhotonNetwork.connected)
        {
            SetMultiplayerTotalLoses(GetMultiplayerTotalLoses() + 1);

        }
        else
        {
            Debug.Log("________ Error: Not identified game mode to set score");
        }

    }
    #endregion

    #region Total

    public static int GetTotalWins()
    {
        return PlayerPrefs.GetInt("TotalWins");
    }

    public static void SetTotalWins(int wins)
    {
        Debug.Log("SetTotalWins=" + wins);
        PlayerPrefs.SetInt("TotalWins", wins);
    }


    public static int GetTotalLoses()
    {
        return PlayerPrefs.GetInt("TotalLoses");
    }

    public static void SetTotalLoses(int loses)
    {
        Debug.Log("SetTotalLoses=" + loses);
        PlayerPrefs.SetInt("TotalLoses", loses);
    }


    //Replaced with Balance
    // public static int GetTotalPoints()
    // {
    //     return PlayerPrefs.GetInt("Points");
    // }
    // public static void SetTotalPoints(int points)
    // {
    //     Debug.Log("SetTotalPoints=" + points);
    //     PlayerPrefs.SetInt("Points", points);
    // }


    #endregion

    #region Multiplayer

    public static int GetMultiplayerTotalWins()
    {
        return PlayerPrefs.GetInt("MultiplayerTotalWins");
    }

    public static void SetMultiplayerTotalWins(int wins)
    {
        Debug.Log("SetMultiplayerTotalWins=" + wins);
        PlayerPrefs.SetInt("MultiplayerTotalWins", wins);
    }


    public static int GetMultiplayerTotalLoses()
    {

        return PlayerPrefs.GetInt("MultiplayerTotalLoses");
    }

    public static void SetMultiplayerTotalLoses(int points)
    {
        Debug.Log("SetMultiplayerTotalLoses=" + points);
        PlayerPrefs.SetInt("MultiplayerTotalLoses", points);
    }


    #endregion

    #region Wifi

    public static int GetWifiTotalWins()
    {
        return PlayerPrefs.GetInt("WifiTotalWins");
    }

    public static void SetWifiTotalWins(int wins)
    {
        Debug.Log("SetWifiTotalWins=" + wins);
        PlayerPrefs.SetInt("WifiTotalWins", wins);
    }


    public static int GetWifiTotalLoses()
    {
        return PlayerPrefs.GetInt("WifiTotalLoses");
    }

    public static void SetWifiTotalLoses(int points)
    {
        Debug.Log("SetWifiTotalLoses=" + points);
        PlayerPrefs.SetInt("WifiTotalLoses", points);
    }

    #endregion

    #region Singleplayer

    public static int GetSingleplayerTotalWins()
    {
        return PlayerPrefs.GetInt("SingleplayerTotalWins");
    }

    public static void SetSingleplayerTotalWins(int wins)
    {
        Debug.Log("SetSingleplayerTotalWins=" + wins);
        PlayerPrefs.SetInt("SingleplayerTotalWins", wins);
    }


    public static int GetSingleplayerTotalLoses()
    {
        return PlayerPrefs.GetInt("SingleplayerTotalLoses");
    }

    public static void SetSingleplayerTotalLoses(int points)
    {
        Debug.Log("SetSingleplayerTotalLoses=" + points);
        PlayerPrefs.SetInt("SingleplayerTotalLoses", points);
    }


    #endregion

    #region Balance

    public const string CURRENCY = "coins";
    public const int DEFAULT_CURRENCY = 100;
    public delegate void onBalanceChanged();
    public static event onBalanceChanged balanceChanged;

    public static int GetBalance()
    {
        return PlayerPrefs.GetInt(CURRENCY, DEFAULT_CURRENCY);
    }

    public static void SetBalance(int value)
    {
        PlayerPrefs.SetInt(CURRENCY, value);
        PlayerPrefs.Save();
        if (balanceChanged != null)
            balanceChanged.Invoke();
    }

    public static void CreditBalance(int value)
    {
        int current = GetBalance();
        SetBalance(current + value);
    }

    public static bool DebitBalance(int value)
    {
        int current = GetBalance();
        if (current < value)
            return false;

        SetBalance(current - value);
        return true;
    }

    #endregion

    #region Chests

    public static int GetChestAmount()
    {
        return PlayerPrefs.GetInt("ChestAmount", 0);
    }

    public static void SetChestAmount(int val)
    {
        int _val = val;
        Mathf.Clamp(_val, 0, int.MaxValue); //Amout of chests can't get below 0
        PlayerPrefs.SetInt("ChestAmount", _val);
    }

    #endregion

    #region Cards

    static readonly string path = Application.persistentDataPath + "/";
    static readonly string cardFileExtension = ".cr";

    public static Card[] GetDroppableCards()
    {
        List<Card> toReturn = new List<Card>();
        Card[] allCards = Resources.LoadAll<Card>("Prefabs/Cards/Resources");

        foreach (Card c in allCards)
            if (GetCopiesInTotal(c) != c.limitCopiesInUse)
                toReturn.Add(c);
        return toReturn.ToArray();
    }

    public static int GetTotalNumOfMoreCopiesToDrop()
    {
        int totalLimitPerDecks = 0;
        int totalOwnedCardsAmount = 0;
        foreach (Card c in GetAllCardsExceptGeAtDf())
            totalLimitPerDecks += c.limitCopiesInUse;
        foreach (Card c in GetAllCardsExceptGeAtDf())
            totalOwnedCardsAmount += GetCopiesInTotal(c);

        return Mathf.Clamp(totalLimitPerDecks - totalOwnedCardsAmount, 0, int.MaxValue);
    }

    public static Card[] GetAllCardsExceptGeAtDf()
    {
        //All cards except General, Attack, Defense
        List<Card> toReturn = new List<Card>(Resources.LoadAll<Card>("Prefabs/Cards/Resources"));
        toReturn.Remove(CardManager.GetCardPrefabByTitle("General"));
        toReturn.Remove(CardManager.GetCardPrefabByTitle("Attack"));
        toReturn.Remove(CardManager.GetCardPrefabByTitle("Defense"));
        return toReturn.ToArray();
    }

    public static int GetCopiesInTotal(Card card)
    {
        if (File.Exists(path + card.title + cardFileExtension))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path + card.title + cardFileExtension, FileMode.Open);
            int data = (int)bf.Deserialize(file);
            file.Close();
            return data;
        }
        else return card.defaultCopiesInTotal;
    }

    public static void SetCopiesInTotal(Card card, int newValue)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path + card.title + cardFileExtension);

        bf.Serialize(file, newValue);
        file.Close();
    }

    public static void AddOneToTotalCopiesAmount(Card[] cards)
    {
        foreach (Card c in cards)
            SetCopiesInTotal(c, GetCopiesInTotal(c) + 1);
    }

    public static Card[] GetAllSavedCards()
    {
        List<Card> toReturn = new List<Card>();

        DirectoryInfo levelDirectoryPath = new DirectoryInfo(path);
        FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);
        foreach (FileInfo file in fileInfo)
        {
            if (file.Extension == cardFileExtension)
            {
                string name = file.Name.Remove(file.Name.Length - 3, 3);
                //Debug.Log(name);
                Card c = CardManager.GetCardPrefabByTitle(name);
                toReturn.Add(c);
            }
        }

        return toReturn.ToArray();
    }

    public static void ResetDeck()
    {
        foreach (Card c in GetAllSavedCards())
            SetCopiesInTotal(c, c.defaultCopiesInTotal);
    }

    #endregion

}
