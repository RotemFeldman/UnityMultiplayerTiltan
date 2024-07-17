using System;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField] private float speed = 20;
    [SerializeField] private float lifeTime = 10;

    private float _timeLeft;

    private void Awake()
    {
        _timeLeft = lifeTime;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0f)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //TODO projectile instantiation data
    }

    /*private void OnCollisionEnter(Collision other)
    {
        if (photonView.IsMine)
        {
            if (!other.gameObject.TryGetComponent(out PhotonView pv))
            {
                // hit terrain
                PhotonNetwork.Destroy(gameObject);
                return;
            }
                

            if (!pv.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);

                /*if (other.gameObject.CompareTag("Player"))
                {
                    var p = other.gameObject.GetComponent<PlayerController>();
                    p.ApplyDamage();
                }#1#
            }
        }

    }*/
}
