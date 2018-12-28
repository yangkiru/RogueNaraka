using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class IsometricSpriteRenderer : MonoBehaviour {
    private float pos;
    private float _pos;
    public int add;
    new SpriteRenderer renderer;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        //StartCoroutine(SetOrder());
    }

    void OnDisable()
    {
        add = 0;
    }

    IEnumerator SetOrder ()
    {
        while (true)
        {
            //pos = transform.position.y;
            //if (pos != _pos)
            //{
            //    //
            //}
            //_pos = pos;
            yield return new WaitForSeconds(0.5f);
        }
	}
}
