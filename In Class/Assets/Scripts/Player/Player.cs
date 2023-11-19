using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using UnityEngine;
public struct PlayerData
{
    public string playerName;
    public int characterSpriteIndex;
    public int accessorySpriteIndex;
}

public class Player : NetworkBehaviour
{
    public static Player Instance;

    PlayerData playerData = new PlayerData
    {
        playerName = "",
        characterSpriteIndex = 0,
        accessorySpriteIndex = 0
    };
    
    public TMP_InputField playerInputField;

    private SpriteLookup spriteLookup;

    // Getters
    public PlayerData PlayerData { get => playerData; }
    public string PlayerName { get => playerData.playerName; }
    public Sprite CharacterSprite { get => spriteLookup.GetCharacterSprite(playerData.characterSpriteIndex); }
    public Sprite AccessorySprite { get => spriteLookup.GetAccessoriesSprite(playerData.accessorySpriteIndex); }

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
        playerData.playerName = Regex.Replace(name.Trim(), @"\s+", " ");
    }
    public void UpdateCharacterSprite(int spriteIndex)
    {
        Debug.Log("Update Character Sprite to Sprite: " + spriteIndex);
        playerData.characterSpriteIndex = spriteIndex;
    }
    public void UpdateAccessoriesSprite(int spriteIndex)
    {
        Debug.Log("Update Accessory Sprite to Sprite: " + spriteIndex);
        playerData.accessorySpriteIndex = spriteIndex;
    }

}
