using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    float startRocketPos;

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);

    [SerializeField] GameObject rocket;
    [SerializeField] GameObject launchPad;
    [SerializeField] GameObject directionalLight;


    Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startRocketPos = rocket.transform.position.y;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float partToReach = rocket.transform.position.y /
                            (transform.localScale.y + startRocketPos + launchPad.transform.position.y + 80f);
        Vector3 offset = partToReach * movementVector;
        transform.position = offset + startPosition;
        directionalLight.GetComponent<Light>().intensity = partToReach;
    }
}