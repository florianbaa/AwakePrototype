using UnityEngine;
using System.Collections;


namespace VikingCrewTools.Sidescroller {
    /// <summary>
    /// This class controls the body of a character and handles all connections to things like inventory etc.
    /// </summary>
    public class CharacterController2D : MonoBehaviour {
        [Header("Hover over variable names to see tooltips!"), Tooltip("Increase this to make the character faster and stronger")]
        public float moveForce;
        [Tooltip("Increase this to make the character jump higher")]
        public float jumpVelocity;
        [Tooltip("This force is added to the character body when walking up an incline. Otherwise it might be very hard to wal up a hill or stairs")]
        public float upforceAtIncline;
        [Tooltip("When the character is crouching she should move slower. Thus we reduce the force a bit by multiplying with this value."), Range(0f,1f)]
        public float crouchingMoveForceMultiplier = 0.5f;

        public FirearmBehaviour gun {
            get {
                if (inventory == null) return null;
                return inventory.currentGun;
            }
        }
        
        #region Physics properties
        /* It has come to our attention that many who makes 2D games actually for different reasons need to still use the 3D physics
        engine. One such reason is that procedural generation of 2D colliders is pretty slow while the mesh collider for 3D is very
        performant. We therefore will from now on include the possibilitty to also use 3D physics. Unfortunately this makes the code
        a bit more complex but we hope you will bear with us as this makes the syste a lot more usable in future updates where we may
        include voxel world integrations for example.
    */
        private bool is2D { get { return rb2D; } }
        internal Vector2 velocity {
            get {
                if (is2D) return rb2D.velocity;
                else return rb3D.velocity;
            }
        }
        private float mass {
            get {
                if (is2D) return rb2D.mass;
                else return rb3D.mass;
            }
        }
        
        private Rigidbody2D rb2D;
        private Rigidbody rb3D;
        
        public BodyPartCollider mainBody;
        public BodyPartCollider feet;
        [HideInInspector]
        public BodyPartCollider[] bodyParts;
        #endregion Physics properties

        public bool isGrounded;
        public bool isFacingRight;
        public bool isCrouching;
        public LayerMask groundableMaterials;
        public Animator bodyAnimator;

        public new AudioSource audio;
        public AudioClip jumpSound;
        public AudioClip landSound;

        public int maxDoubleJumps = 1;
        [SerializeField]
        private int currentDoubleJumps = 0;

        [HideInInspector]
        public InventoryBehaviour inventory;

        private bool hasBeenSetup = false;
        // Use this for initialization
        void Start() {
            if (!hasBeenSetup)
                Setup();
        }

        public void Setup() {
            bodyParts = feet.transform.parent.GetComponentsInChildren<BodyPartCollider>();
            rb2D = GetComponent<Rigidbody2D>();
            rb3D = GetComponent<Rigidbody>();

            audio = GetComponent<AudioSource>();
            inventory = GetComponent<InventoryBehaviour>();
            hasBeenSetup = true;
        }

        // Update is called once per frame
        void Update() {

            UpdateAnimationController();
        }

        void FixedUpdate() {
            UpdateGrounded();
        }

        public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force) {
            if (is2D)
                rb2D.AddForce(force, mode);
            else
                rb3D.AddForce(force, (ForceMode)mode);
        }

        public void Run(float direction) {
            if (direction == 0) return;

            if (Mathf.Abs(direction) > 1)
                direction /= Mathf.Abs(direction);
            if (isCrouching)
                AddForce(new Vector2(direction * moveForce * crouchingMoveForceMultiplier, 0));
            else
                AddForce(new Vector2(direction * moveForce, 0));

            //If we're on an incline add some force upwards
            if (IsIncline(direction > 0)) {
                Vector2 force = Vector2.up * upforceAtIncline;
                AddForce(force);
            }

        }

        /// <summary>
        /// Tries to determine if this character is walking up a slope. If so then we add a force upwards to reduce friction
        /// Think of this like walking up stairs where you would push yourself upwards a bit
        /// </summary>
        /// <returns></returns>
        bool IsIncline(bool rightSide) {
            return feet.FeelInFront(rightSide, groundableMaterials);
        }

        public void Jump() {
            if (isGrounded || currentDoubleJumps <= maxDoubleJumps) {
                //This represents a velocity change up to the velocity of jumpVelocity
                AddForce(Vector2.up * (jumpVelocity - velocity.y) * mass, ForceMode2D.Impulse);
                audio.PlayOneShot(jumpSound);

                currentDoubleJumps++;
            }
        }

        public void Fire() {
            if (gun != null) {
                if (gun.currentAmmo == 0)
                    inventory.ReloadCurrentWeapon();

                gun.Fire();
            }
        }

        public void Reload() {
            inventory.ReloadCurrentWeapon();
        }

        public void AutoFire() {
            if (gun != null)
                gun.AutoFire();
        }

