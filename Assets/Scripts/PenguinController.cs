using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class PenguinController : MonoBehaviour
{
    public enum ClosestDirection{
        Left,
        Right,
        None
    }

    
    public Tilemap tilemap;
    public float normalSpeed = 2.0f;
    public float slideSpeed = 2.0f;
    private float currentSpeed;
    public GameObject startingPenguin;
    public float eggSpeed = 2.0f;
    public GameObject eggPrefab;
    public float throwForce = 10f;

    public float maxStamina, currentStamina;
    public float speedStamina;
    public float recoverSpeedStamina;
    public Stamina staminaBar;
    public static bool isWater;

    private Vector3Int direction;
    private GameObject selectedPenguin;
    private GameObject eggPenguin;
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        isWater = false;
        isMoving = false;
        currentSpeed = normalSpeed;

        selectedPenguin = startingPenguin;
        selectedPenguin.transform.SetParent(null);
        eggPenguin = startingPenguin;
        Penguin startPenguin = startingPenguin.GetComponent<Penguin>();
        startPenguin.SetEggTrue();
        startPenguin.SetSelectedTrue();

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 selecPenWorldPosition = selectedPenguin.transform.position;
        Vector3Int selecPenTilePosition = tilemap.WorldToCell(selecPenWorldPosition);
        float verticalInput= Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 eggDirection = Vector3.zero;
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
            float timeToReachTarget = distanceToTarget / currentSpeed;
            LerpToTarget(selectedPenguin.transform, targetTilePosition, timeToReachTarget);
        }

        if(horizontalInput > 0){
            if(verticalInput == 0){
                eggDirection = new Vector3(1, 0, 0);
            }
            if(Input.GetKeyDown(KeyCode.L)){
                GameObject closestPenguin = GetClosestPenguin(ClosestDirection.Right);
                HandleChangePlayer(closestPenguin);
            }
        }else if(horizontalInput < 0){
            if(verticalInput == 0){
                eggDirection = new Vector3(-1, 0, 0);
            }
            if(Input.GetKeyDown(KeyCode.L)){
                GameObject closestPenguin = GetClosestPenguin(ClosestDirection.Left);
                HandleChangePlayer(closestPenguin);
            }
        }else{
            if(verticalInput == 0){
                eggDirection = new Vector3(0, 1, 0);
                if(isWater){
                    currentStamina += recoverSpeedStamina;
                    if(currentStamina > maxStamina){
                        currentStamina = maxStamina;
                        currentSpeed = normalSpeed;
                    }
                    staminaBar.SetStamina(currentStamina);
                }
            }
            if(Input.GetKeyDown(KeyCode.L)){
                if(eggPenguin != null && selectedPenguin != eggPenguin){
                    HandleChangePlayer(eggPenguin);
                }else{
                    GameObject closestPenguin = GetClosestPenguin(ClosestDirection.None);
                    HandleChangePlayer(closestPenguin);
                }
            }
        }
        
        if(Input.GetKey(KeyCode.O)){
            if(currentStamina < 0){
                currentStamina = 0;
                currentSpeed = normalSpeed;
            }else if(currentStamina > 0){
                currentStamina -= speedStamina;
                currentSpeed = slideSpeed;
                staminaBar.SetStamina(currentStamina);
            }
        }

        if(Input.GetKeyUp(KeyCode.O)){
            currentSpeed = normalSpeed;
        }
        

        if (Input.GetKeyDown(KeyCode.K))
        {
            if(eggPenguin){
                if(eggDirection == Vector3.zero){
                    eggDirection = new Vector3(0, 1, 0);
                }
                float angle;
                if(selectedPenguin == eggPenguin){
                    angle = Mathf.Atan2(eggDirection.y, eggDirection.x);
                }else{
                    Vector3 eggtoselectedDirection = selectedPenguin.transform.position - eggPenguin.transform.position;
                    angle = Mathf.Atan2(eggtoselectedDirection.y, eggtoselectedDirection.x);
                }
                ThrowEgg(angle, eggSpeed);
            }
        }
    }

    GameObject GetClosestPenguin(ClosestDirection closDirection){
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

    public void ThrowEgg(float angle, float throwSpeed){
        GameObject eggObject = Instantiate(eggPrefab, eggPenguin.transform.position, Quaternion.identity);
        Egg egg = eggObject.GetComponent<Egg>();
        egg.SetFromPenguin(eggPenguin);
        egg.penguinController = this;
        eggPenguin.GetComponent<Penguin>().SetEggFalse();
        
        Rigidbody2D rb = eggObject.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * throwSpeed, ForceMode2D.Impulse);
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

    public GameObject GetSelectedPenguin(){
        return selectedPenguin;
    }

    public void SetSelectedPenguin(GameObject penguin){
        HandleChangePlayer(penguin);
    }

}
