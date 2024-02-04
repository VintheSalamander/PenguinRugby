using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Vector3Int direction;
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
        eggPenguin = startingPenguin;
        Penguin startPenguin = startingPenguin.GetComponent<Penguin>();
        startPenguin.SetEggTrue();
        startPenguin.SetSelectedTrue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject closestPenguin = GetClosestPenguin(Input.mousePosition);
            selectedPenguin.transform.SetParent(transform);
            selectedPenguin.GetComponent<Penguin>().SetSelectedFalse();
            closestPenguin.GetComponent<Penguin>().SetSelectedTrue();
            closestPenguin.transform.SetParent(null);
            selectedPenguin = closestPenguin;
        }
        
        if(selectedPenguin){
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
                float distanceToTarget = Vector3.Distance(selectedPenguin.transform.position, targetTilePosition);
                float timeToReachTarget = distanceToTarget / playerSpeed;
                LerpToTarget(selectedPenguin.transform, targetTilePosition, timeToReachTarget);
            }
        }
        if(Input.GetMouseButtonDown(1) || Input.GetMouseButton(1)){
            Vector3 penguinsWorldPosition = transform.position;
            Vector3Int penguinsTilePosition = tilemap.WorldToCell(penguinsWorldPosition);
            Vector3 mousePosition = Input.mousePosition;
            if(mousePosition.y > Screen.height / 2){
                direction = new Vector3Int(1, 0, 0);
            }else{
                direction = new Vector3Int(-1, 0, 0);
            }
            Vector3 targetTilePosition = tilemap.GetCellCenterWorld(penguinsTilePosition + direction);
            float distanceToTarget = Vector3.Distance(transform.position, targetTilePosition);
            float timeToReachTarget = distanceToTarget / penguinsSpeed;
            LerpToTarget(transform, targetTilePosition, timeToReachTarget);
        }
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(eggPenguin){
                ThrowEgg();
            }
        }
    }

    GameObject GetClosestPenguin(Vector3 mousePos)
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        GameObject closestChild = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            float distance = Vector3.Distance(child.position, worldMousePos);
            if (distance < closestDistance)
            {
                closestChild = child.gameObject;
                closestDistance = distance;
            }
        }

        return closestChild;
    }

    void LerpToTarget(Transform transformToMove, Vector3 target, float timeToTarget){
        transformToMove.position = Vector3.Lerp(transformToMove.position, target, Time.deltaTime / timeToTarget);
        isMoving = false;
    }

    void ThrowEgg()
    {
        GameObject eggObject = Instantiate(eggPrefab, eggPenguin.transform.position, Quaternion.identity);
        eggObject.GetComponent<Egg>().SetFromPenguin(eggPenguin);
        eggPenguin.GetComponent<Penguin>().SetEggFalse();

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 throwDirection = mousePosition - eggPenguin.transform.position;
        float angle = Mathf.Atan2(throwDirection.y, throwDirection.x);

        Rigidbody2D rb = eggObject.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * eggSpeed, ForceMode2D.Impulse);
        eggPenguin = null;
    }

    public static void SetEggPenguin(GameObject penguin){
        eggPenguin = penguin;
        eggPenguin.GetComponent<Penguin>().SetEggTrue();
    }

    public static GameObject GetEggPenguin(){
        return eggPenguin;
    }
}
