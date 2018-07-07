using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    { 
        [SerializeField] private float m_MaxSpeed = 10f;
        [SerializeField] private float m_JumpForce = 400f;
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;
        [SerializeField] private bool m_AirControl = false;
        public float maxVerticalVelocity = 10f;
        public LayerMask m_WhatIsGround;

        private Transform m_GroundCheck;
        public float k_GroundedRadius = 0.1f;
        private bool m_Grounded;
        private Transform m_CeilingCheck;
        const float k_CeilingRadius = .01f;
        private Animator m_Anim;
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            m_Grounded = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    if (colliders[i].isTrigger)
                        continue;

                    m_Grounded = true;
                    break;
                }
            }

            m_Anim.SetBool("Ground", m_Grounded);

            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Mathf.Min(m_Rigidbody2D.velocity.y, maxVerticalVelocity));

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }

        public bool IsOnGround()
        {
            return m_Grounded;
        }

        public void Move(float move, bool crouch, bool jump)
        {
            // If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        void OnDrawGizmos()
        {
            if (m_GroundCheck == null)
                return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
        }
    }
}
