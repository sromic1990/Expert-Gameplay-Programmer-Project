using UnityEngine;

public class Walker : Enemy
{
    public float speed = 1f;
    internal LayerMask mask = 1;

    internal bool movingRight;
    
    internal override void WakeUp()
    {
        base.WakeUp();
        movingRight = Random.Range(0, 2) < 1 ? true : false;
    }

    void FixedUpdate()
    {
        if (isVulnerable)
        {
            LookAhead();
            rb.velocity = new Vector2(speed * GetDirection(), rb.velocity.y);
        }
    }

    internal virtual void LookAhead()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(this.transform.position + GetDirection() * 0.35f * Vector3.right, Vector2.down, 1.5f, mask);
        if (rayHit.collider == null)
        {
            // Don't fall!
            ChangeDirection();
        }
        else
        {
            rayHit = Physics2D.Raycast(this.transform.position, GetDirection() * Vector3.right, 1f, mask);
            if (rayHit.collider != null)
            {
                // Don't run into walls!
                ChangeDirection();
            }
        }
    }

    internal void ChangeDirection()
    {
        movingRight = !movingRight;
    }

    internal float GetDirection()
    {
        return (movingRight ? 1f : -1f);
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Vector3 temp = this.transform.position + GetDirection() * 0.35f * Vector3.right;
    //    Gizmos.DrawLine(temp, temp + 1.5f * Vector3.down);
    //}
}
