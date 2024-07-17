using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    
    private const string RecievedamageRPC = "RecieveDamage";

    [SerializeField] private int hp = 100;

    [Header("Control")] 
    [SerializeField] private PlayerInputHandler inputHandler;

    [SerializeField] private float speed = 10;
    
    [Header("Projectile")]
    private const string ProjectilePrefabName = "Prefabs/Projectile";
    private const string ProjectileTag = "Projectile";
    private const string ApplyDamage_RPC = nameof(ApplyDamage);
    
    
    private Camera _cachedCamera;

    private Vector3 _raycastPos;
    private Vector3 _movementVector;

    public override void OnEnable()
    {
        base.OnEnable();
        if (photonView.IsMine)
        {
            inputHandler.onShootInput.AddListener(Shoot);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (photonView.IsMine)
        {
            inputHandler.onShootInput.RemoveListener(Shoot);
        }
    }

    private void Start()
    {
        _cachedCamera = Camera.main;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            _movementVector = inputHandler.movementInput;
            
            //stolen from the course git
            Ray ray = _cachedCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                _raycastPos = hit.point;
            }
            
            Vector3 directionToFace = _raycastPos - gameObject.transform.position;
            Quaternion lookAtRotation = Quaternion.LookRotation(directionToFace);
            Vector3 eulerRotation = lookAtRotation.eulerAngles;
            eulerRotation.x = 0;
            eulerRotation.z = 0;
            
            transform.eulerAngles = eulerRotation;
            transform.Translate(_movementVector * (Time.deltaTime * speed),Space.World);
        }
        
    }

    private void Shoot()
    {
        var tr = transform;
        GameObject proj =
            PhotonNetwork.Instantiate(ProjectilePrefabName, tr.position, tr.rotation);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //TODO implement pun observer
    }

    [PunRPC]
    private void ApplyDamage()
    {
        Debug.Log("applied damage, life left:" + hp);
        hp -= 10;

        if (hp <= 0)
        {
            Debug.Log("died");
            
            if(photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ProjectileTag))
        {
            var proj = other.GetComponent<Projectile>();
            
            //TODO ask lior about this line, how is it different from isMine?
            if(proj.photonView.Owner.ActorNumber == photonView.Owner.ActorNumber)
                return;

            if (proj.photonView.IsMine)
            {
                StartCoroutine(DestroyDelay(1f,proj.gameObject));
                photonView.RPC(ApplyDamage_RPC,RpcTarget.All);
                proj.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator DestroyDelay(float delayTime, GameObject gameObjectToDestroy)
    {
        yield return new WaitForSeconds(delayTime);
        PhotonNetwork.Destroy(gameObjectToDestroy);
    }
}
