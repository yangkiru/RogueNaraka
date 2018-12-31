using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapResize : MonoBehaviour
{

    public PolygonCollider2D map;
    public BoardManager boardManager;

    private void OnDrawGizmosSelected()
    {
        Vector2[] temp = new Vector2[4];
        temp[0] = new Vector2(boardManager.boardRange[1].x, boardManager.boardRange[1].y);
        temp[1] = new Vector2(boardManager.boardRange[0].x, boardManager.boardRange[1].y);
        temp[2] = new Vector2(boardManager.boardRange[0].x, boardManager.boardRange[0].y);
        temp[3] = new Vector2(boardManager.boardRange[1].x, boardManager.boardRange[0].y);

        map.points = temp;
    }
}