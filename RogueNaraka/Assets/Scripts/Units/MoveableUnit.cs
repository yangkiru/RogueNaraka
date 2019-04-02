﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.ExtensionMethod;
using RogueNaraka.TimeScripts;

namespace RogueNaraka.UnitScripts
{
    public class MoveableUnit : MonoBehaviour
    {   
        const float CHECK_ADDED_BOARD_SIZE_X = 0.2f;
        const float CHECK_ADDED_BOARD_SIZE_Y = 0.2f;

        [SerializeField]
        Unit unit;

        private Vector2 destination;
        private Action<bool> onArrivedCallback;
        public Action<bool> OnArrivedCallback { get { return this.onArrivedCallback; } }

        public float speed {
            get {
                return unitSpeed * (1 + unit.stat.GetCurrent(STAT.SPD) * 0.1f) *
                    (1 + factor);
            }
        }
        float unitSpeed;
        public float factor;

        //추후 Stat으로 옮기고 변경
        private float accelerationRate;
        private float curSpeed;
        public float CurSpeed { get { return this.curSpeed; } }
        private Vector2 moveDir;
        public Vector2 MoveDir { get { return this.moveDir; } }

        private enum MOVE_STATE {STOP, ACCELERATE, MOVE, DECELERATE, END};
        private MOVE_STATE moveState;
        //

        void Reset()
        {
            unit = GetComponent<Unit>();
        }

        public void Init(UnitData data)
        {
            SetSpeed(data.moveSpeed);
            if(unit.data.accelerationRate == 0.0f) {
                this.accelerationRate = 0.2f;
            } else {
                this.accelerationRate = unit.data.accelerationRate;
            }
        }

        public void SetSpeed(float speed)
        {
            unitSpeed = speed;
        }
        
        /// <summary>목적지를 설정하고, MoveState를 ACCELERATE상태로 바꿉니다.</summary>
        public void SetDestination(Vector3 pos, Action<bool> callback = null)
        {
            this.destination = pos;
            this.onArrivedCallback = callback;
            this.moveState = MOVE_STATE.ACCELERATE;
            unit.animator.SetBool("isWalk", true);
        }

        /// <summary>유닛을 즉시 멈춥니다.</summary>
        public void Stop() {
            this.curSpeed = 0.0f;
            this.moveState = MOVE_STATE.STOP;
            unit.animator.SetBool("isWalk", false);
            this.moveDir = new Vector2(0.0f, 0.0f);
        }

        public void ForceToDecelerate(float _decelerationRate) {
            this.curSpeed = this.curSpeed * (1.0f - _decelerationRate);
        }

        void Update()
        {
            if(unit.targetable.target) {
                unit.animator.SetFloat("x", unit.targetable.direction.x);
                unit.animator.SetFloat("y", unit.targetable.direction.y);
            } else {
                if(this.moveDir.x != 0 || this.moveDir.y != 0) {
                    unit.animator.SetFloat("x", this.moveDir.x);
                    unit.animator.SetFloat("y", this.moveDir.y);
                }
            }
        }

        void FixedUpdate() {
            Move();
            CheckUnitInBoard();
        }

        private void Move() {
            switch(this.moveState) {
                case MOVE_STATE.STOP:
                    return;
                case MOVE_STATE.ACCELERATE:
                    Accelerate();
                break;
                case MOVE_STATE.MOVE:
                break;
                case MOVE_STATE.DECELERATE:
                    DecelerateForArrive();
                break;
                default:
                    Debug.LogError("MoveState is Incorrect!");
                return;
            }
            this.moveDir = this.destination.SubtractVector3FromVector2(this.transform.position);
            float distanceToDest = moveDir.sqrMagnitude;
            this.moveDir.Normalize();
            this.transform.Translate(moveDir * this.curSpeed * TimeManager.Instance.FixedDeltaTime);
            if(distanceToDest <= MathHelpers.DecelerateDistance(this.accelerationRate, this.curSpeed)) {
                this.moveState = MOVE_STATE.DECELERATE;
            }
        }
        private void CheckUnitInBoard() {
            Vector2 changedPos = this.transform.position;
            if(changedPos.x < BoardManager.instance.boardRange[0].x + CHECK_ADDED_BOARD_SIZE_X) {
                changedPos.x = BoardManager.instance.boardRange[0].x + CHECK_ADDED_BOARD_SIZE_X;
            } else if(changedPos.x > BoardManager.instance.boardRange[1].x - CHECK_ADDED_BOARD_SIZE_X) {
                changedPos.x = BoardManager.instance.boardRange[1].x - CHECK_ADDED_BOARD_SIZE_X;
            }
            if(changedPos.y < BoardManager.instance.boardRange[0].y + CHECK_ADDED_BOARD_SIZE_Y) {
                changedPos.y = BoardManager.instance.boardRange[0].y + CHECK_ADDED_BOARD_SIZE_Y;
            } else if(changedPos.y > BoardManager.instance.boardRange[1].y - CHECK_ADDED_BOARD_SIZE_Y) {
                changedPos.y = BoardManager.instance.boardRange[1].y - CHECK_ADDED_BOARD_SIZE_Y;
            }
            this.transform.position = changedPos;
        }

        private void Accelerate() {
            this.curSpeed += this.accelerationRate * this.speed * TimeManager.Instance.FixedDeltaTime;
            if(this.curSpeed >= this.speed) {
                this.curSpeed = this.speed;
                this.moveState = MOVE_STATE.MOVE;
            }
        }

        private void DecelerateForArrive() {
            this.curSpeed -= this.accelerationRate * this.speed * TimeManager.Instance.FixedDeltaTime;
            if(this.curSpeed <= 0.0f) {
                this.curSpeed = 0.0f;
                this.moveState = MOVE_STATE.STOP;
                unit.animator.SetBool("isWalk", false);
                if(this.onArrivedCallback != null) {
                    this.onArrivedCallback(true);
                }
                this.moveDir = new Vector2(0.0f, 0.0f);
            }
        }
    }
}
