using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int minStepsToEncounter;
    [SerializeField] private int maxStepsToEncounter;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;
    private bool movimingInGrass;
    private float stepTimer;
    private int stepsToEncounter;


    private const string IS_WALK_PARAM = "Walk";
    private const string BattleScene= "BattleScene";
    private const float timePerStep = 0.5f;
    private void Awake()
    {
        playerControls = new PlayerControls();
        CalculateStepsNextEncounter();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;

        movement = new Vector3(x, 0, z).normalized;

        anim.SetBool(IS_WALK_PARAM, movement != Vector3.zero);

        if (x != 0 && x < 0)
        {
            playerSprite.flipX = true;
        }

        if (x != 0 && x > 0)
        {
            playerSprite.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);

        Collider[] colliders = Physics.OverlapSphere(transform.position,1f, grassLayer);
        movimingInGrass = colliders.Length!= 0 && movement != Vector3.zero;

        if( movimingInGrass == true )

        {
            stepTimer += Time.fixedDeltaTime;
            if (stepTimer > timePerStep)
            {
                stepsInGrass++;
                stepTimer = 0;
                if (stepsInGrass >= stepsToEncounter)
                {
                    SceneManager.LoadScene(BattleScene);
                }


            }
        }
    }

    private void CalculateStepsNextEncounter()
    {
        stepsToEncounter = Random.Range(minStepsToEncounter, maxStepsToEncounter);
    }
}