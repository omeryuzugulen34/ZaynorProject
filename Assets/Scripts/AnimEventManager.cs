//using System.Diagnostics;
using UnityEngine;

public class AnimEventManager : MonoBehaviour
{
    CharacterControl characterControl;

    void Start()
    {
        characterControl = gameObject.transform.parent.gameObject.GetComponent<CharacterControl>();    
    }


    public void ComboPunch(int punchNumber){
        characterControl.HandleAttack();
        Debug.LogWarning("Punch " + punchNumber);
    }
}
