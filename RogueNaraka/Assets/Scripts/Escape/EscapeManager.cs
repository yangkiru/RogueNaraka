using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.SingletonPattern;

namespace RogueNaraka.Escapeable
{
    public class EscapeManager : MonoSingleton<EscapeManager>
    {
        public LinkedStack<Escapeable> Stack { get { return stack; } }

        private LinkedStack<Escapeable> stack = new LinkedStack<Escapeable>();
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (stack.Count != 0)
                    stack.Peek().OnEscape();
            }
        }

        public void SetTop(Escapeable item)
        {
            if(stack.Contains(item))
                stack.SetTop(item);
        }

        [ContextMenu("CheckStack")]
        public void CheckStack()
        {
            string result = string.Empty;
            for (LinkedListNode<Escapeable> n = this.stack.First; n != null; n = n.Next)
                result = string.Format("{0} {1}", result, n.Value.name);
            Debug.Log(result);
        }
    }

    public class LinkedStack<T> : LinkedList<T>
    {
        public void Push(T item)
        {
            this.AddLast(item);
        }

        public T Pop()
        {
            T result = this.Last.Value;
            this.RemoveLast();
            return result;
        }

        public T Peek()
        {
            return this.Last.Value;
        }

        public new T Remove(T item)
        {
            LinkedListNode<T> resultNode = this.FindLast(item);
            T result = resultNode.Value;
            this.Remove(resultNode);
            return result;
        }

        public void SetTop(T item)
        {
            Remove(item);
            Push(item);
        }
    }
}
