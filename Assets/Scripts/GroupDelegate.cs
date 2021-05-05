using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupDelegate : MonoBehaviour
{
    [SerializeField]
    private PersonajeBase owner;

    public void OnTriggerEnter(Collider other)
    {
        owner.group.Add(other.gameObject.GetComponent<PersonajeBase>());
    }
    public void OnTriggerExit(Collider other)
    {
        owner.group.Remove(other.gameObject.GetComponent<PersonajeBase>());
    }
    public void OnTriggerStay(Collider other)
    {
        PersonajeBase person = other.gameObject.GetComponent<PersonajeBase>();
        if (!owner.group.Contains(person))
            owner.group.Add(person);
    }
}
