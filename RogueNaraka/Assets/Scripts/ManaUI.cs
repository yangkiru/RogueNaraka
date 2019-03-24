using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    public Image fillImg;
    public Image caseImg;
    public Animator animator;
    public Transform cashedTransform;
    public float current;
    public float goal;
    public bool isOver;
    float t = 1;

    public void SetMana(SkillData data, int level = 1)
    {
        float use = data.manaCost + (data.levelUp.manaCost *  (level - 1));
        float mp = 0;
        if (BoardManager.instance.player && !BoardManager.instance.player.deathable.isDeath)
            mp = BoardManager.instance.player.stat.mp;
        else
        {
            Stat randomStat = Stat.DataToStat("randomStat");
            if (randomStat != null)
                mp = randomStat.mp;
            else
                mp = Stat.DataToStat().mp;
        }

        SetMana(mp, use);
    }

    private void OnEnable()
    {
        animator.SetBool("isOver", isOver);
    }
    private void OnDisable()
    {
        current = 0;
        goal = 0;
    }

    public void SetMana(float max, float use)
    {
        Debug.Log(use + " " + max + " " + use / max);
        float result = use / max;
        goal = Mathf.Clamp01(result);
        t = 0;
        isOver = result > 1;

        if (gameObject.activeSelf)
            animator.SetBool("isOver", isOver);

        gameObject.SetActive(true);
    }

    [ContextMenu("Test")]
    public void Test()
    {
        SetMana(2,1);
    }

    private void LateUpdate()
    {
        t += Time.unscaledDeltaTime;
        if (t > 1)
            t = 1;
        if (current == goal)
            t = 0;
        current = Mathf.Lerp(current, goal, t);
        fillImg.fillAmount = current;
    }
}
