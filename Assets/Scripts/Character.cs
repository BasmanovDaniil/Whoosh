using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float strafeSpeed = 4f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private AdvancedSettings advanced = new AdvancedSettings();
    [SerializeField] private bool lockCursor = true;

    [System.Serializable]
    public class AdvancedSettings
    {
        public float gravityMultiplier = 1f;
        public PhysicMaterial zeroFrictionMaterial;
        public PhysicMaterial highFrictionMaterial;
        public float groundStickyEffect = 5f;
    }

    public static Character instance;
    public bool grounded { get; private set; }

    private CapsuleCollider capsule;
    private const float jumpRayLength = 0.7f;
    private Vector2 input;
    private IComparer rayHitComparer;
    public Car car;

    private void Awake()
    {
        instance = this;
        // Set up a reference to the capsule collider.
        capsule = collider as CapsuleCollider;
        grounded = true;
        Screen.lockCursor = lockCursor;
        rayHitComparer = new RayHitComparer();
    }

    private void OnDisable()
    {
        Screen.lockCursor = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Screen.lockCursor = lockCursor;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0));
            RaycastHit[] hits = Physics.RaycastAll(ray, 100, 1 << LayerMask.NameToLayer("Vehicles"));
            System.Array.Sort(hits, rayHitComparer);
            if (hits.Length > 0)
            {
                Detach();
                hits[0].transform.GetComponentInParent<Car>().Attach();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Detach();
        }
    }

    private void Detach()
    {
        if (car != null)
        {
            car.Detach();
        }
    }

    public void FixedUpdate()
    {
        float speed = runSpeed;

        // Read input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool jump = Input.GetButton("Jump");

        // On standalone builds, walk/run speed is modified by a key press.
        // We select appropriate speed based on whether we're walking by default, and whether the walk/run toggle button is pressed:
        speed = runSpeed;

        // On mobile, it's controlled in analogue fashion by the v input value, and therefore needs no special handling.

        input = new Vector2(h, v);

        // normalize input if it exceeds 1 in combined length:
        if (input.sqrMagnitude > 1) input.Normalize();

        // Get a vector which is desired move as a world-relative direction, including speeds
        Vector3 desiredMove = transform.forward*input.y*speed + transform.right*input.x*strafeSpeed;

        // preserving current y velocity (for falling, gravity)
        float yv = rigidbody.velocity.y;

        // add jump power
        if (grounded && jump)
        {
            yv += jumpPower;
            grounded = false;
        }

        // Set the rigidbody's velocity according to the ground angle and desired move
        rigidbody.velocity = desiredMove + Vector3.up * yv;
        
        // Use low/high friction depending on whether we're moving or not
        if (desiredMove.magnitude > 0 || !grounded)
        {
            collider.material = advanced.zeroFrictionMaterial;
        }
        else
        {
            collider.material = advanced.highFrictionMaterial;
        }


        // Ground Check:

        // Create a ray that points down from the centre of the character.
        Ray ray = new Ray(transform.position, -transform.up);

        // Raycast slightly further than the capsule (as determined by jumpRayLength)
        RaycastHit[] hits = Physics.RaycastAll(ray, capsule.height*jumpRayLength);
        System.Array.Sort(hits, rayHitComparer);


        if (grounded || rigidbody.velocity.y < jumpPower*.5f)
        {
            // Default value if nothing is detected:
            grounded = false;
            // Check every collider hit by the ray
            for (int i = 0; i < hits.Length; i++)
            {
                // Check it's not a trigger
                if (!hits[i].collider.isTrigger)
                {
                    // The character is grounded, and we store the ground angle (calculated from the normal)
                    grounded = true;

                    // stick to surface - helps character stick to ground - specially when running down slopes
                    //if (rigidbody.velocity.y <= 0) {
                    rigidbody.position = Vector3.MoveTowards(rigidbody.position,
                        hits[i].point + Vector3.up*capsule.height*.5f, Time.deltaTime*advanced.groundStickyEffect);
                    //}
                    rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
                    break;
                }
            }
        }

        Debug.DrawRay(ray.origin, ray.direction*capsule.height*jumpRayLength, grounded ? Color.green : Color.red);

        // add extra gravity
        rigidbody.AddForce(Physics.gravity*(advanced.gravityMultiplier - 1));
    }

    //used for comparing distances
    private class RayHitComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((RaycastHit) x).distance.CompareTo(((RaycastHit) y).distance);
        }
    }
}