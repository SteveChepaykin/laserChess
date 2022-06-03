using System.Collections.Generic;
using UnityEngine;

public class Switch : ChessPiece
{
    public override HitResult TestHit(Direction hitDirection) {
        if(currentDirection == Direction.Up) {
            if(hitDirection == Direction.Up) {
                return new HitResult(direction: Direction.Left, hitresult: Result.Reflect);
            } else if(hitDirection == Direction.Right) {
                return new HitResult(direction: Direction.Down, hitresult: Result.Reflect);
            } else return new HitResult(direction: hitDirection, hitresult: Result.Hit);
        }
        else if(currentDirection == Direction.Right) {
            if(hitDirection == Direction.Right) {
                return new HitResult(direction: Direction.Up, hitresult: Result.Reflect);
            } else if(hitDirection == Direction.Down) {
                return new HitResult(direction: Direction.Left, hitresult: Result.Reflect);
            } else return new HitResult(direction: hitDirection, hitresult: Result.Hit);
        }
        else if(currentDirection == Direction.Down) {
            if(hitDirection == Direction.Down) {
                return new HitResult(direction: Direction.Right, hitresult: Result.Reflect);
            } else if(hitDirection == Direction.Left) {
                return new HitResult(direction: Direction.Up, hitresult: Result.Reflect);
            } else return new HitResult(direction: hitDirection, hitresult: Result.Hit);
        }
        else if(currentDirection == Direction.Left) {
            if(hitDirection == Direction.Left) {
                return new HitResult(direction: Direction.Down, hitresult: Result.Reflect);
            } else if(hitDirection == Direction.Up) {
                return new HitResult(direction: Direction.Right, hitresult: Result.Reflect);
            } else return new HitResult(direction: hitDirection, hitresult: Result.Hit);
        } else {
            return new HitResult(direction: hitDirection, hitresult: Result.Hit);
        }
    }
}
