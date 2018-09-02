using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillShowCase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public SkillManager skillManager
    {
        get { return SkillManager.instance; }
    }
    
    public int position;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skillManager.isDragable && IsSelected())
        {
            skillManager.DrawLine(position);
            StartCoroutine(SetLine(true));
        }
    }

    private IEnumerator SetLine(bool value)
    {
        yield return null;
        skillManager.SetLine(value);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillManager.isDragable && IsSelected())
            skillManager.DrawLine(position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        skillManager.SetLine(false);
        if (skillManager.isDragable && IsSelected())
        {
            var pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            if(raycastResults.Count > 0)
            {
                if (raycastResults[0].gameObject.CompareTag("Skill"))
                {
                    int showCaseId = skillManager.GetId(position);
                    Skill target = raycastResults[0].gameObject.GetComponent<Skill>();
                    if (!skillManager.HasSkill(showCaseId))
                        skillManager.EquipSkill(position, target.position);
                    else
                    {
                        if(target.data.id == showCaseId)
                        {
                            target.LevelUp(1);
                            skillManager.SetSkillPnl(false);
                        }
                    }
                }
            }
        }
    }

    public bool IsSelected()
    {
        int selected = skillManager.selected;
        int length = skillManager.showCase.Length;
        return selected == position || (selected + 1) % length == position || (selected + 2) % length == position;
    }
}
