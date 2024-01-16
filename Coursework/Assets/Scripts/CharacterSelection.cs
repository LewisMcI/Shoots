using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] SpriteRenderer accessorySprite;

    public void UpdateCharacter()
    {
        Player player = Player.Instance;
        playerSprite.sprite = player.CharacterSprite;
        accessorySprite.sprite = player.AccessorySprite;
    }
}
