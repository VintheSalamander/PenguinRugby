using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public PenguinController penguinController;
    public float sealPushForce;
    private GameObject fromPenguin;
    private bool ignoreFrom;
    // Start is called before the first frame update
    void Start()
    {
        ignoreFrom = false;
        StartCoroutine(TrueIgnoreFrom());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFromPenguin(GameObject penguin){
        fromPenguin = penguin;
    }
    
    void OnCollisionEnter2D(Collision2D collision){
        LayerMask collisionLayer = collision.gameObject.layer;
        if (collisionLayer == LayerMask.NameToLayer("Penguins")){
            HandlePenguinCollision(collision);
        }else if(collisionLayer == LayerMask.NameToLayer("Seals")){
            HandleSealCollision();
        }
    }

    void OnCollisionStay2D(Collision2D collision){
        if (collision.gameObject.layer == LayerMask.NameToLayer("Penguins")){
            HandlePenguinCollision(collision);
        }
    }

    void HandlePenguinCollision(Collision2D collision){
        if(collision.gameObject != fromPenguin){
            penguinController.SetEggPenguin(collision.gameObject);
            Destroy(gameObject);
        }else{
            if(ignoreFrom){
                penguinController.SetEggPenguin(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
    
    void HandleSealCollision(){
        penguinController.SetEggPenguin(penguinController.GetFurthestPenguin(transform.position));
        Debug.Log("1 EGG DOWN");
        EggLifeController.EggDown();
        Destroy(gameObject);
    }
    
    IEnumerator TrueIgnoreFrom(){
        yield return new WaitForSeconds(0.75f);
        ignoreFrom = true;
        yield return null;
    }
}