        public void ReleaseAutoFire() {
            if (gun != null)
                gun.ReleaseAutoFire();
        }

        /// <summary>
        /// Makes the weapon follow a world space position
        /// </summary>
        /// <param name="position"></param>
        public void AimAtPosition(Vector3 position) {

            Vector3 rot = bodyAnimator.transform.rotation.eulerAngles;
            isFacingRight = position.x >= transform.position.x;
            if (isFacingRight) {
                rot.y = 90;
            } else {
                rot.y = 270;
            }
            bodyAnimator.transform.rotation = Quaternion.Euler(rot);

            if (gun != null) {
                position.z = gun.transform.position.z;
                gun.transform.LookAt(position, Vector3.up);
            }
        }

        public void AimAtDirection(float angle) {

            //TODO
            if (gun != null) {
                //gun.transform.rotation = Quaternion.Euler(0,0,angle);
            }
        }

        public void AimAtDirection(Vector3 direction) {
            Vector3 rot = bodyAnimator.transform.rotation.eulerAngles;
            isFacingRight = direction.x >= 0;
            if (isFacingRight) {
                rot.y = 90;
            } else {
                rot.y = 270;
            }
            //Turns the animated body to point the right direction
            bodyAnimator.transform.rotation = Quaternion.Euler(rot);
            if (gun != null) {
                if (direction == Vector3.zero)
                    direction = new Vector3(isFacingRight ? 1 : -1, 0, 0);
                gun.transform.forward = direction.normalized;

            }
        }

        public void UpdateAim(Transform target) {
            AimAtPosition(target.position);
        }

        void UpdateGrounded() {
            
            if (CheckForGround()) {
                if (!isGrounded) {
                    //Just landed so need to set collider sizes
                    foreach (var bodypart in bodyParts) {
                        bodypart.SetOriginalSizeAndPos();
                    }
                    audio.PlayOneShot(landSound);
                }
                isGrounded = true;
                currentDoubleJumps = 0;
            } else {
                
                if (isGrounded) {//Just took off so need to set collider sizes
                    foreach (var bodypart in bodyParts) {
                        bodypart.SetJumpSizeAndPos();
                    }
                }
                isGrounded = false;
            }
        }

        private bool CheckForGround() {
            Vector2 circleCastStart = (Vector2)feet.transform.position + 0.5f * Vector2.up;
            Vector2 circleCastDirection = Vector2.down;
            float radius = feet.GetWidth() / 2;
            float circleCastDistance = 0.5f + (transform.position - feet.transform.position).magnitude + feet.GetWidth() / 2f;
            bool didHit = false;

            if (is2D) {
                RaycastHit2D hit = Physics2D.CircleCast(circleCastStart, radius, circleCastDirection, circleCastDistance, groundableMaterials);
                didHit = hit.collider != null;
            } else {
                RaycastHit hit;
                Physics.SphereCast(circleCastStart,  radius, circleCastDirection, out hit,  circleCastDistance, groundableMaterials);
                didHit = hit.collider != null;
            }

            if(didHit)
                DebugDrawPhysics.DebugDrawCircleCast(circleCastStart, radius, circleCastDirection, circleCastDistance, Color.red);
            else
                DebugDrawPhysics.DebugDrawCircleCast(circleCastStart, radius, circleCastDirection, circleCastDistance, Color.green);

            return didHit;
        }

        void UpdateAnimationController() {
            bodyAnimator.SetBool("OnGround", isGrounded);
            bodyAnimator.SetFloat("Forward", (isFacingRight ? velocity.x : -velocity.x));
        }

        public void Crouch(bool doCrouch) {
            doCrouch = doCrouch && isGrounded;

            //If we want to stand up then we need to check if there is room for it
            if (!doCrouch && isCrouching) {
                //TODO: maybe check for all bodyparts? Maybe even use a shape cast in potential position and size?
                float distance = mainBody.GetHeight() / mainBody.crouchSizeRatio / 2;
                Vector3 origin = mainBody.transform.position + 0.1f * Vector3.up;
                if (is2D && Physics2D.Raycast(origin, Vector2.up, distance, groundableMaterials))
                    return;
                if (!is2D && Physics.Raycast(origin, Vector3.up, distance, groundableMaterials))
                    return;
            }

            bodyAnimator.SetBool("Crouch", doCrouch);
            //Make body smaller when crouching
            if (doCrouch && !isCrouching) {
                foreach (var bodypart in bodyParts) {
                    bodypart.SetCrouchSizeAndPos();
                }
            }
            if (!doCrouch && isCrouching) {
                foreach (var bodypart in bodyParts) {
                    bodypart.SetOriginalSizeAndPos();
                }
            }

            isCrouching = doCrouch;
        }

        public void HandleDeathCallback() {
            foreach (var bodypart in bodyParts) {
                bodypart.DisableCollider();
            }
        }
    }
}