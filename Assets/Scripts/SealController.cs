using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SealController : MonoBehaviour
{
    public enum ClosestDirection{
        Left,
        Right,
        None
    }

    private Vector3Int direction;
    public Tilemap tilemap;
    public float playerSpeed = 2.0f;
    public float SealsSpeed = 2.0f;
    public GameObject startingSeal;
    private static GameObject selectedSeal;
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        selectedSeal = startingSeal;
        selectedSeal.transform.SetParent(null);
        selectedSeal.GetComponent<Seal>().SetSelectedTrue();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 selecSealWorldPosition = selectedSeal.transform.position;
        Vector3Int selecSealTilePosition = tilemap.WorldToCell(selecSealWorldPosition);
        float verticalInput= Input.GetAxis("Vertical1");
        float horizontalInput = Input.GetAxis("Horizontal1");
        if(isMoving == false && verticalInput != 0){
            isMoving = true;
            if(horizontalInput > 0){
                if(verticalInput > 0){
                    if(selecSealTilePosition.y % 2 == 0){
                        direction = new Vector3Int(0, 1, 0);
                    }else{
                        direction = new Vector3Int(1, 1, 0);
                    }
                }else if(verticalInput < 0){
                    if(selecSealTilePosition.y % 2 == 0){
                        direction = new Vector3Int(-1, 1, 0);
                    }else{
                        direction = new Vector3Int(0, 1, 0);
                    }
                }
            }else if(horizontalInput < 0){
                if(verticalInput > 0){
                    if(selecSealTilePosition.y % 2 == 0){
                        direction = new Vector3Int(0, -1, 0);
                    }else{
                        direction = new Vector3Int(1, -1, 0);
                    }
                }else if(verticalInput < 0){
                    if(selecSealTilePosition.y % 2 == 0){
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
            Vector3 targetTilePosition = tilemap.GetCellCenterWorld(selecSealTilePosition + direction);
            float distanceToTarget = Vector3.Distance(selectedSeal.transform.position, targetTilePosition);
            float timeToReachTarget = distanceToTarget / playerSpeed;
            LerpToTarget(selectedSeal.transform, targetTilePosition, timeToReachTarget);
        }

        if(horizontalInput > 0){
            if(Input.GetKeyDown(KeyCode.Tab)){
                GameObject closestSeal = GetClosestSeal(ClosestDirection.Right);
                HandleChangePlayer(closestSeal);
            }
        }else if(horizontalInput < 0){
            if(Input.GetKeyDown(KeyCode.Tab)){
                GameObject closestSeal = GetClosestSeal(ClosestDirection.Left);
                HandleChangePlayer(closestSeal);
            }
        }else{
            if(Input.GetKeyDown(KeyCode.Tab)){
                GameObject closestSeal = GetClosestSeal(ClosestDirection.None);
                HandleChangePlayer(closestSeal);
            }
        }
        
        if(Input.GetKey(KeyCode.Alpha1)){
            if(verticalInput != 0){
                Vector3 SealsWorldPosition = transform.position;
                Vector3Int SealsTilePosition = tilemap.WorldToCell(SealsWorldPosition);
                if(verticalInput > 0){
                    direction = new Vector3Int(1, 0, 0);
                }else if(verticalInput < 0){
                    direction = new Vector3Int(-1, 0, 0);
                }
                Vector3 targetTilePosition = tilemap.GetCellCenterWorld(SealsTilePosition + direction);
                float distanceToTarget = Vector3.Distance(transform.position, targetTilePosition);
                float timeToReachTarget = distanceToTarget / SealsSpeed;
                LerpToTarget(transform, targetTilePosition, timeToReachTarget);
            }
        }
    }

    GameObject GetClosestSeal(ClosestDirection closDirection){
        GameObject closestChild = null;
        GameObject directionChild = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            float distance = Vector3.Distance(child.position, selectedSeal.transform.position);

            if(closDirection != ClosestDirection.None){
                float childDirection = child.position.x - selectedSeal.transform.position.x;
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

    void HandleChangePlayer(GameObject tochangeSeal){
        selectedSeal.transform.SetParent(transform);
        selectedSeal.GetComponent<Seal>().SetSelectedFalse();
        tochangeSeal.GetComponent<Seal>().SetSelectedTrue();
        tochangeSeal.transform.SetParent(null);
        selectedSeal = tochangeSeal;
    }

    public void SetSelectedSeal(GameObject Seal){
        HandleChangePlayer(Seal);
    }

}

