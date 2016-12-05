using UnityEngine;
using System.Collections;

public class LuukTankScriptBackup : Tank {

    private bool targetFound = false;

    GameObject newPosition;

    private GameObject enemyTank;
    private Vector3 goalToMoveTo;
    private int frameCount;

    Renderer tankBody;
    [SerializeField]
    private GameObject tankBodyObject;

    private int rayDistance = 2;
    private float rayLenght = 5.0f;
    private float rayLenghtForward = 5.0f;
    private float rayFarLenght = 5.0f;

    GameObject frontFeelerPos;
    GameObject leftFeelerPos;
    GameObject farLeftFeelerPos;
    GameObject rightFeelerPos;
    GameObject farRightFeelerPos;


    private bool goingLeft = false;
    private bool goingRight = false;

    private bool rotatedleft = false;

    private bool tankIsMoving = false;
    private bool runningSearchState = false;

    private bool canShoot = false;

    private Vector3 haltPosition;
    private bool wallAhead = false;

    IEnumerator searchRoutine;
    IEnumerator combatRoutine;
    IEnumerator alertRoutine;
    IEnumerator panicRoutine;

    private bool somethingFront = false;
    private bool somethingLeft = false;
    private bool somethingFarLeft = false;
    private bool somethingRight = false;
    private bool somethingFarRight = false;

    private bool chooseLeftRight;



