using System;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField] private float speed = 20;

    private void Update()
    {
        if (photonView.IsMine)
        {
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //TODO projectile instantiation data
    }
}
