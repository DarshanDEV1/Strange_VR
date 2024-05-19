using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gola : MonoBehaviour
{
    [SerializeField] LayerMask m_Hit_Layer_Mask;
    [SerializeField] ParticleSystem m_Hit_Effect;
    private void Start()
    {
        MoveGola();
    }

    private void MoveGola()
    {
        //GetComponent<Rigidbody>().AddForce(Vector3.forward * 100f);
        GetComponent<Rigidbody>().velocity = FindObjectOfType<OVRCameraRig>().centerEyeAnchor.forward * 100f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.gameObject.layer);
        if (m_Hit_Layer_Mask == (m_Hit_Layer_Mask | (1 << collision.collider.gameObject.layer)))
        {
            m_Hit_Effect.Play();
        }
    }

}
