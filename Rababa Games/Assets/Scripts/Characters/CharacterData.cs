using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterIcon;
    public int health;

    public AudioClip hurtSFX;
    public AudioClip attackSFX;
    public AudioClip[] idleSFX;

    public GameObject characterPrefabs;
}
