using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogCharacter : MonoBehaviour
{
    public Speaker speaker;

    public void DialogOff()
    {
        Destroy(gameObject);
    }
}
