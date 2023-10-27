using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public string playerName = "";
    public GameObject defaultPlayerCard;
    public TMP_InputField playerInputField;

    private GameObject playerCard;

    private static readonly System.Random random = new();

    private static List<string> wordsList = new List<string>
{
    "Sorcer", "Warior", "Mage", "Knight", "Druid", "Elven", "Necro", "Witch", "Rogue",
    "Shadow", "Viper", "Astral", "Titan", "Faith", "Bane", "Abyss", "Coven", "Blaze"
};

    private static List<string> titlesList = new List<string>
{
    "Mystic", "Oracle", "Doom", "Sorcer", "Warior", "Witch", "Elven", "Druid", "Angel",
    "Viper", "Abyss", "Rogue", "Mage", "Frost", "Necro", "Serpnt", "Wyrm"
};
    public static Player Instance;

    private void Start()
    {
        if (!IsOwner)
            return;
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.Log("Destroying other Player Instance");
            Destroy(this);
        }

        if (playerName == "")
            playerName = GenerateRandomName();
        playerInputField.text = playerName;
        Debug.Log("Done: " + playerName);
    }
    public static string GenerateRandomName()
    {
        string word = wordsList[random.Next(wordsList.Count)];
        string title = titlesList[random.Next(titlesList.Count)];
        int number = random.Next(1, 1000); 
        string name = $"{title}{word}{number}";
        return name;
    }

    public GameObject CreatePlayerCard()
    {
        playerCard = Instantiate(defaultPlayerCard);
        return playerCard;
    }

    public void UpdateName(string name)
    {
        // Regex for removing double spaces
        playerName = Regex.Replace(name.Trim(), @"\s+", " ");
    }
}
