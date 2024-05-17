using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using UnityEngine.SceneManagement;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public GameObject camera;
    public List<GameObject> Players = new List<GameObject>();
    public int NumPlayers = 0;
    public string player1Weapon;
    public List<GameObject> Cooldowns = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    
    public void AddPlayer(int slot)
    {
        if (NumPlayers == 4)
            return;

        NumPlayers++;
    }

    public void ChangePlayerClass(int slot, string Class)
    {
        Destroy(transform.GetChild(slot).GetChild(0).gameObject);
        GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/Classes/" + Class), transform.GetChild(slot));
        Transform position = GameObject.Find("CharacterSlotPositions").transform.GetChild(slot).transform;
        temp.transform.position = position.transform.position;
        temp.transform.localScale = position.transform.localScale;
        temp.name = "Player" + (slot + 1);
        temp.SetActive(true);
    }

    public void RemovePlayer(int slot)
    {
        Destroy(transform.GetChild(slot).GetChild(0).gameObject);
        NumPlayers--;
    }

    public void StartGame()
    {
        if (NumPlayers == 1)
        {
            player1Weapon = CharacterCustomization.instance.WeaponName;
            GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/Classes/" + CharacterCustomization.instance.className.text), transform.GetChild(0));
            temp.transform.GetComponent<SpriteRenderer>().color = CharacterCustomization.instance.skinColor;
			temp.transform.GetChild(0).GetComponent<SpriteRenderer>().color = CharacterCustomization.instance.hairColor;
			temp.transform.GetChild(1).GetComponent<SpriteRenderer>().color = CharacterCustomization.instance.skinColor;
			temp.SetActive(true);
            camera.SetActive(false);
        }
    }

}
