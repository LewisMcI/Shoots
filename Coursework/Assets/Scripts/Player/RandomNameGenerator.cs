using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNameGenerator : MonoBehaviour
{
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
    public static string GenerateRandomName()
    {
        string word = wordsList[random.Next(wordsList.Count)];
        string title = titlesList[random.Next(titlesList.Count)];
        int number = random.Next(1, 1000);
        string name = $"{title}{word}{number}";
        return name;
    }
}
