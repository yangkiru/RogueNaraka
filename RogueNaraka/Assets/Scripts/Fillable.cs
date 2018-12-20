using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.UnitScripts;

public class Fillable : MonoBehaviour
{
    public bool isPlayer;
    public Canvas canvas;
    public float current = 100;
    public float goal = 100;
    public Image img;
    public Unit unit;
    public TYPE type;
    public Vector2 move;
    public RectTransform rectTransform;

    private float t = 1;

    public enum TYPE { HEALTH, MANA }

    void Awake()
    {
        if (!isPlayer)
        {
            Vector2 pos = gameObject.transform.position;  // get the game object position
            Vector2 viewportPoint = Camera.main.WorldToViewportPoint(pos);  //convert game object position to VievportPoint

            // set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)
            rectTransform.anchorMin = viewportPoint;
            rectTransform.anchorMax = viewportPoint;
        }
    }
    void Update()
    {
        if (!isPlayer && unit)
        {
            transform.parent.position = unit.transform.position + (Vector3)move;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (unit)
        {
            if (float.IsNaN(current))
            {
                switch (type)
                {
                    case TYPE.HEALTH: current = unit.hpable.currentHp; break;
                    case TYPE.MANA: current = unit.mpable.currentMp; break;
                }
            }
            switch (type)
            {
                case TYPE.HEALTH: goal = unit.hpable.currentHp / unit.hpable.maxHp; break;
                case TYPE.MANA: goal = unit.mpable.currentMp / unit.mpable.maxMp; break;
            }
            t += Time.deltaTime;
            if (t > 1)
                t = 1;
            if (current == goal)
                t = 0;
            current = Mathf.Lerp(current, goal, t);
            img.fillAmount = current;
        }
        else
        {
            unit = BoardManager.instance.player;
        }
    }
}
