using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FB.Init(this.OnInitComplete);
        
    }

    private void OnInitComplete()
    {
        var tutParams = new Dictionary<string, object>();
        tutParams[AppEventParameterName.ContentID] = "Facebook SDK init";
        tutParams[AppEventParameterName.Description] = "Facebook SDK init!";
        tutParams[AppEventParameterName.Success] = "1";

        FB.LogAppEvent (
            AppEventName.CompletedTutorial,
            parameters: tutParams
        );
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
