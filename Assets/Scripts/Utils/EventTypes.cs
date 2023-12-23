using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Event
{
  [Serializable] public class Float : UnityEvent<float> { }
  [Serializable] public class DoubleFloat : UnityEvent<float, float> { }

  [Serializable] public class DoubleVector2 : UnityEvent<Vector2, Vector2> { }

  [Serializable] public class ETransform : UnityEvent<Transform> { }
}
