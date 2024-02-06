using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seal : MonoBehaviour
{
    private PenguinController penguinController;
    private float penguinForce;
    private float eggForce;
    private float penForceDuration;
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

            float minOffset = Mathf.Deg2Rad * 5f;
            float maxOffset = Mathf.Deg2Rad * 45f;
            float randomOffset = Random.Range(minOffset, maxOffset);

            float pushAngleEgg = pushAngle + randomOffset;

            Rigidbody2D penguinRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            penguinRigidbody.velocity = new Vector2(Mathf.Cos(pushAngle), Mathf.Sin(pushAngle)) * penguinForce;
            StartCoroutine(PenguinPushForce(penguinRigidbody));
            if(collision.gameObject == penguinController.GetEggPenguin()){
                penguinController.ThrowEgg(pushAngleEgg, eggForce);
            }
        }
    }

    public void SetSealVariables(PenguinController penControl, float sealToPenguinForce, float sealToEggForce, float penFDur){
        penguinController = penControl;
        penguinForce = sealToPenguinForce;
        eggForce = sealToEggForce;
        penForceDuration = penFDur;
    }

    IEnumerator PenguinPushForce(Rigidbody2D penguinRigidbody){
        yield return new WaitForSeconds(penForceDuration);
        penguinRigidbody.velocity = Vector2.zero;
        yield return null;
    }
}
