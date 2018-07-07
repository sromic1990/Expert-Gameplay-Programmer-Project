using UnityEngine;

public class Platform : MonoBehaviour
{
	void Awake()
	{
        this.tag = GameplayConstants.TAG_Ground;
        
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer> ();
		if (spriteRenderer != null)
		{
            MatchColliderToSpriteSize(spriteRenderer);
		}
	}

    private void MatchColliderToSpriteSize(SpriteRenderer spriteRenderer)
    {
        BoxCollider2D coll = this.GetComponent<BoxCollider2D>();
        if (coll == null)
        {
            coll = this.gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
        }

        coll.size = spriteRenderer.size - 2f * GameplayConstants.SLIP_ZONE_WIDTH * Vector2.right;
        coll.offset = 0.5f * spriteRenderer.size.y * Vector2.up;

        AddSlipSide(spriteRenderer, true);
        AddSlipSide(spriteRenderer, false);
    }

    private void AddSlipSide(SpriteRenderer spriteRenderer, bool isLeft)
    {
        PhysicsMaterial2D slipMaterial = (PhysicsMaterial2D)Resources.Load("SlipSurface", typeof(PhysicsMaterial2D));
        
        BoxCollider2D coll = this.gameObject.AddComponent<BoxCollider2D>();
        coll.size = new Vector2(GameplayConstants.SLIP_ZONE_WIDTH, spriteRenderer.size.y);
        coll.offset = new Vector2((isLeft ? -0.5f : 0.5f) * (spriteRenderer.size.x - GameplayConstants.SLIP_ZONE_WIDTH), 0.5f * spriteRenderer.size.y);
        coll.sharedMaterial = slipMaterial;
    }
}
