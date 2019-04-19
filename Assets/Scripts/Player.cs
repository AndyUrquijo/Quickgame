using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float Force = 1;
    float inputX;
    public int Health = 3;

    public List<GameObject> Healthbars;
    public PathCreator path;

    public float invulnerabilityDelay;

    Rigidbody2D body;
    SpriteRenderer spriteRend;
    AudioSource audioSource;

    public GameObject TutorialText;
    public GameObject TutorialText2;
    public GameObject WinScreen;
    public GameObject LoseScreen;

    public float inputHeldTimer = 0;
    public Toggle muteToggle;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(_FlashObject(TutorialText, 2));
        TutorialText2.SetActive(false);

        if (PlayerPrefs.GetInt("mute") == 1) muteToggle.isOn = true;
    }

    public void Mute(bool on)
    {
        PlayerPrefs.SetInt("mute", on ? 1 : 0);
        AudioListener.volume = on ? 0 : 1;
    }


    IEnumerator _FlashObject(GameObject go, float duration)
    {
        while (duration > 0)
        {
            int intDel = (int)(duration*7);
            go.SetActive(intDel%4 != 0);
            yield return null;
            duration -= Time.deltaTime;
        }
        go.SetActive(false);
    }
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        if (inputX != 0)
            inputHeldTimer += Time.deltaTime;

        if (invulnerabilityDelay >0)
        {
            int intDel = (int)(invulnerabilityDelay*8);
            spriteRend.enabled = intDel%2 == 0;
        }
    }

    void FixedUpdate()
    {
        if (path.Distance > 1000)
            Win();

        body.AddForce(Vector2.right*inputX*Force);
        if (invulnerabilityDelay >0)
            invulnerabilityDelay -= Time.fixedDeltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision) => CheckHit();
    void OnCollisionStay2D(Collision2D collision) => CheckHit();

    void CheckHit()
    {
        Debug.Log("boop");
        if (invulnerabilityDelay > 0 || !enabled)
            return;

        if(Health == 3 && inputHeldTimer < .5f)
            StartCoroutine(_FlashObject(TutorialText2, 2));

        invulnerabilityDelay = 2;
        Health--;
        Healthbars[Health].GetComponent<Animator>().SetTrigger("Destroy");
        audioSource.PlayOneShot(audioSource.clip);
        if (Health <= 0)
            Lose();

    }


    void Win()
    {
        Debug.Log("Win");
        enabled = false;
        WinScreen.SetActive(true);
        path.StopCounter = true;
    }

    void Lose()
    {
        Debug.Log("Game Over");
        enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Animator>().SetTrigger("Destroy");
        LoseScreen.SetActive(true);
        path.StopCounter = true;
    }


}
