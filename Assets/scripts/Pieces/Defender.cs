using System.Collections.Generic;
using UnityEngine;

public class Defender : ChessPiece
{
    public override HitResult TestHit(Direction hitDirection) {
        if(currentDirection == Direction.Up && hitDirection == Direction.Down) {
            return new HitResult(direction: hitDirection, hitresult: Result.Defend);
        } else if(currentDirection == Direction.Right && hitDirection == Direction.Left) {
            return new HitResult(direction: hitDirection, hitresult: Result.Defend);
        } else if(currentDirection == Direction.Down && hitDirection == Direction.Up) {
            return new HitResult(direction: hitDirection, hitresult: Result.Defend);
        } else if(currentDirection == Direction.Left && hitDirection == Direction.Right) {
            return new HitResult(direction: hitDirection, hitresult: Result.Defend);
        } else return new HitResult(direction: hitDirection, hitresult: Result.Hit);
    }
}
