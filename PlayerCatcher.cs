using StarterAssets;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class PlayerCatcher : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Transform player;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Caught player");
        player.GetComponent<ThirdPersonController>().enabled = false;
        player.position = respawnPoint.position;
        StartCoroutine(Respawn());
    }
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<ThirdPersonController>().enabled = true;
        yield break;
    }
}
