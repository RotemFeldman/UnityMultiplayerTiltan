using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEditor.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField] public int hp = 100;

    [Header("Control")] 
    private const string PlayerEliminated_RPC = nameof(PlayerEliminated);

    public UnityEvent OnLastPlayerRemaining;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private float speed = 10;

    
    [Header("Projectile")]
    private const string ProjectilePrefabName = "Prefabs/Projectile";
    private const string ProjectileTag = "Projectile";
    private const string ApplyDamage_RPC = nameof(ApplyDamage);

    [Header("Visual")] 
    [SerializeField] private MeshRenderer meshRenderer;
    private Color _playerColor;
    
    
    private Camera _cachedCamera;
    private int _playersEliminated;
    private Vector3 _raycastPos;
    private Vector3 _movementVector;

    public override void OnEnable()
    {
        base.OnEnable();
        if (photonView.IsMine)
        {
            inputHandler.OnShootInput += Shoot;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (photonView.IsMine)
        {
            inputHandler.OnShootInput -= Shoot;
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
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Color playerCol = meshRenderer.material.color;

            float colR = playerCol.r;
            float colG = playerCol.g;
            float colB = playerCol.b;
            float colA = playerCol.a;
            stream.SendNext(colR);
            stream.SendNext(colG);
            stream.SendNext(colB);
            stream.SendNext(colA);
        }
        else if (stream.IsReading)
        {
            Color newColor;
            newColor.r = (float)stream.ReceiveNext();
            newColor.g = (float)stream.ReceiveNext();
            newColor.b = (float)stream.ReceiveNext();
            newColor.a = (float)stream.ReceiveNext();
            meshRenderer.material.color = newColor;
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
                
            }
            other.gameObject.SetActive(false);
        }
    }

    private void Shoot()
    {
        Color c = meshRenderer.material.color;
        
        object[] colorData = new object[] {c.r,c.g,c.b,c.a};
        
        Debug.Log("player color as object is " + c);
        
        var tr = transform;
        GameObject proj =
            PhotonNetwork.Instantiate(ProjectilePrefabName, tr.position, tr.rotation,0,colorData);
    }
    
    
    [PunRPC]
    private void PlayerEliminated()
    {
        _playersEliminated++;
        if (_playersEliminated >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log($"{PhotonNetwork.NickName} is the winner");
            OnLastPlayerRemaining?.Invoke();
        }
    }
    
    
    [PunRPC]
    private void ApplyDamage()
    {
        hp -= 10;
        Debug.Log("applied damage, life left:" + hp);

        if (hp <= 0)
        {
            photonView.RPC(PlayerEliminated_RPC,RpcTarget.All);

            if (photonView.IsMine)
                StartCoroutine(DestroyDelay(1f, gameObject));
            gameObject.SetActive(false);
        }
    }
    

    private IEnumerator DestroyDelay(float delayTime, GameObject gameObjectToDestroy)
    {
        yield return new WaitForSeconds(delayTime);
        PhotonNetwork.Destroy(gameObjectToDestroy);
    }
}
