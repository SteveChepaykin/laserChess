using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Vector2Int currentPosition;
    public List<Vector2Int> allCoords;
    public Vector2Int nextLookUp;
    public Direction currentDirection;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // private void PositionSinglePiece(int x, int y, bool force = false) {
    //     chesspieces[x, y].currentX = x;
    //     chesspieces[x, y].currentY = y;
    //     chesspieces[x, y].SetPosition(getTileCenter(x, y), force);   
    // }
    // private Vector3 getTileCenter(int x, int y) {
    //     return new Vector3(x * tileSize, y * tileSize, -yOffset) - bounds + new Vector3(tileSize/2, tileSize/2, 0);
    // }
}
