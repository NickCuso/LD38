using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
  public class ExplosionPhysicsForce : MonoBehaviour
  {
    public float explosionForce = 4;


    private IEnumerator Start()
    {


      if(PhotonView.Get(this).isMine)
      {

        // wait one frame because some explosions instantiate debris which should then
        // be pushed by physics force
        yield return null;

        float multiplier = 1;//GetComponent<ParticleSystemMultiplier>().multiplier;
        //multiplier *= transform.localScale.x;
        multiplier *= transform.localScale.x;
        float r = 10 * multiplier;
        var cols = Physics.OverlapSphere(transform.position, r, LayerMask.GetMask(new[] { "Character", "default" }));
        var rigidbodies = new List<Rigidbody>();
        foreach(var col in cols)
        {
          if(col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
          {
            rigidbodies.Add(col.attachedRigidbody);
          }
        }
        SoundManager.Play(SoundManager.instance.explode,  Mathf.Max(.1f, Mathf.Min(.6f, multiplier * .6f)));

        foreach(var rb in rigidbodies)
        {
          rb.AddExplosionForce(explosionForce * multiplier, transform.position, r, 1 * multiplier, ForceMode.Impulse);

          if(PhotonView.Get(this).isMine)
          {
            var lifeLine = rb.GetComponent<LifeLine>();
            if(lifeLine != null)
            {
              print(explosionForce * multiplier);
              lifeLine.life -= explosionForce * multiplier;
            }
          }
        }
      }
    }
  }
}
