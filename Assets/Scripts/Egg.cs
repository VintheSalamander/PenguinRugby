using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public PenguinController penguinController;
    public float sealPushForce;
    private GameObject fromPenguin;
    private Rigidbody2D rb;
    private bool ignoreFrom;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            HandleSealCollision(collision);
        }
    }

    void OnCollisionStay2D(Collision2D collision){
        if (collision.gameObject.layer == LayerMask.NameToLayer("Penguins")){
            HandlePenguinCollision(collision);
        }
    }

    void HandlePenguinCollision(Collision2D collision){
        if(collision.gameObject != fromPenguin){
            Destroy(gameObject);
            penguinController.SetEggPenguin(collision.gameObject);
            
        }else{
            if(ignoreFrom){
                Destroy(gameObject);
                penguinController.SetEggPenguin(collision.gameObject);
            }
        }
    }
    
    void HandleSealCollision(Collision2D collision){
        penguinController.SetEggPenguin(penguinController.GetSelectedPenguin());
        Debug.Log("1 EGG DOWN");
        EggLifeController.EggDown();
    }
    
    IEnumerator TrueIgnoreFrom(){
        yield return new WaitForSeconds(0.75f);
        ignoreFrom = true;
        yield return null;
    }
}
