﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.SingletonPattern;

public class EscapeManager : MonoBehaviour
{
    bool isExit;
    public static EscapeManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Stack<Escapeable> Stack { get { return stack; } }
    private Stack<Escapeable> stack = new Stack<Escapeable>();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (stack.Count != 0)
                stack.Peek().OnEscape();
        }
    }
}
