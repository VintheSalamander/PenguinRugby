using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public PenguinController penguinController;
    private GameObject fromPenguin;
    private Collider2D eggCollider;
    private bool ignoreFrom;
    // Start is called before the first frame update
    void Start()
    {
        eggCollider = GetComponent<Collider2D>();
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Penguins")){
            HandlePenguinCollision(collision);
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
    
    IEnumerator TrueIgnoreFrom(){
        yield return new WaitForSeconds(0.75f);
        ignoreFrom = true;
        yield return null;
    }
}
