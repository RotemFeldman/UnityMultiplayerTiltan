using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    
    private const string RecievedamageRPC = "RecieveDamage";

    [SerializeField] private int HP = 100;

    [Header("Control")] 
    [SerializeField] private PlayerInputHandler inputHandler;

    [SerializeField] private float speed = 10;
    
    [Header("Projectile")]
    private const string ProjectilePrefabName = "Prefabs/Projectile";
    private const string ProjectileTag = "Projectile";
    
    private Quaternion _projectileSpawnQuaternion;
    
    
    
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
            _projectileSpawnQuaternion = lookAtRotation;
            Vector3 eulerRotation = lookAtRotation.eulerAngles;
            eulerRotation.x = 0;
            eulerRotation.z = 0;
            
            transform.eulerAngles = eulerRotation;
            transform.Translate(_movementVector * (Time.deltaTime * speed),Space.World);
        }
        
    }

    private void Shoot()
    {
        GameObject proj =
            PhotonNetwork.Instantiate(ProjectilePrefabName, transform.position, _projectileSpawnQuaternion);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //TODO implement pun observer
    }
}
