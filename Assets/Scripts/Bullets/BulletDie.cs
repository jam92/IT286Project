using UnityEngine;
using System.Collections;

public class BulletDie : MonoBehaviour {

    public float LifeTime;
    private bool m_dead;
    private float m_timeUntilDie;
	// Use this for initialization
	void Start () {
        m_timeUntilDie = Time.time + LifeTime;
        m_dead = false;
	}

    // Update is called once per frame
    void Update()
    {
        if (m_timeUntilDie < Time.time && !m_dead == true)
        {
            Destroy();
        }
    }

    IEnumerator DestroyBullet()
    {

        if (GetComponent<Rigidbody2D>())
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(.1f);
        try
        {
            GameManager.singleton.BulletManager.GetComponent<BulletManager>().Bullets.Remove(gameObject);
        }
        catch
        {
            Debug.Log("Bullet is Not in BulletList");
        }
        Destroy(gameObject);
    }
    void HandleCollisions(Collider2D col) {
        //Don't die from deflections
        if (col.tag == "Weapon" || col.tag == "Item")
        {
        }
        //Hit the player
        else if (col.tag == "Player")
        {
            if (GetComponent<SpriteRenderer>())
            {
                if (GetComponent<SpriteRenderer>().color != Color.yellow)
                {
                    col.GetComponent<PlayerControl>().DamagePlayer();
                }
            }
            Destroy();
        }
        //Handle friendly fire
        else if (col.tag == "Enemy") {
            if (GetComponent<SpriteRenderer>())
            {
                if (GetComponent<SpriteRenderer>().color == Color.yellow)
                {
                    col.GetComponent<EnemyHealth>().getHit(4, Vector2.zero);
                    col.GetComponent<EnemyAI>().KnockBack(-(transform.position - col.transform.position).normalized,2.0f);
                    Destroy();
                }
            }
        }
        //Hit a wall or something
        else {
            Destroy();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        HandleCollisions(col);
    }
    void OnTriggerStay2D(Collider2D col) {
        HandleCollisions(col);
    }
    void Destroy()
    {
        m_dead = true;
        if (GetComponent<Animator>())
        {
            GetComponent<Animator>().SetBool("Dead", m_dead);
            GetComponent<BoxCollider2D>().enabled = false;
            
        }
        StartCoroutine(DestroyBullet());
    }
}
