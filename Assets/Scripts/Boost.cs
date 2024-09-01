using Photon.Pun;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public BoostSpawner spawner;
    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("collided");
            spawner.FreeSpawn();
            PhotonNetwork.Destroy(gameObject);
        }
    }*/
    public void Collect()
    {
        spawner.FreeSpawn();
        PhotonNetwork.Destroy(gameObject);
    }
}