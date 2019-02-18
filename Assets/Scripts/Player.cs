using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject WinScreen;
    public GameObject LoseScreen;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        if(path.Distance > 1000)
        {
            Win();
        }

        if(invulnerabilityDelay >0)
        {
            int intDel = (int)(invulnerabilityDelay*8);
            spriteRend.enabled = intDel%2 == 0;
        }
    }

    void FixedUpdate()
    {
        body.AddForce(Vector2.right*inputX*Force);
        if (invulnerabilityDelay >0)
            invulnerabilityDelay -= Time.fixedDeltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision) => CheckHit();
    void OnCollisionStay2D(Collision2D collision) => CheckHit();

    void CheckHit()
    {
        Debug.Log("boop");
        if (invulnerabilityDelay > 0)
            return;

        invulnerabilityDelay = 2;
        Health--;
        Healthbars[Health].SetActive(false);

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
        Destroy(gameObject);
        LoseScreen.SetActive(true);
        path.StopCounter = true;
    }


}
