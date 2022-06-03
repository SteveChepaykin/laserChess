using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum Direction {
    Up = 0,
    Right = -90,
    Down = 180,
    Left = 90
}

public enum Result {
    Hit,
    Win,
    Defend,
    Reflect
}

public class HitResult 
{
    public HitResult(Direction direction, Result hitresult) {
        Direction = direction;
        Hitresult = hitresult;
    }
    public Direction Direction;
    public Result Hitresult;
}

public class ChessBoard : MonoBehaviour
{
    [Header("Prefabs and Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMeterials;

    [Header("ART STUFF")]
    [SerializeField] private Material tileMaterial; 
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deadSize = 0.3f;
    [SerializeField] private float hoverSize = 1.2f;
    [SerializeField] private float deadSpacing = 0.3f;
    // [SerializeField] private float dragOffset = 1f;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject whiteUI;
    [SerializeField] private GameObject blackUI;

    private ChessPiece[,] chesspieces;
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    private List<ChessPiece> allLaser = new List<ChessPiece>();
    private ChessPiece currentluDragging;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private bool isWhiteTurn;
    private GameObject currentUI;
    private bool isRotating = false;

    private void Awake() {
        isWhiteTurn = true;
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update() {
        if(!currentCamera) {
            currentCamera = Camera.main;
            return;
        }

        currentUI = isWhiteTurn ? whiteUI : blackUI;

        RaycastHit info;
        if(Input.GetMouseButtonDown(0)) {
            
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Highlight"))) {
                Vector2Int hitpos = LookUpTileIndex(info.transform.gameObject); 
                if(currentluDragging != null && !isRotating) {
                    Vector2Int prevpos = new Vector2Int(currentluDragging.currentX, currentluDragging.currentY);
                    bool validmove = MoveTo(currentluDragging, hitpos.x, hitpos.y);
                    if(validmove) {
                        currentluDragging.SetPosition(getTileCenter(hitpos.x, hitpos.y));
                    }
                    // currentUI.transform.Find("rotate").gameObject.SetActive(false);
                    // SetActiveUI(false, false);
                    DeactivateAllUI();
                    currentluDragging = null;
                    RemoveHighlightTiles();
                } 
                if(chesspieces[hitpos.x, hitpos.y] != null && !isRotating) {  
                    if((chesspieces[hitpos.x, hitpos.y].team == 0 && isWhiteTurn) || (chesspieces[hitpos.x, hitpos.y].team == 1 && !isWhiteTurn)) {
                        currentluDragging = chesspieces[hitpos.x, hitpos.y];
                        availableMoves = currentluDragging.GetAvailableMoves(ref chesspieces, TILE_COUNT_X, TILE_COUNT_Y);
                        // currentUI.transform.Find((currentluDragging.type == ChessPieceType.Cannon) ? "fire" : "rotate").gameObject.SetActive(true);
                        DeactivateAllUI();
                        if(currentluDragging.type == ChessPieceType.Cannon) {
                            SetActiveUI(true, true);
                        } else {
                            SetActiveUI(false, true);
                        }
                        HighlightTiles();
                    }
                }
                // if(chesspieces[hitpos.x, hitpos.y] == null && currentluDragging != null) {
                    
                // }
            }
            // } else {
            //     if(currentHover != -Vector2Int.one) {
            //         tiles[currentHover.x, currentHover.y].layer = ContainsValidMove(ref availableMoves, currentHover) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
            //         currentHover = -Vector2Int.one;
            //     }
            //     if(currentluDragging != null) {
            //         currentluDragging.SetPosition(getTileCenter(currentluDragging.currentX, currentluDragging.currentY));
            //         currentluDragging = null;
            //         RemoveHighlightTiles();
            //     }
            // }
        }
    }

    //GENERATING BOARD
    private void GenerateAllTiles(float tilesize, int tilecountX, int tilecountY) {
        yOffset += transform.position.z;
        bounds = new Vector3((tilecountX / 2) * tileSize, (tilecountY / 2) * tileSize, 0) + boardCenter;
        tiles = new GameObject[tilecountX, tilecountY];
        for(int x = 0; x < tilecountX; x++) {
            for(int y = 0; y < tilecountY; y++) {
                tiles[x, y] = GenerateTile(tilesize, x, y);
            }
        }
    }
    private GameObject GenerateTile(float tilesize, int X, int Y) {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", X, Y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(X*tilesize, Y*tilesize, -0.5f * yOffset) - bounds;
        vertices[1] = new Vector3(X*tilesize, (Y + 1)*tilesize, -0.5f * yOffset) - bounds;
        vertices[2] = new Vector3((X + 1)*tilesize, Y*tilesize, -0.5f * yOffset) - bounds;
        vertices[3] = new Vector3((X + 1)*tilesize, (Y + 1)*tilesize, -0.5f * yOffset) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2};

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.AddComponent<BoxCollider>();
        tileObject.layer = LayerMask.NameToLayer("Tile");

        return tileObject;
    }

    //OPERATIONS
    private Vector2Int LookUpTileIndex(GameObject hitinfo) {
        for(int x = 0; x < TILE_COUNT_X; x++) {
            for(int y = 0; y < TILE_COUNT_Y; y++) {
                if(tiles[x, y] == hitinfo) {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one;
    }
    private bool MoveTo(ChessPiece curdrag, int x, int y) {
        if(!ContainsValidMove(ref availableMoves, new Vector2(x, y))) return false;

        Vector2Int prevpos = new Vector2Int(curdrag.currentX, curdrag.currentY);
        // if(chesspieces[x, y] != null && curdrag.type == ChessPieceType.King) {
        //     ChessPiece ocp = chesspieces[x, y];
        //     if(ocp.type != ChessPieceType.King && ocp.type != ChessPieceType.Cannon) {
        //         if(ocp.team == curdrag.team) {
        //             return false;
        //         } 

        //         if(ocp.team == 0) {
                    
        //             deadWhites.Add(ocp);
        //                 ocp.SetScale(Vector3.one * deadSize);
        //                 ocp.SetRotation(Direction.Up);
        //                 ocp.SetPosition(new Vector3(-1 * tileSize, 8 * tileSize, yOffset) 
        //                     - bounds 
        //                     + new Vector3(tileSize/2, tileSize/2, -2 * yOffset)
        //                     + (Vector3.right * deadSpacing) * deadWhites.Count);
        //         } else {
                    
        //             deadBlacks.Add(ocp);
        //                 ocp.SetScale(Vector3.one * deadSize);
        //                 ocp.SetRotation(Direction.Up);
        //                 ocp.SetPosition(new Vector3(-1 * tileSize, -1 * tileSize, -yOffset) 
        //                     - bounds 
        //                     + new Vector3(tileSize/2, tileSize/2, -2 * yOffset)
        //                     + (Vector3.right * deadSpacing) * deadBlacks.Count);
        //         } 
        //         return true;
        //     }
        // }
        chesspieces[x, y] = curdrag;
        chesspieces[prevpos.x, prevpos.y] = null;
        PositionSinglePiece(x, y);
        isWhiteTurn = !isWhiteTurn;
        return true;
    }
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos) {
        for(int i = 0; i < moves.Count; i++) {
            if(moves[i].x == pos.x && moves[i].y == pos.y) {
                return true;
            }
        }
        return false;
    }
    public void RotateTo() {
        RemoveHighlightTiles();
        isRotating = true;
        // currentUI.transform.Find("rotate").Find("OK").gameObject.SetActive(true);
        SetActiveUI(false, true, true);
        if(currentluDragging != null) {
            Direction dir = currentluDragging.currentDirection;
            Direction newDir;
            if(dir == Direction.Up) {
                newDir = Direction.Right;
            } else if(dir == Direction.Right) {
                newDir = Direction.Down;
            } else if(dir == Direction.Down) {
                newDir = Direction.Left;
            } else newDir = Direction.Up;
            currentluDragging.SetRotation(newDir);
        }
    }
    public void ConfirmRotation() {
        // currentUI.transform.Find("rotate").Find("OK").gameObject.SetActive(false);
        SetActiveUI(false, false, true);
        // currentUI.transform.Find("rotate").gameObject.SetActive(false);
        SetActiveUI(false, false);
        isRotating = false;
        currentluDragging = null;
        isWhiteTurn = !isWhiteTurn;
    }
    private void SetActiveUI(bool isFire, bool isActive, bool alsoChild = false) {
        if(isFire) {
            currentUI.transform.Find("fire").gameObject.SetActive(isActive);
        } else {
            if(alsoChild) {
                currentUI.transform.Find("rotate").Find("OK").gameObject.SetActive(isActive);
            } else {
                currentUI.transform.Find("rotate").gameObject.SetActive(isActive);
            }
        }
    }
    private void DeactivateAllUI() {
        SetActiveUI(true, false);
        SetActiveUI(false, false);
        SetActiveUI(false, true, false);
    }

    //POSITIONING
    private void PositionAllPieces() {
        for(int x = 0; x < TILE_COUNT_X; x++) {
            for(int y = 0; y < TILE_COUNT_X; y++) {
                if(chesspieces[x, y] != null) PositionSinglePiece(x, y, true);
            }
        }
    }
    private void PositionSinglePiece(int x, int y, bool force = false) {
        chesspieces[x, y].currentX = x;
        chesspieces[x, y].currentY = y;
        chesspieces[x, y].SetPosition(getTileCenter(x, y), force);   
    }
    private Vector3 getTileCenter(int x, int y) {
        return new Vector3(x * tileSize, y * tileSize, -yOffset) - bounds + new Vector3(tileSize/2, tileSize/2, 0);
    }

    //SPAWNING PIECES
    private void SpawnAllPieces() {
        chesspieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        int blackteam = 1; int whiteteam = 0;

        chesspieces[7, 0] = SpawnSinglePiece(ChessPieceType.Cannon, whiteteam, Direction.Up);
        chesspieces[0, 7] = SpawnSinglePiece(ChessPieceType.Cannon, blackteam, Direction.Down);

        for(int x = 1; x < TILE_COUNT_X - 4; x++) {
            chesspieces[x, 1] = SpawnSinglePiece(ChessPieceType.Double, whiteteam, Direction.Up);
        }
        for(int x = 5; x < TILE_COUNT_X - 1; x++) {
            chesspieces[x, 1] = SpawnSinglePiece(ChessPieceType.Switch, whiteteam, Direction.Up);
        }
        chesspieces[0, 1] = SpawnSinglePiece(ChessPieceType.Switch, blackteam, Direction.Left);
        chesspieces[3, 5] = SpawnSinglePiece(ChessPieceType.Defender, blackteam, Direction.Left);
        chesspieces[5, 3] = SpawnSinglePiece(ChessPieceType.Defender, whiteteam, Direction.Down);
        chesspieces[7, 6] = SpawnSinglePiece(ChessPieceType.Switch, blackteam, Direction.Up);
        chesspieces[1, 6] = SpawnSinglePiece(ChessPieceType.Switch, blackteam, Direction.Left);
        chesspieces[1, 6] = SpawnSinglePiece(ChessPieceType.Switch, blackteam, Direction.Left);
        chesspieces[4, 3] = SpawnSinglePiece(ChessPieceType.King, whiteteam, Direction.Up);
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team, Direction rotation) {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        cp.SetRotation(rotation, true);
        cp.GetComponent<SpriteRenderer>().material = teamMeterials[team];
        return cp;
    }
    private void SpawnLaser(Direction rotation, int team, int posX, int posY) {
        ChessPiece laser = Instantiate(prefabs[5], transform).GetComponent<ChessPiece>();
        laser.team = team;
        laser.type = ChessPieceType.Laser;
        laser.SetRotation(rotation, true);
        allLaser.Add(laser);
        chesspieces[posX, posY] = laser;
        PositionSinglePiece(posX, posY, true);
    }

    //HIGHLIGHTING TILES
    private void HighlightTiles() {
        for(int i = 0; i < availableMoves.Count; i++) {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }
    private void RemoveHighlightTiles() {
        for(int i = 0; i < availableMoves.Count; i++) {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }

    //LASER LOGIC
    public void FireCannon() {
        ChessPiece curcannon = isWhiteTurn ? chesspieces[7, 0] : chesspieces[0, 7];
        Direction fireDirection = curcannon.currentDirection;
        Vector2Int curPosition = isWhiteTurn ? new Vector2Int(curcannon.currentX, curcannon.currentY + 1) : new Vector2Int(curcannon.currentX, curcannon.currentY - 1);
        List<Direction> laserTrail = new List<Direction>();
        for(int i = 0; i < 100; i++) {
            Vector2Int nextpos = GetNextPosition(curPosition, fireDirection);
            if(nextpos == -Vector2Int.one) {
                StartCoroutine(DestroyLaser());
                break;
            }
            laserTrail.Add(fireDirection);
            ChessPiece cp = chesspieces[nextpos.x, nextpos.y];
            if(cp != null) {
                HitResult newResult = cp.TestHit(fireDirection);
                if(newResult.Hitresult == Result.Hit) {
                    StartCoroutine(DestroyLaser());
                    if(cp.team == 0) {
                        deadWhites.Add(cp);
                        cp.SetScale(Vector3.one * deadSize);
                        cp.SetRotation(Direction.Up);
                        cp.SetPosition(new Vector3(-1 * tileSize, 8 * tileSize, yOffset) 
                            - bounds 
                            + new Vector3(tileSize/2, tileSize/2, -2 * yOffset)
                            + (Vector3.right * deadSpacing) * deadWhites.Count);
                    } else {
                        deadBlacks.Add(cp);
                        cp.SetScale(Vector3.one * deadSize);
                        cp.SetRotation(Direction.Up);
                        cp.SetPosition(new Vector3(-1 * tileSize, -1 * tileSize, -yOffset) 
                            - bounds 
                            + new Vector3(tileSize/2, tileSize/2, -2 * yOffset)
                            + (Vector3.right * deadSpacing) * deadBlacks.Count);
                    }
                    chesspieces[nextpos.x, nextpos.y] = null;
                    isWhiteTurn = !isWhiteTurn;
                    break;
                } 
                else if(newResult.Hitresult == Result.Defend) {
                    StartCoroutine(DestroyLaser());
                    break;
                }  
                else if(newResult.Hitresult == Result.Reflect) {
                    fireDirection = newResult.Direction;
                    curPosition = nextpos;
                }
                else if(newResult.Hitresult == Result.Win) {
                    CheckMate(cp.team);
                }
            } else {
                SpawnLaser(fireDirection, 0, nextpos.x, nextpos.y);
                curPosition = nextpos;
            }
        }
        // currentUI.transform.Find("fire").gameObject.SetActive(false);
        SetActiveUI(true, false);
        laserTrail.Clear();
        isWhiteTurn = !isWhiteTurn;
    }
    private Vector2Int GetNextPosition(Vector2Int frompos, Direction direction) {
        int newX;
        int newY;
        switch (direction) {
            case Direction.Up: 
                newY = frompos.y + 1;
                if(newY >= TILE_COUNT_Y) {
                    return -Vector2Int.one;
                }
                return new Vector2Int(frompos.x, newY);
            case Direction.Right: 
                newX = frompos.x + 1;
                if(newX >= TILE_COUNT_X) {
                    return -Vector2Int.one;
                }
                return new Vector2Int(newX, frompos.y);
            case Direction.Down: 
                newY = frompos.y - 1;
                if(newY < 0) {
                    return -Vector2Int.one;
                }
                return new Vector2Int(frompos.x, newY);
            case Direction.Left: 
                newX = frompos.x - 1;
                if(newX < 0) {
                    return -Vector2Int.one;
                }
                return new Vector2Int(newX, frompos.y);
            default:
                return -Vector2Int.one;
        }
    } 
    IEnumerator DestroyLaser() {
        yield return new WaitForSeconds(2f);
        for(int i = 0; i < allLaser.Count; i++) {
            ChessPiece l = chesspieces[allLaser[i].currentX, allLaser[i].currentY];
            Destroy(l.gameObject);
            chesspieces[l.currentX, l.currentY] = null;
        }
        allLaser.Clear();
    }

    //CHECKMATE
    private void CheckMate(int team) {
        DisplayVictory(team);
    }
    private void DisplayVictory(int WinningTeam) {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(WinningTeam == 0 ? 1 : 0).gameObject.SetActive(true);
    }
    public void OnReset() {
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        RemoveHighlightTiles();

        for(int x = 0; x < TILE_COUNT_X; x++) {
            for(int y = 0; y < TILE_COUNT_Y; y++) {
                if(chesspieces[x, y] != null) {
                    Destroy(chesspieces[x, y].gameObject);
                }
                chesspieces[x, y] = null;
            }
        }
        for(int i = 0; i < deadWhites.Count; i++) {
            Destroy(deadWhites[i].gameObject);
        }
        for(int i = 0; i < deadBlacks.Count; i++) {
            Destroy(deadBlacks[i].gameObject);
        }
        deadWhites.Clear();
        deadBlacks.Clear();

        currentluDragging = null;
        availableMoves.Clear();

        SpawnAllPieces();
        PositionAllPieces();
        isWhiteTurn = true;
    }
    public void OnExit() {
        Application.Quit();
    }
}
