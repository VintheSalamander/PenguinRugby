using UnityEngine;

public class Penguin : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEggTrue(){
        animator.SetBool("hasEgg", true);
    }

    public void SetEggFalse(){
        animator.SetBool("hasEgg", false);
    }

    public void SetSelectedTrue(){
        animator.SetBool("isSelected", true);
    }

    public void SetSelectedFalse(){
        animator.SetBool("isSelected", false);
    }

    void OnCollisionStay2D(Collision2D collision){
        LayerMask collisionLayer = collision.gameObject.layer;
        if (collisionLayer == LayerMask.NameToLayer("Water") && animator.GetBool("isSelected")){
            PenguinController.isWater = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision){
        LayerMask collisionLayer = collision.gameObject.layer;
        if (collisionLayer == LayerMask.NameToLayer("Water")){
            PenguinController.isWater = false;
        }
    }
}
