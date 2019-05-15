using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCollider : MonoBehaviour
{
    public new SpriteRenderer renderer;
    public PolygonCollider2D coll;

    int last = -1;

    private void Reset()
    {
        renderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<PolygonCollider2D>();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (renderer.sprite == null)
            return;
        int current = renderer.sprite.GetInstanceID();
        if (last != current)
        {
            //for (int i = 0; i < coll.pathCount; i++) coll.SetPath(i, null);
            coll.pathCount = renderer.sprite.GetPhysicsShapeCount();

            List<Vector2> path = new List<Vector2>();
            for (int i = 0; i < coll.pathCount; i++)
            {
                path.Clear();
                renderer.sprite.GetPhysicsShape(i, path);
                coll.SetPath(i, path.ToArray());
            }
            last = current;
        }
    }
}
