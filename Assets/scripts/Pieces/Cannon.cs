using System.Collections.Generic;
using UnityEngine;

public class Cannon : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY) {
        List<Vector2Int> arr = new List<Vector2Int>(); 
        return arr;
    }

    public override HitResult TestHit(Direction hitDirection) {
        return new HitResult(direction: hitDirection, hitresult: Result.Defend);
    }
}
