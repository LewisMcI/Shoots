using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using UnityEngine;
public struct PlayerData
{
    public string name;
    public float maxHealth;
    public float currHealth;
}

public class Player : NetworkBehaviour
{
    public static Player Instance;

    PlayerData playerData = new PlayerData
    {
        name = "",
        maxHealth = 100,
        currHealth = 100
    };
    
    public TMP_InputField playerInputField;

    private SpriteLookup spriteLookup;

    // Getters
    public PlayerData PlayerData { get => playerData; }
    public string PlayerName { get => playerData.name; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.Log("Destroying other Player Instance");
            Destroy(this);
        }
        spriteLookup = GetComponent<SpriteLookup>();
        if (!spriteLookup)
            throw new Exception("Could not find Sprite Table");
        if (PlayerName == "")
            UpdateName(RandomNameGenerator.GenerateRandomName());
        playerInputField.text = PlayerName;
    }

    public void UpdateName(string name)
    {
        // Regex for removing double spaces
        playerData.name = Regex.Replace(name.Trim(), @"\s+", " ");
    }

    public void UpdateMaxHealth(float maxHealth)
    {
        playerData.maxHealth = maxHealth;
    }
    public void UpdateCurrHealth(float currHealth)
    {
        playerData.currHealth = currHealth;
    }
}
