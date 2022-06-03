using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY) {
        List<Vector2Int> arr = new List<Vector2Int>();
        //RIGHT
        if(currentX + 1 < tileCountX) {
            if(board[currentX + 1, currentY] == null) {
                arr.Add(new Vector2Int(currentX + 1, currentY));
            } else if(board[currentX + 1, currentY].team != team) {
                arr.Add(new Vector2Int(currentX + 1, currentY));
            }
            if(currentY + 1 < tileCountY) {
            if(board[currentX + 1, currentY + 1] == null) {
                arr.Add(new Vector2Int(currentX + 1, currentY + 1));
            } else if(board[currentX + 1, currentY + 1].team != team) {
                arr.Add(new Vector2Int(currentX + 1, currentY + 1));
            } }
            if(currentY - 1 >= 0) {
            if(board[currentX + 1, currentY - 1] == null) {
                arr.Add(new Vector2Int(currentX + 1, currentY - 1));
            } else if(board[currentX + 1, currentY - 1].team != team) {
                arr.Add(new Vector2Int(currentX + 1, currentY - 1));
            } }
        }
        //LEFT
        if(currentX - 1 >= 0) {
            if(board[currentX - 1, currentY] == null) {
                arr.Add(new Vector2Int(currentX - 1, currentY));
            } else if(board[currentX - 1, currentY].team != team) {
                arr.Add(new Vector2Int(currentX - 1, currentY));
            }
            if(currentY + 1 < tileCountY) {
            if(board[currentX - 1, currentY + 1] == null) {
                arr.Add(new Vector2Int(currentX - 1, currentY + 1));
            } else if(board[currentX - 1, currentY + 1].team != team) {
                arr.Add(new Vector2Int(currentX - 1, currentY + 1));
            } }
            if(currentY - 1 >= 0) {
            if(board[currentX - 1, currentY - 1] == null) {
                arr.Add(new Vector2Int(currentX - 1, currentY - 1));
            } else if(board[currentX - 1, currentY - 1].team != team) {
                arr.Add(new Vector2Int(currentX - 1, currentY - 1));
            } }
        }
        //UP
        if(currentY + 1 < tileCountY) {
        if(board[currentX, currentY + 1] == null) {
            arr.Add(new Vector2Int(currentX, currentY + 1));
        } else if(board[currentX, currentY + 1].team != team) {
            arr.Add(new Vector2Int(currentX, currentY + 1));
        } }
        //DOWN
        if(currentY - 1 >= 0) {
        if(board[currentX, currentY - 1] == null) {
            arr.Add(new Vector2Int(currentX, currentY - 1));
        } else if(board[currentX, currentY - 1].team != team) {
            arr.Add(new Vector2Int(currentX, currentY - 1));
        } }
        return arr;
    }

    public override HitResult TestHit(Direction hitDirection) {
        return new HitResult(direction: hitDirection, hitresult: Result.Win);
    }
}
