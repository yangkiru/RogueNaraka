using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueNaraka.UnitScripts
{
    public class ClickToMoveableUnit : MonoBehaviour
    {
        private Unit unit;
        private MoveableUnit moveable;
        private Vector2 move = Vector2.zero;
        private Vector2 clickPoint;
        public bool IsActive;

        private void Awake(){
            unit = GetComponent<Unit>();
            moveable = GetComponent<MoveableUnit>();
        }

        private void Update(){
            if (IsActive && Time.timeScale != 0 && Input.GetMouseButtonDown(0)){
                clickPoint = GameManager.instance.GetMousePosition();
                if(!BoardManager.IsMouseInBoard()) return;
                unit.attackable.IsAttackable = false;
                moveable.SetDestination(clickPoint, EnableAttackable);
            }
        }

        private void EnableAttackable(bool isArrive){
            if (unit.attackable != null){
                unit.attackable.IsAttackable = true;
            }
        }
    }
}