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
                if(clickPoint.x < BoardManager.instance.boardRange[0].x || clickPoint.x > BoardManager.instance.boardRange[1].x ||
                    clickPoint.y < BoardManager.instance.boardRange[0].y || clickPoint.y > BoardManager.instance.boardRange[1].y
                ) return;
                moveable.SetDestination(clickPoint);
            }
        }
    }
}