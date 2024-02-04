using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penguin : MonoBehaviour
{
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
}
