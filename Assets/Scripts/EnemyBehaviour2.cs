using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour2 : MonoBehaviour
{
    public GameObject player;
    public GameObject ground;
    public float movespeed = 20;
    public float scandistance = 15f;
    public float groundscandistance = 5f;
    public float enemyDistance;
    public float groundDistance;
    public CanonController canonController;
    

    public LayerMask targetLayerMask;
    public float coolDown = 1f;
    float timer = 0;

    IInputReceiver[] inputReceivers;
    Rigidbody rb;

    private void OnEnable()
    {
        inputReceivers = GetComponentsInChildren<IInputReceiver>();
    }

    private void Update()
    {
        if(timer < coolDown)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if(Physics.Raycast(
                transform.position,
                transform.forward, 
                out RaycastHit hitInfo, 
                scandistance, 
                targetLayerMask, 
                QueryTriggerInteraction.Ignore))
            {
                foreach(var inputReceiver in inputReceivers)
                {
                    inputReceiver.OnFireDown();
                }
            }

            timer = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        ground = GameObject.FindWithTag("ground");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //calculates distance
        enemyDistance = Vector3.Distance(this.transform.position, player.transform.position);
        groundDistance = Vector3.Distance(this.transform.position, ground.transform.position);

        // makes the drone hover, tweak values as necessary
        rb.AddForce(0, 20, 0);
        /*if (groundDistance < 2)
        {
            rb.AddForce(0, 5, 0);
        }

        else if (groundDistance >= 2 && groundDistance < 5)
        {
            rb.AddForce(0, 2, 0);
        }

        else if (groundDistance >= 5)
        {
            rb.AddForce(0, -18, 0);
        }*/


        //always aims when in range
        if (enemyDistance <= 20)
        {
            transform.LookAt(player.transform);
        }

        //decides if it should move and how
        if (enemyDistance <= 20 && enemyDistance >= 10)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, movespeed * Time.deltaTime);
        }
        else if (enemyDistance <= 5)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -movespeed * 3 * Time.deltaTime);
        }
    }

    private void OnDrawGizmos() //for shooting once implemented
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * scandistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * groundscandistance);


        
    }

}