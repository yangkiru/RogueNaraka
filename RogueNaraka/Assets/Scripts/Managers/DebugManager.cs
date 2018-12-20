using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {

    public bool reset;
    public bool setStage;
    public int stage;
    public bool killPlayer;
    public bool killEnemies;
    public bool setShopStage;
    public int shopStage;
    public bool levelUp;
    private void OnValidate()
    {
        if (reset)
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Reset");
            reset = false;
        }
        if (setStage)
        {
            setStage = false;
            stage = 0;
            PlayerPrefs.SetInt("stage", stage);
        }
        if (killPlayer && BoardManager.instance && BoardManager.instance.player)
        {
            BoardManager.instance.player.Kill();
            killPlayer = false;
        }
        else if (killPlayer)
            killPlayer = false;
        if (killEnemies && BoardManager.instance)
        {
            for (int i = 0; i < BoardManager.instance.enemies.Count; i++)
            {
                BoardManager.instance.enemies[i].Kill();
            }
            killEnemies = false;
        }
        else if (killEnemies)
            killEnemies = false;
        if(setShopStage)
        {
            PlayerPrefs.SetInt("shopStage", shopStage);
            setShopStage = false;
            shopStage = 0;
        }
        if(levelUp)
        {
            PlayerPrefs.SetInt("isLevelUp", 1);
            levelUp = false;
        }
    }
}
