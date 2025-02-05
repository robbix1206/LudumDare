﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Character : MonoBehaviour {
    [Tooltip("Minion type.")]
    public MinonType minion;
    [Tooltip("Is the character the player ?.")]
    public bool player;
    [Tooltip("Minimum coins the character can drop.")]
    public int minCoin;
    [Tooltip("Maximum coins the character can drop.")]
    public int maxCoin;
    [Tooltip("Minimum xp the character can drop.")]
    public int minXp;
    [Tooltip("Maximum xps the character can drop.")]
    public int maxXp;
    [Tooltip("Character's life.")]
    public int life;
    [Tooltip("Rigidbody of the coin.")]
    public Rigidbody2D coin;
    [Tooltip("Rigidbody of the xp.")]
    public Rigidbody2D xp;
    [Tooltip("The time the character is immortal after being hurt.")]
    public float immunityTime = 0.6f;
    private float immuneTime = 0.0f;
    private bool color_modified = false;
    private Color color;
    [Tooltip("Speed of the character.")]
    public float speed;

    [Tooltip("The team the character belong to")]
    public Team team;

    /* For attack */
    [Tooltip("Damages.")]
    public int damages;
    private int damage_ref;
    private bool attacking = false;
    [Tooltip("Cooldown between 2 attacks.")]
    public float attackCd = 0.5f;
    private float attackCd_ref;
    [Tooltip("Cooldown between 2 attacks for range attack.")]
    public float attackRangeCd = 2f;
    private float attackRangeCd_ref;
    [Tooltip("Attack duration.")]
    public float attackDuration = 0.1f;
    private float attackTime = 0f;
    [Tooltip("Collider 2D of the hit detection.")]
    public Collider2D attackTrigger;
    public Projectile projectile;
    [Tooltip("Projectile shot by the entity")]
    private int lifeMax;
    private int ref_life;
    private int relationFlint = -1;
    private int relationConner = -1;
    private int relationKya = -1;
    private int relationSaria = -1;

    [Tooltip("Health ba rof the unity")]
    public Scrollbar bar;
    [Tooltip("Health ba rof the unity")]
    public Text gold_text;
    private int gold;
    [Tooltip("Health ba rof the unity")]
    public Text xp_text;
    public Text level_text;
    private int xps;
    private int level;
    private int xpsMax;

    private void Reset()
    {
        xpsMax = 50;
        gold = 0;
        xps = 0;
        life = ref_life;
        lifeMax = ref_life;
        Vector2 posIni = new Vector2(0.0f, 0.0f);
        transform.position = posIni;
        level = 1;
        this.bar.size = (((float)this.life) / ((float)this.lifeMax));
        attackCd = attackCd_ref;
        attackRangeCd = attackRangeCd_ref;
        damages = damage_ref;
        if (xp_text != null)
            xp_text.text = "0 / 100";
        if (gold_text != null)
            gold_text.text = "0";
        if (level_text != null)
            level_text.text = "Level 1";
    }

    private void Awake()
    {
        attackTrigger.enabled = false;
        if (xp_text != null)
            xp_text.text = "0 / 100";
        if (gold_text != null)
            gold_text.text = "0";
        if (level_text != null)
            level_text.text = "Level 1";
    }
    // Use this for initialization
    void Start ()
    {
        switch (this.team)
            {
            case Team.Flint:
                color = new Color(40.0f / 255.0f, 200.0f / 255.0f, 120.0f / 255.0f);
                break;
            case Team.Conner:
                color = new Color(255.0f / 255.0f, 215.0f / 255.0f, 0.0f / 255.0f);
                break;
            case Team.Kya:
                color = new Color(165.0f / 255.0f, 82.0f / 255.0f, 82.0f / 255.0f);
                break;
            case Team.Saria:
                color = new Color(200.0f / 255.0f, 255.0f / 45.0f, 255.0f / 255.0f);
                break;
            default:
                color = new Color(0.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);
                break;
            }
        ref_life = life;
        lifeMax = life;
        GetComponent<SpriteRenderer>().color = color;
        gold = 0;
        xps = 0;
        level = 1;
        xpsMax = 50;
        attackCd_ref = attackCd;
        attackRangeCd_ref = attackRangeCd;
        damage_ref = damages;
    }
	// Update is called once per frame
	void Update () {
        if (immuneTime > 0.0f) // Immunité lors d'un coup reçu.
        {
            if (color_modified)
            {
                color_modified = false;
                GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1F);
            }
            else
            {
                color_modified = true;
                GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0.5F);
            }
            immuneTime -= Time.deltaTime;
            if (immuneTime <= 0.0f)
            {
                GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1F);
            }
        }
        if (attacking)
        {
            if (attackTrigger.enabled && (attackTime <= (attackCd - attackDuration)))
            {
                attackTrigger.enabled = false;
            }
            if (attackTime >= 0f)
            {
                attackTime -= Time.deltaTime;
            }
            else
            {
                attacking = false;
            }
        }
    }

    private void levelUp()
    {
        int compUp = Random.Range(1, 5);
        switch (compUp)
        {
            case 1:
                lifeMax += 100;
                break;

            case 2:
                damages += 15;
                break;

            case 3:
                attackCd -= 0.2f;
                if (attackCd < 0.0f)
                    attackCd = 0.0f;
                break;

            case 4:
                attackRangeCd -= 0.2f;
                if (attackRangeCd < 0.0f)
                    attackRangeCd = 0.0f;
                break;

            case 5:
                gold += 300;
                break;
        }
        life = lifeMax;
        level++;
    }

    public void Damaged(Character charac)
    {
        int dmg = charac.getDamage();
        Team team = charac.getTeam();
        if (team != this.team)
        {
            if (immuneTime <= 0.0f)
            {
                life -= dmg;
                if (this.bar)
                {
                    this.bar.size = (((float)this.life) / ((float)this.lifeMax));
                }
                if (life <= 0)
                {
                    if (this.isPlayer())
                    {
                        Reset();
                    }
                    if (charac.isPlayer())
                    {
                        if (this.team == Team.Conner && charac.getRelationConner() > -2)
                            charac.setRelationConner(2);
                        if (this.team == Team.Flint && charac.getRelationFlint() > -2)
                            charac.setRelationFlint(2);
                        if (this.team == Team.Saria && charac.getRelationSaria() > -2)
                            charac.setRelationSaria(2);
                        if (this.team == Team.Kya && charac.getRelationKya() > -2)
                            charac.setRelationKya(2);
                        int nbCoins = Random.Range(minCoin, maxCoin); // Generate coins...
                        for (int i = 0; i < nbCoins; i++)
                        {
                            float randomX = Random.Range(0.0f, 2.0f) - 1.0f;
                            float randomY = Random.Range(0.0f, 2.0f) - 1.0f;
                            Vector2 posCoin = new Vector2(transform.position.x + randomX, transform.position.y + randomY);
                            Instantiate(coin, posCoin, transform.rotation);
                        }
                        charac.setGold(charac.getGold() + nbCoins);
                        int nbXp = Random.Range(minXp, maxXp); // ... and xp
                        for (int i = 0; i < nbXp; i++)
                        {
                            float randomX = Random.Range(0.0f, 2.0f) - 1.0f;
                            float randomY = Random.Range(0.0f, 2.0f) - 1.0f;
                            Vector2 posXp = new Vector2(gameObject.transform.position.x + randomX, transform.position.y + randomY);
                            Instantiate(xp, posXp, transform.rotation);
                        }
                        charac.setXps(charac.getXps() + nbXp);
                        if (charac.getXps() > charac.getXpsMax())
                        {
                            charac.setXps(charac.getXpsMax() - charac.getXps());
                            charac.setXpsMax((int)(getXpsMax() * 2f));
                            charac.levelUp();
                        }
                        string displayXp = charac.getXps().ToString();
                        displayXp += "/";
                        displayXp += charac.getXpsMax().ToString();
                        charac.setXpText(displayXp);
                        charac.setGoldText(charac.getGold().ToString());
                        charac.setLevel(charac.getLevel());
                    }
                    if (!this.isPlayer())
                        Destroy(gameObject);
                }
                immuneTime = immunityTime;
            }
        }
    }

    void Healed(Character charac)
    {
        int dmg = charac.getDamage();
        life += dmg;
        if (this.bar != null)
        {
            this.bar.size = ((float)this.life) / ((float) this.lifeMax);
        }
        if (life > lifeMax)
        {
            life = lifeMax;
        }
    }

    public void Attack()
    {
        if (!attacking)
        {
            //Need to swap two tie the player rotation for the attack
            gameObject.transform.rotation = Quaternion.Inverse(gameObject.transform.rotation);
            gameObject.transform.rotation = Quaternion.Inverse(gameObject.transform.rotation);
            attacking = true;
            attackTime = attackCd;
            attackTrigger.enabled = true;
        }
    }

    public void Heal()
    {
        if (!attacking)
        {
            //Need to swap two tie the player rotation for the attack
            gameObject.transform.rotation = Quaternion.Inverse(gameObject.transform.rotation);
            gameObject.transform.rotation = Quaternion.Inverse(gameObject.transform.rotation);
            attacking = true;
            attackTime = attackCd;
            attackTrigger.enabled = true;
        }
    }

    public void RangeAttack(Vector2 targetPos)
    {
        if (!attacking)
        {
            Vector2 targetDir = (targetPos - ((Vector2)transform.position));
            GameObject obj;
            Rigidbody2D rb;
            obj = Instantiate(projectile.gameObject, transform.position, transform.rotation) as GameObject;
            obj.GetComponent<Projectile>().setLauncher(this);
            rb = obj.GetComponent<Rigidbody2D>();
            rb.velocity = targetDir.normalized * 20.0f;
            attacking = true;
            attackTime = attackRangeCd;
        }
    }

    public void MassRangeAttack(Vector2 targetPos)
    {
        if (!attacking)
        {
            float angle = -60.0f;
            while (angle <= 60.0f)
            {
                Vector2 targetDir = Quaternion.AngleAxis(angle, Vector3.back) * (targetPos - ((Vector2)transform.position));
                GameObject obj = Instantiate(projectile.gameObject, transform.position, transform.rotation) as GameObject;
                obj.GetComponent<Projectile>().setLauncher(this);
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                rb.velocity = targetDir.normalized * 5.0f;
                attacking = true;
                attackTime = attackRangeCd;
                angle = angle + 30.0f;
            }
        }
    }

    public GameObject CharacterSpawn(Vector2 position)
    {
        return (Instantiate(gameObject, position, transform.rotation));
    }

    public float getSpeed()
    {
        return (speed);
    }
    public int getLifePurcentage()
    {
        return ((life * 100) / lifeMax);
    }
    public bool isPlayer()
    {
        return (player);
    }
    public Team getTeam()
    {
        return (team);
    }
    public int getDamage()
    {
        return (damages);
    }
    public bool isAttacking()
    {
        return (this.attacking);
    }
    public bool isInvincible()
    {
        return (this.immuneTime > 0.0f);
    }
    public int getLifeMax()
    {
        return (this.lifeMax);
    }
    public MinonType getMinionType()
    {
        return (minion);
    }
    public void setGoldText(string text)
    {
        if (gold_text != null)
            gold_text.text = text;
    }
    public void setXpText(string text)
    {
        if (xp_text != null)
            xp_text.text = text;
    }
    public void setLevelText(string text)
    {
        string txtLevel;
        txtLevel = "Level ";
        if (level_text != null)
            txtLevel += text;
        level_text.text = txtLevel;
    }
    public int getRelationFlint() { return relationFlint; }
    public int getRelationSaria() { return relationSaria; }
    public int getRelationKya() { return relationKya; }
    public int getRelationConner() { return relationConner; }
    public void setRelationFlint(int value) { relationFlint += value; }
    public void setRelationSaria(int value) { relationSaria += value; }
    public void setRelationKya(int value) { relationKya += value; }
    public void setRelationConner(int value) { relationConner += value; }
    public void setGold(int value) { gold = value; }
    public void setXps(int value) { xps = value; }
    public void setXpsMax(int value) { xpsMax = value; }
    public void setLevel(int value) { level = value; }
    public int getGold() { return (gold); }
    public int getXps() { return (xps); }
    public int getXpsMax() { return (xpsMax); }
    public int getLevel() { return (level); }
}
