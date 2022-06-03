using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType {
    None,
    Switch,
    Double,
    Defender,
    Cannon,
    King,
    Laser
}

public class ChessPiece : MonoBehaviour
{
    public ChessPieceType type;
    public int team;
    public int currentX;
    public int currentY;
    public Direction currentDirection;
    private Vector3 desiredPosition;
    private Vector3 desiredScale = new Vector3(0.17f, 0.17f, 0);
    private Vector3Int desiredRotation;

    // void Start() {
    //     transform.rotation = Quaternion.Euler(rotation);
    // }

    // public enum Direction {
    //     Forward = 0,
    //     Backward = 180,
    //     Left = -90,
    //     Right = 90
    // }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation.x, desiredRotation.y, desiredRotation.z), Time.deltaTime * 10);
    }

    public virtual void SetPosition(Vector3 pos, bool force = false) {
        desiredPosition = pos;
        if(force) {
            transform.position = desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false) {
        desiredScale = scale;
        if(force) {
            transform.localScale = desiredScale;
        }
    }

    public virtual void SetRotation(Direction rotation, bool force = false) {
        currentDirection = rotation;
        desiredRotation = new Vector3Int(0, 0, (int)rotation);
        if(force) {
            transform.rotation = Quaternion.Euler(desiredRotation.x, desiredRotation.y, desiredRotation.z);
        }
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY) {
        List<Vector2Int> arr = new List<Vector2Int>();
        //RIGHT
        if(currentX + 1 < tileCountX) {
            if(board[currentX + 1, currentY] == null) {
                arr.Add(new Vector2Int(currentX + 1, currentY));
            }
            if(currentY + 1 < tileCountY) {
                if(board[currentX + 1, currentY + 1] == null) {
                    arr.Add(new Vector2Int(currentX + 1, currentY + 1));
                }
            }
            if(currentY - 1 >= 0) {
                if(board[currentX + 1, currentY - 1] == null) {
                    arr.Add(new Vector2Int(currentX + 1, currentY - 1));
                }
            }
        }
        //LEFT
        if(currentX - 1 >= 0) {
            if(board[currentX - 1, currentY] == null) {
                arr.Add(new Vector2Int(currentX - 1, currentY));
            } 
            if(currentY + 1 < tileCountY) {
                if(board[currentX - 1, currentY + 1] == null) {
                    arr.Add(new Vector2Int(currentX - 1, currentY + 1));
                } 
            }
            if(currentY - 1 >= 0) {
                if(board[currentX - 1, currentY - 1] == null) {
                    arr.Add(new Vector2Int(currentX - 1, currentY - 1));
                } 
            }
        }
        //UP
        if(currentY + 1 < tileCountY) {
            if(board[currentX, currentY + 1] == null) {
                arr.Add(new Vector2Int(currentX, currentY + 1));
            }
        }
        //DOWN
        if(currentY - 1 >= 0) {
            if(board[currentX, currentY - 1] == null) {
                arr.Add(new Vector2Int(currentX, currentY - 1));
            }
        }
        return arr;
    }

    public virtual HitResult TestHit(Direction hitDirection) {
        return new HitResult(direction: hitDirection, hitresult: Result.Hit);
    }
}
