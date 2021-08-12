using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Priority = Log.Priority;

public class Weapon : MonoBehaviour {

    /* --- DEBUG --- */
    protected Priority debugPrio = Priority.COLLISION;
    protected string debugTag = "[HITBOX]: ";

    /* --- COMPONENTS ---*/
    public WeaponRenderer _renderer;

    /* --- VARIABLES --- */
    [HideInInspector] List<Hitbox> container = new List<Hitbox>();
    public int attackDamage = 1;
    public float force = 1f;
    public float knockDuration = 0.15f;


    /* --- UNITY --- */
    void OnTriggerEnter2D(Collider2D collider) {
        Attack(collider);
    }

    /* --- METHODS --- */
    void Attack(Collider2D collider) {
        // if (_renderer.currAnimation != null && _renderer.currAnimation.frameIndex > 2) {

            // add the item if it is in the container and has the correct tag
            if (collider.tag == "Hitbox" && collider.GetComponent<Hitbox>() != null && !container.Contains(collider.GetComponent<Hitbox>())) {
                Hitbox hitbox = collider.GetComponent<Hitbox>();
                container.Add(hitbox);
                OnAttack(hitbox);
            }

        // }

    }

    public void Reset() {

        container = new List<Hitbox>();

    }

    /* --- VIRTUAL --- */

    // on adding
    public virtual void OnAttack(Hitbox hitbox) {
        Log.Write(hitbox.state.name + " is being hit by a " + name, debugPrio, debugTag);

        hitbox.state.Hurt(attackDamage);
        Vector3 direction = hitbox.state.transform.position - transform.position;
        hitbox.state.Knock(force, direction, knockDuration);

    }

}
