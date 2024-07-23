using System;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField] public MeshRenderer meshRenderer;

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
        object[] data = (info.photonView.InstantiationData);
        Color newColor = new Color((float)data[0], (float)data[1], (float)data[2], (float)data[3]);
        meshRenderer.material.color = newColor;
        Debug.Log(newColor);
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
