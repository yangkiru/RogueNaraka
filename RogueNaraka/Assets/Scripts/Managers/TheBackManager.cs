using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class TheBackManager : MonoBehaviour
{    // Start is called before the first frame update
    void Start()
    {
        var bro = Backend.Initialize(true);
        if(bro.IsSuccess())
        {
            Debug.Log("초기화 성공");
            Debug.Log( Backend.Utils.GetGoogleHash() );
            Debug.Log("구글 해시");
            //CustomSignUp();
        }
        else{
            Debug.LogError("초기화 실패");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Backend.AsyncPoll();
        
    }
}
