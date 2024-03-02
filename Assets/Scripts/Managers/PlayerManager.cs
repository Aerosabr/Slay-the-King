using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class PlayerManager : MonoBehaviour
{
    //Script that contains all players and manages their inputs
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
        StartCoroutine(ChoosingTarget(player, new BattleAction(player, "Attack", 1, "Increase")));
    }

    public void PlayerBlock(int player)
    {
        BattleManager.instance.Actions.Add(new BattleAction(player, "Block"));
        PlayerUI.instance.PlayerAction(player);
        ActionBar.instance.UpdateActionBar();
    }

    public void PlayerAbility(int player)
    {
        StartCoroutine(ChoosingTarget(player, new BattleAction(player, "Ability", 1, "Decrease")));
    }

    public void PlayerUltimate(int player)
    {
        StartCoroutine(ChoosingTarget(player, new BattleAction(player, "Ultimate")));
    }

    public void PlayerCancel(int player)
    {

    }

    IEnumerator ChoosingTarget(int player, BattleAction action)
    {
        Targetable.instance.Targetted = false;
        Targetable.instance.FindTargettable();
        yield return new WaitUntil(() => Targetable.instance.Targetted);
        Targetable.instance.Targetted = false;
        BattleManager.instance.Actions.Add(action);
        PlayerUI.instance.PlayerAction(player);
        ActionBar.instance.UpdateActionBar();
    }
}
