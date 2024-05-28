using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class Player : Entity, IEffectable, IDamageable
{
    //Player Class
    public string Class;

    //Ability Cooldowns
    public float AttackCD;
    public float Ability1CD;
    public float Ability2CD;
    public float UltimateCD;
    public float MovementCD;
    public List<GameObject> Cooldowns = new List<GameObject>();

    public Rigidbody2D rb;

    //ConsumableHotBar
    public ActivateConsumables[] consumableSlot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
	}

    void Update()
    {
        if (Buffs.Count > 0)
            HandleBuff();
    }

    #region IEffectable Components
    public Dictionary<string, Buff> Buffs = new Dictionary<string, Buff>();

    public void ApplyBuff(Buff buff)
    {
        if (Buffs.ContainsKey(buff.Source))
            RemoveBuff(Buffs[buff.Source]);

        Buffs.Add(buff.Source, buff);
        Buffs[buff.Source].ApplyEffect();
    }

    public void RemoveBuff(Buff buff)
    {
        buff.RemoveEffect();
        Buffs.Remove(buff.Source);
    }

    public void HandleBuff()
    {
        List<string> keys = Buffs.Keys.ToList();
        foreach (string key in keys)
        {
            if (Buffs[key].HandleEffect())
                RemoveBuff(Buffs[key]);
        }
    }
    #endregion

    #region IDamageable Components
    public int Damaged(int amount) 
    {
        int damage = 0;
        amount = Mathf.Abs(amount);
        if (Shield > 0)
        {
            if (Shield >= amount)
            {
                damage += amount;
                Shield -= amount;
                amount = 0;
            }
            else
            {
                damage += amount - Shield;
                amount -= Shield;
                Shield = 0;
            }
        }

        if (amount > 0)
        {
            damage += (Mathf.Abs(amount) - Defense > 0) ? Mathf.Abs(amount) - Defense : 1;

            if (currentHealth - damage > 0)
                currentHealth -= damage;
            else
            {
                damage = currentHealth;
                currentHealth = 0;
                GetComponent<PlayerSpriteController>().PlayAnimation("Death");
                GetComponent<PlayerSpriteController>()._rigidbody.velocity = Vector2.zero;
                GetComponent<PlayerSpriteController>().Movable = false;
            }
        }
        
        DamagePopup.Create(rb.transform.position, damage, false);
        return damage;
    }

    public int trueDamaged(int amount)
    {
        int damage;
        if (amount > currentHealth)
        {
            damage = currentHealth;
            currentHealth = 0;
            GetComponent<PlayerSpriteController>().PlayAnimation("Death");
            GetComponent<PlayerSpriteController>()._rigidbody.velocity = Vector2.zero;
            GetComponent<PlayerSpriteController>().Movable = false;
        }
        else
        {
            damage = currentHealth - amount;
            currentHealth -= amount;
        }

        return damage;
    }

    public int Healed(int amount)
    {
        int totalHealed;
        if (currentHealth + amount > maxHealth)
        {
            totalHealed = maxHealth - currentHealth;
            currentHealth = maxHealth;
        }
        else
        {
            totalHealed = amount;
            currentHealth += amount;
        }

        DamagePopup.Create(rb.transform.position, amount, false);
        return totalHealed;
    }
    #endregion

    public Player GetPlayerComponent()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        if (playerManager != null)
        {
            Transform player1Transform = playerManager.transform.Find("Player1");
            if (player1Transform != null && player1Transform.childCount > 0)
            {
                return player1Transform.GetChild(0).GetComponent<Player>();
            }
            else
            {
                Debug.LogError("Player1 does not exist or has no children");
            }
        }
        else
        {
            Debug.LogError("PlayerManager not found in the scene");
        }
        return null;
    }

    public void UpdateEquipmentStats(EquipmentSO equipment, int change)
    {
        changeHealth(equipment.health * change, 0);
        changeAttack(equipment.attack * change, 0);
        changeDefense(equipment.defense * change, 0);
        changeDexterity(equipment.dexterity * change, 0);
        changeAttackSpeed(equipment.attack_speed * change, 0);
        CDR += equipment.cooldown_reduction * change;
        Luck += equipment.luck * change;
    }

    public void OnConsume1()
    {
        consumableSlot[0].Activate();
    }

	public void OnConsume2()
	{
		consumableSlot[1].Activate();
	}

	public void OnConsume3()
	{
		consumableSlot[2].Activate();
	}

}
