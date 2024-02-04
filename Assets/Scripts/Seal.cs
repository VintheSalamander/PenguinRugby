using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seal : MonoBehaviour
{
    public PenguinController penguinController;
    public float penguinForce;
    public float eggForce;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelectedTrue(){
        animator.SetBool("isSelected", true);
    }

    public void SetSelectedFalse(){
        animator.SetBool("isSelected", false);
    }

    void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Penguin"))
        {
            Vector2 pushDirection = collision.transform.position - transform.position;
            float pushAngle = Mathf.Atan2(pushDirection.y, pushDirection.x);

            Rigidbody2D penguinRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            penguinRigidbody.AddForce(new Vector2(Mathf.Cos(pushAngle), Mathf.Sin(pushAngle)) * penguinForce, ForceMode2D.Impulse);
            if(collision.gameObject == PenguinController.GetEggPenguin()){
                penguinController.ThrowEgg(pushAngle, eggForce);
                EggLifeController.EggDown();
            }
        }
    }
}
