using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public BoostSpawner spawner;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            spawner.FreeSpawn();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

