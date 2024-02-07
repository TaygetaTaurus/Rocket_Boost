using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100.0f;
    [SerializeField] float boostSpeed = 100.0f;
    [SerializeField] float levelLoadDelay = 1f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    Light[] lightOnRocket;

    MeshRenderer[] meshRenderer;

    bool collisionsDisabled = false;

    enum State { Alive,Dying,Transcending }
    State state = State.Alive;

    Rigidbody rigidBody;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        lightOnRocket = FindObjectsOfType<Light>();
        meshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild)
        {
            RespondToDebugKey();
        }
    }

    private void RespondToDebugKey()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionsDisabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                return;
            case "Finish":
                state = State.Transcending;
                break;
            default:
                state = State.Dying;
                break;
        }
        StartChangingLevel(state);
    }

    private void OnTriggerEnter(Collider other)
    {
        state = State.Transcending;
        StartChangingLevel(state);
    }

    private void StartChangingLevel(State state)
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
        if (state == State.Transcending)
        {
            audioSource.PlayOneShot(success);
            successParticles.Play();
            Invoke(nameof(LoadNextLevel), levelLoadDelay);
        }
        else if (state == State.Dying)
        {
            audioSource.PlayOneShot(death);
            deathParticles.Play();
            DestroyPlayer();
            Invoke(nameof(LoadFirstLevel), levelLoadDelay);
        }
    }

    private void DestroyPlayer()
    {
        foreach (MeshRenderer meshRenderer1 in meshRenderer)
        {
            meshRenderer1.gameObject.SetActive(false);
        }
        foreach (Light light1 in lightOnRocket)
        {
            if (light1.gameObject.tag == "OnRocket")
            {
                light1.gameObject.SetActive(false);
            }
        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero;

        float rotationThisFrame = Time.deltaTime * rotationSpeed;

        if (Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        float speedThisFrame = Time.deltaTime * boostSpeed;
        rigidBody.AddRelativeForce(Vector3.up * speedThisFrame);
        
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        if(!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }
}
