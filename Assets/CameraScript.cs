using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player; // Référence vers le GameObject du joueur

    public float smoothTime = 0.3f; // Temps de transition entre l'ancienne et la nouvelle position

    private Vector3 velocity = Vector3.zero;

    public void FreezeCamera(Vector3 positionToFreeze)
    {
        Debug.Log(positionToFreeze.ToString());
        Debug.Log(transform.position.ToString());
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(positionToFreeze.x, positionToFreeze.y, -10);
    }
}
