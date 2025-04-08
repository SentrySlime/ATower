using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageInterface
{

    void Damage(float damage);

    void Damage(float damage, bool criticalHit);

}
