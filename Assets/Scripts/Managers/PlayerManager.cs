using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public Dictionary<int, Player> Players = new Dictionary<int, Player>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 1; i < 5; i++)
        {
            AddPlayer(i);
        }
    }

    public void AddPlayer(int slot)
    {
        if (Players.Count == 4)
            return;
      
        Player player = new Player(Random.Range(1, 100));
        Players.Add(slot, player);
        PlayerUI.instance.updateUI();
    }

    public void RemovePlayer(int slot)
    {
        Players.Remove(slot);
        PlayerUI.instance.updateUI();
    }

    public void PlayerAttack(int player)
    {
        BattleManager.instance.Actions.Add(new BattleAction(player, "Attack", 1, "Increase"));
        PlayerUI.instance.PlayerAction(player);
        ActionBar.instance.UpdateActionBar();
        Targetable.instance.FindTargettable();
    }

    public void PlayerBlock(int player)
    {
        BattleManager.instance.Actions.Add(new BattleAction(player, "Block"));
        PlayerUI.instance.PlayerAction(player);
        ActionBar.instance.UpdateActionBar();
    }

    public void PlayerAbility(int player)
    {
        BattleManager.instance.Actions.Add(new BattleAction(player, "Ability", 1, "Decrease"));
        PlayerUI.instance.PlayerAction(player);
        ActionBar.instance.UpdateActionBar();
    }

    public void PlayerUltimate(int player)
    {
        BattleManager.instance.Actions.Add(new BattleAction(player, "Ultimate"));
        PlayerUI.instance.PlayerAction(player);
        ActionBar.instance.UpdateActionBar();
    }

}
