using System;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField] public MeshRenderer meshRenderer;
    [SerializeField] public Collider collider;

    [SerializeField] private float speed = 20;
    [SerializeField] private float lifeTime = 10;
    

    private void Update()
    {
        if (photonView.IsMine)
        {
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0f)
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
    }

    public void DisableProjectile()
    {
        meshRenderer.enabled = false;
        collider.enabled = false;
    }
    
}
