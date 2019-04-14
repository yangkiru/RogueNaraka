using System;
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
        const float CLAMP_BOARD_SIZE_X = CHECK_ADDED_BOARD_SIZE_X * 1.5f;
        const float CLAMP_BOARD_SIZE_Y = CHECK_ADDED_BOARD_SIZE_Y * 1.5f;
        [SerializeField]
        Unit unit;

        [SerializeField]
        Transform cachedTransform;

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

        public float AccelerationRate { get { return accelerationRate; } set { accelerationRate = value; } }
        private float accelerationRate;
        public float DecelerationRate { get { return decelerationRate; } set { decelerationRate = value; } }
        private float decelerationRate;
        private float curSpeed;
        public float CurSpeed { get { return this.curSpeed; } }

        private float decelerationDistance;

        private Vector2 moveDir;
        public Vector2 MoveDir { get { return this.moveDir; } }

        private enum MOVE_STATE {STOP, ACCELERATE, MOVE, DECELERATE, END};
        [SerializeField]
        private MOVE_STATE moveState;
        //

        void Reset()
        {
            unit = GetComponent<Unit>();
            cachedTransform = transform;
        }

        public void Init(UnitData data)
        {
            SetSpeed(data.moveSpeed);
            if(unit.data.accelerationRate == 0.0f) {
                this.accelerationRate = 0.5f;
            } else {
                this.accelerationRate = unit.data.accelerationRate;
            }

            if (unit.data.decelerationRate == 0.0f) {
                this.decelerationRate = 1.0f;
            }
            else {
                this.decelerationRate = unit.data.decelerationRate;
            }
            moveState = MOVE_STATE.STOP;
            curSpeed = 0;
        }

        public void SetSpeed(float speed)
        {
            unitSpeed = speed;
        }
        
        /// <summary>목적지를 설정하고, MoveState를 ACCELERATE상태로 바꿉니다.</summary>
        public void SetDestination(Vector3 pos, Action<bool> callback = null)
        {
            this.destination = BoardManager.instance.ClampToBoard(pos, CLAMP_BOARD_SIZE_X, CLAMP_BOARD_SIZE_Y);//목적지 보드 제한
            this.onArrivedCallback = callback;
            this.moveState = MOVE_STATE.ACCELERATE;
            unit.animator.SetBool("isWalk", true);
            decelerationDistance = Mathf.Max(0.1f, Mathf.InverseLerp(Vector2.Distance(cachedTransform.position, pos), 0, decelerationRate));
        }

        /// <summary>유닛을 즉시 멈춥니다.</summary>
        public void Stop() {
            this.curSpeed = 0.0f;
            this.moveState = MOVE_STATE.STOP;
            unit.animator.SetBool("isWalk", false);
            this.moveDir = new Vector2(0.0f, 0.0f);
            if (this.onArrivedCallback != null)
            {
                this.onArrivedCallback(false);
            }
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
            if (unit.isStun)
            {
                if (this.curSpeed > 0)
                    this.curSpeed -= this.decelerationRate * this.speed * TimeManager.Instance.FixedDeltaTime * (1 + factor);
                else
                    this.curSpeed = 0;
                return;
            }
            switch(this.moveState) {
                case MOVE_STATE.STOP:
                    return;
                case MOVE_STATE.ACCELERATE:
                    Accelerate();
                break;
                case MOVE_STATE.MOVE:
                    if (this.speed > this.curSpeed)
                        this.moveState = MOVE_STATE.ACCELERATE;
                break;
                case MOVE_STATE.DECELERATE:
                    DecelerateForArrive();
                break;
                default:
                    Debug.LogError("MoveState is Incorrect!");
                return;
            }
            /* 목적지에 가까워졌을 때, 방향이 바뀌는 것을 방지하기 위해 조건문 추가 */
            if (this.moveState != MOVE_STATE.DECELERATE)
                this.moveDir = this.destination.SubtractVector3FromVector2(this.cachedTransform.position);
            float distanceToDest = moveDir.sqrMagnitude;
            this.moveDir.Normalize();
            this.cachedTransform.Translate(moveDir * this.curSpeed * TimeManager.Instance.FixedDeltaTime);

            //if (this.moveState != MOVE_STATE.DECELERATE && this.moveState != MOVE_STATE.STOP &&
            //    distanceToDest <= MathHelpers.DecelerateDistance(this.decelerationRate, this.curSpeed))
            //{
            //    this.moveState = MOVE_STATE.DECELERATE;
            //}
            if (this.moveState != MOVE_STATE.DECELERATE && this.moveState != MOVE_STATE.STOP &&
                distanceToDest <= decelerationDistance)
            {
                this.moveState = MOVE_STATE.DECELERATE;
            }
        }
        private void CheckUnitInBoard() {
            Vector2 changedPos = this.cachedTransform.position;
            
            bool isOut = false;
            if(changedPos.x < BoardManager.instance.boardRange[0].x + CHECK_ADDED_BOARD_SIZE_X) {
                changedPos.x = BoardManager.instance.boardRange[0].x + CHECK_ADDED_BOARD_SIZE_X;
                isOut = true;
            } else if(changedPos.x > BoardManager.instance.boardRange[1].x - CHECK_ADDED_BOARD_SIZE_X) {
                changedPos.x = BoardManager.instance.boardRange[1].x - CHECK_ADDED_BOARD_SIZE_X;
                isOut = true;
            }
            if(changedPos.y < BoardManager.instance.boardRange[0].y + CHECK_ADDED_BOARD_SIZE_Y) {
                changedPos.y = BoardManager.instance.boardRange[0].y + CHECK_ADDED_BOARD_SIZE_Y;
                isOut = true;
            } else if(changedPos.y > BoardManager.instance.boardRange[1].y - CHECK_ADDED_BOARD_SIZE_Y) {
                changedPos.y = BoardManager.instance.boardRange[1].y - CHECK_ADDED_BOARD_SIZE_Y;
                isOut = true;
            }

            //if (isOut && this.moveState != MOVE_STATE.STOP)
            //{
            //    this.moveState = MOVE_STATE.DECELERATE;
            //}
            this.cachedTransform.position = changedPos;
        }

        private void Accelerate() {
            this.curSpeed += this.accelerationRate * this.speed * TimeManager.Instance.FixedDeltaTime * (1 + factor);
            if(this.curSpeed >= this.speed) {
                this.curSpeed = this.speed;
                this.moveState = MOVE_STATE.MOVE;
            }
        }

        private void DecelerateForArrive() {
            this.curSpeed -= this.decelerationRate * (this.curSpeed + this.speed) * 0.5f * TimeManager.Instance.FixedDeltaTime * (1 + factor);
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
