using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
  // Like clamp, but compares to min and max without signs
  static public float UnsignedClamp(float value, float min, float max)
  {
    if (Mathf.Abs(value) < Mathf.Abs(min))
      return Mathf.Abs(min) * Mathf.Sign(value);

    if (Mathf.Abs(value) > Mathf.Abs(max))
      return Mathf.Abs(max) * Mathf.Sign(value);

    return value;
  }
}