    // Use this for initialization
    protected override void Start()
    {
        base.Start();



        chooseLeftRight = false;
        enemyTank = target;
        //goalToMoveTo = goal;

        tankBody = tankBodyObject.GetComponent<Renderer>();

        searchRoutine = SearchState();
        combatRoutine = CombatState();
        alertRoutine = AlertState();
        panicRoutine = PanicState();

        frontFeelerPos = new GameObject("frontFeelerPos");
        frontFeelerPos.transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.z + (tankBody.bounds.size.z / 2)));
        frontFeelerPos.transform.rotation = transform.rotation;
        frontFeelerPos.transform.parent = gameObject.transform;

        leftFeelerPos = new GameObject("leftFeelerPos");
        leftFeelerPos.transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.z + (tankBody.bounds.size.z / 2)));
        leftFeelerPos.transform.rotation = transform.rotation;

        leftFeelerPos.transform.Rotate(Vector3.up, -30);

        leftFeelerPos.transform.parent = gameObject.transform;
        leftFeelerPos.transform.Translate((Vector3.forward - (Vector3.right - (Vector3.forward / 2))) * rayDistance);

        farLeftFeelerPos = new GameObject("farLeftFeelerPos");
        farLeftFeelerPos.transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.z + (tankBody.bounds.size.z / 2)));
        farLeftFeelerPos.transform.rotation = transform.rotation;

        farLeftFeelerPos.transform.Rotate(Vector3.up, -90);

        farLeftFeelerPos.transform.parent = gameObject.transform;
        farLeftFeelerPos.transform.Translate((Vector3.forward - (Vector3.right - (Vector3.forward / 2))) * rayDistance);


        rightFeelerPos = new GameObject("rightFeelerPos");
        rightFeelerPos.transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.z + (tankBody.bounds.size.z / 2)));
        rightFeelerPos.transform.rotation = transform.rotation;

        rightFeelerPos.transform.Rotate(Vector3.up, 30);

        rightFeelerPos.transform.parent = gameObject.transform;
        rightFeelerPos.transform.Translate((Vector3.forward - (Vector3.left - (Vector3.forward / 2))) * rayDistance);

        farRightFeelerPos = new GameObject("farRightFeelerPos");
        farRightFeelerPos.transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.z + (tankBody.bounds.size.z / 2)));
        farRightFeelerPos.transform.rotation = transform.rotation;

        farRightFeelerPos.transform.Rotate(Vector3.up, 90);

        farRightFeelerPos.transform.parent = gameObject.transform;
        farRightFeelerPos.transform.Translate((Vector3.forward - (Vector3.left - (Vector3.forward / 2))) * rayDistance);

        //if (enemyTank == null)
        //{



        //}

        //if (enemyTank != null)
        //{
        //    StopCoroutine(searchRoutine);
        //    StartCoroutine(CombatState());
        //    Debug.Log("Enemy Found! CombatState!");
        //}

        //if (health == 2)
        //{
        //    StartCoroutine(AlertState());
        //    Debug.Log("Tank was Damaged! AlertState!");
        //}

        //if (health == 1)
        //{
        //    StopCoroutine(alertRoutine);
        //    StartCoroutine(PanicState());
        //   Debug.Log("Tank was Critically Damaged! PanicState!");
        //}
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!tankIsMoving)
        {
            StartCoroutine(searchRoutine);
            Debug.Log("Starting Coroutine Moving");
        }

        if (!runningSearchState)
        {
            StartCoroutine(SearchAim());
        }

       if (canShoot == true)
        {
            AimAt(enemyTank);
        }

        feelerCheck();
    }


    protected override void MoveTo(Vector3 destination)
    {
        base.MoveTo(goalToMoveTo);

    }

    protected override void AimAt(GameObject targetLocation)
    {
        base.AimAt(enemyTank);
    }

    protected override void Fire()
    {
        base.Fire();
    }



    void LineUpShot()
    {
        //Target was found, Aim turret at target

    }

    //void LookForTarget()
    //{


    //}



    IEnumerator SearchState()
    {
        tankIsMoving = true;
        ///<summary> 
        /// Movement Block!
        /// </summary>
        newPosition = new GameObject("movePosition");
        newPosition.transform.position = transform.position;
        newPosition.transform.rotation = transform.rotation;
        newPosition.transform.parent = gameObject.transform;
        newPosition.transform.Translate(Vector3.forward * 15);

        goalToMoveTo = newPosition.transform.position;
        MoveTo(goalToMoveTo);

        yield return new WaitUntil(() => !shouldMove);
        Destroy(newPosition);

        tankIsMoving = false;

        
    }

    IEnumerator SearchAim()
    {
        runningSearchState = true;

        ///<summary>
        ///RotationBlock!
        /// </summary>
        GameObject NewRotationLeft = new GameObject("lookAtPosition");
        NewRotationLeft.transform.position = transform.position;
        NewRotationLeft.transform.rotation = transform.rotation;
        NewRotationLeft.transform.parent = gameObject.transform;
        NewRotationLeft.transform.Translate((Vector3.forward - Vector3.right) * 5);


        enemyTank = NewRotationLeft;
        AimAt(NewRotationLeft);



        yield return new WaitUntil(() => !aiming);
        Destroy(NewRotationLeft);


        GameObject NewRotationRight = new GameObject("lookAtPosition2");
        NewRotationRight.transform.position = transform.position;
        NewRotationRight.transform.rotation = transform.rotation;
        NewRotationRight.transform.parent = gameObject.transform;
        NewRotationRight.transform.Translate((Vector3.forward + Vector3.right) * 5);

        enemyTank = NewRotationRight;
        AimAt(NewRotationRight);



        yield return new WaitUntil(() => !aiming);
        Destroy(NewRotationRight);

        runningSearchState = false;


    }



    IEnumerator AlertState()
    {
        yield return null;
    }

    IEnumerator CombatState()
    {

        Fire();

        yield return null;

    }

    IEnumerator PanicState()
    {
        yield return null;
    }
    void TurnLeft()
    {
        newPosition.transform.Translate(Vector3.left * 3);
        Debug.Log("Turning Left");
        goalToMoveTo = newPosition.transform.position;
        MoveTo(goalToMoveTo);
    }

    void TurnRight()
    {
        newPosition.transform.Translate(Vector3.right * 3);
        Debug.Log("Turning Right");
        goalToMoveTo = newPosition.transform.position;
        MoveTo(goalToMoveTo);
    }

    void TurnStop()
    {
        somethingFront = false;
        somethingLeft = false;
        somethingFarLeft = false;
        somethingRight = false;
        somethingFarRight = false;

        newPosition.transform.position = transform.position;
        newPosition.transform.rotation = transform.rotation;
        newPosition.transform.parent = gameObject.transform;
        goalToMoveTo = newPosition.transform.position;
        newPosition.transform.Translate(Vector3.forward * 15);

    }


    void feelerCheck()
    {
        RaycastHit frontHit;
        RaycastHit leftHit;
        RaycastHit farLeftHit;
        RaycastHit rightHit;
        RaycastHit farRightHit;

        Ray frontRay = new Ray(frontFeelerPos.transform.position, (frontFeelerPos.transform.forward * rayLenght));
        Ray leftRay = new Ray(frontFeelerPos.transform.position, (leftFeelerPos.transform.forward * rayLenghtForward));
        Ray farLeftRay = new Ray(frontFeelerPos.transform.position, (farLeftFeelerPos.transform.forward * rayFarLenght));
        Ray rightRay = new Ray(frontFeelerPos.transform.position, (rightFeelerPos.transform.forward * rayLenghtForward));
        Ray farRightRay = new Ray(frontFeelerPos.transform.position, (farRightFeelerPos.transform.forward * rayFarLenght));

        //frontRay
        Debug.DrawRay(frontFeelerPos.transform.position, (frontFeelerPos.transform.forward * rayLenght), Color.red);
        //leftRay
        Debug.DrawRay(frontFeelerPos.transform.position, (leftFeelerPos.transform.forward * rayLenghtForward), Color.green);
        //farLeftRay
        Debug.DrawRay(frontFeelerPos.transform.position, (farLeftFeelerPos.transform.forward * rayFarLenght), Color.yellow);
        //rightRay
        Debug.DrawRay(frontFeelerPos.transform.position, (rightFeelerPos.transform.forward * rayLenghtForward), Color.green);
        //farRightRay
        Debug.DrawRay(frontFeelerPos.transform.position, (farRightFeelerPos.transform.forward * rayFarLenght), Color.yellow);

        int chooseLeftRight = Random.Range(1, 100);

        //BOOLEAN TRIGGERS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        if (Physics.Raycast(frontRay, out frontHit, rayLenght))
        {
            if (frontHit.collider.tag == "Prop")
            {
                Debug.Log("FrontHit!");
                somethingFront = true;
            }
        }

        if (Physics.Raycast(leftRay, out leftHit, rayLenghtForward))
        {
            if (leftHit.collider.tag == "Prop")
            {
                Debug.Log("LeftHit!");
                somethingLeft = true;
            }
        }

        if (Physics.Raycast(rightRay, out rightHit, rayLenghtForward))
        {
            if (rightHit.collider.tag == "Prop")
            {
                Debug.Log("RightHit!");
                somethingRight = true;
            }
        }

        if (Physics.Raycast(farLeftRay, out farLeftHit, rayLenghtForward))
        {
            if (farLeftHit.collider.tag == "Prop")
            {
                Debug.Log("farLeftHit!");
                somethingFarLeft = true;
            }
        }

        if (Physics.Raycast(farRightRay, out farRightHit, rayLenghtForward))
        {
            if (farRightHit.collider.tag == "Prop")
            {
                Debug.Log("farRightHit!");
                somethingFarRight = true;
            }
        }

        // END OF BOOLEAN TRIGGERS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        //BEHAVIOUR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        int leftRight = Random.Range(0, 1);

        if (somethingFront == true)
        {
            if (leftRight == 0)
            {
                TurnRight();
            }
            
            if(leftRight ==1)
            {
                TurnLeft();
            }
        }

        if (somethingLeft == true)
        {
            TurnRight();
        }
        if (somethingRight == true)
        {
            TurnLeft();
        }

        if ((somethingLeft == true) && (somethingFront == true))
        {
            TurnRight();
        }

        if ((somethingRight == true) && (somethingFront == true))
        {
            TurnLeft();
        }

        if ((somethingRight == true) && somethingFarRight == true)
        {
            TurnLeft();
        }

        if ((somethingLeft == true) && somethingFarLeft == true)
        {
            TurnLeft();
        }

        if ((somethingFront == true) && (somethingLeft == true) && (somethingRight == true))
        {
            Debug.Log("StopMoving!");
        }

        if (somethingFarLeft == true)
        {
            TurnStop();
        }

        if (somethingFarRight == true)
        {
            TurnStop();
        }

        else
        {
            TurnStop();
        }

        // END OF BEHAVIOUR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tank")
        {
            //StopCoroutine(searchRoutine);
            StopAllCoroutines();
            
            enemyTank = other.gameObject;
            

            AimAt(enemyTank);

            canShoot = true;
            Debug.Log("Shooting");
            StartCoroutine(CombatState());
        }

       

    }


}