using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PenguinController : MonoBehaviour
{
    public enum ClosestDirection
    {
        Left,
        Right,
        None
    }

    private Vector3Int direction;
    private Vector3 eggDirection;
    public Tilemap tilemap;
    public float playerSpeed = 2.0f;
    public float penguinsSpeed = 2.0f;
    public GameObject startingPenguin;

    public float eggSpeed = 2.0f;
    private static GameObject selectedPenguin;
    private static GameObject eggPenguin;
    private bool isMoving;
    public GameObject eggPrefab;
    public float throwForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        selectedPenguin = startingPenguin;
        selectedPenguin.transform.SetParent(null);
        eggPenguin = startingPenguin;
        Penguin startPenguin = startingPenguin.GetComponent<Penguin>();
        startPenguin.SetEggTrue();
        startPenguin.SetSelectedTrue();
        eggDirection = new Vector3(0, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 selecPenWorldPosition = selectedPenguin.transform.position;
        Vector3Int selecPenTilePosition = tilemap.WorldToCell(selecPenWorldPosition);
        float verticalInput= Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        if(isMoving == false && verticalInput != 0)
        {
            isMoving = true;
            if(horizontalInput > 0){
                if(verticalInput > 0){
                    if(selecPenTilePosition.y % 2 == 0){
                        direction = new Vector3Int(0, 1, 0);
                    }else{
                        direction = new Vector3Int(1, 1, 0);
                    }
                }else if(verticalInput < 0){
                    if(selecPenTilePosition.y % 2 == 0){
                        direction = new Vector3Int(-1, 1, 0);
                    }else{
                        direction = new Vector3Int(0, 1, 0);
                    }
                }
            }else if(horizontalInput < 0){
                if(verticalInput > 0){
                    if(selecPenTilePosition.y % 2 == 0){
                        direction = new Vector3Int(0, -1, 0);
                    }else{
                        direction = new Vector3Int(1, -1, 0);
                    }
                }else if(verticalInput < 0){
                    if(selecPenTilePosition.y % 2 == 0){
                        direction = new Vector3Int(-1, -1, 0);
                    }else{
                        direction = new Vector3Int(0, -1, 0);
                    }
                }
            }else{
                if(verticalInput > 0){
                    direction = new Vector3Int(1, 0, 0);
                }else{
                    direction = new Vector3Int(-1, 0, 0);
                }
            }
            Vector3 targetTilePosition = tilemap.GetCellCenterWorld(selecPenTilePosition + direction);
            eggDirection = targetTilePosition - selectedPenguin.transform.position;
            float distanceToTarget = Vector3.Distance(selectedPenguin.transform.position, targetTilePosition);
            float timeToReachTarget = distanceToTarget / playerSpeed;
            LerpToTarget(selectedPenguin.transform, targetTilePosition, timeToReachTarget);
        }

        if(horizontalInput > 0){
            if(verticalInput == 0){
                eggDirection = new Vector3(1, 0, 0);
            }
            if(Input.GetKeyDown(KeyCode.Tab)){
                GameObject closestPenguin = GetClosestPenguin(ClosestDirection.Right);
                HandleChangePlayer(closestPenguin);
            }
        }else if(horizontalInput < 0){
            if(verticalInput == 0){
                eggDirection = new Vector3(-1, 0, 0);
            }
            if(Input.GetKeyDown(KeyCode.Tab)){
                GameObject closestPenguin = GetClosestPenguin(ClosestDirection.Left);
                HandleChangePlayer(closestPenguin);
            }
        }else{
            if(verticalInput == 0){
                eggDirection = new Vector3(0, 1, 0);
            }
            if(Input.GetKeyDown(KeyCode.Tab)){
                GameObject closestPenguin = GetClosestPenguin(ClosestDirection.None);
                HandleChangePlayer(closestPenguin);
            }
        }
        
        if(Input.GetKey(KeyCode.Alpha1)){
            if(verticalInput != 0){
                Vector3 penguinsWorldPosition = transform.position;
                Vector3Int penguinsTilePosition = tilemap.WorldToCell(penguinsWorldPosition);
                if(verticalInput > 0){
                    direction = new Vector3Int(1, 0, 0);
                }else if(verticalInput < 0){
                    direction = new Vector3Int(-1, 0, 0);
                }
                Vector3 targetTilePosition = tilemap.GetCellCenterWorld(penguinsTilePosition + direction);
                float distanceToTarget = Vector3.Distance(transform.position, targetTilePosition);
                float timeToReachTarget = distanceToTarget / penguinsSpeed;
                LerpToTarget(transform, targetTilePosition, timeToReachTarget);
            }
        }
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(eggPenguin){
                ThrowEgg();
            }
        }
    }

    GameObject GetClosestPenguin(ClosestDirection closDirection)
    {
        GameObject closestChild = null;
        GameObject directionChild = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            float distance = Vector3.Distance(child.position, selectedPenguin.transform.position);

            if(closDirection != ClosestDirection.None){
                float childDirection = child.position.x - selectedPenguin.transform.position.x;
                switch(closDirection){
                    case ClosestDirection.Left:
                        if (childDirection < 0){
                            directionChild = child.gameObject;
                        }
                        break;
                    case ClosestDirection.Right:
                        if (childDirection > 0){
                                directionChild = child.gameObject;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (distance < closestDistance)
            {
                closestChild = child.gameObject;
                closestDistance = distance;
            }
        }
        if(directionChild == null){
            return closestChild;
        }else{
            return directionChild;
        }
    }

    void LerpToTarget(Transform transformToMove, Vector3 target, float timeToTarget){
        transformToMove.position = Vector3.Lerp(transformToMove.position, target, Time.deltaTime / timeToTarget);
        isMoving = false;
    }

    void ThrowEgg(){
        GameObject eggObject = Instantiate(eggPrefab, eggPenguin.transform.position, Quaternion.identity);
        Egg egg = eggObject.GetComponent<Egg>();
        egg.SetFromPenguin(eggPenguin);
        egg.penguinController = this;
        eggPenguin.GetComponent<Penguin>().SetEggFalse();

        float angle = Mathf.Atan2(eggDirection.y, eggDirection.x);

        Rigidbody2D rb = eggObject.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * eggSpeed, ForceMode2D.Impulse);
        eggPenguin = null;
    }

    void HandleChangePlayer(GameObject tochangePenguin){
        selectedPenguin.transform.SetParent(transform);
        selectedPenguin.GetComponent<Penguin>().SetSelectedFalse();
        tochangePenguin.GetComponent<Penguin>().SetSelectedTrue();
        tochangePenguin.transform.SetParent(null);
        selectedPenguin = tochangePenguin;
    }

    public void SetEggPenguin(GameObject penguin){
        eggPenguin = penguin;
        eggPenguin.GetComponent<Penguin>().SetEggTrue();
    }

    public GameObject GetEggPenguin(){
        return eggPenguin;
    }

    public void SetSelectedPenguin(GameObject penguin){
        HandleChangePlayer(penguin);
    }

}
