using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  An entity is an abstract object
/// </summary>
public interface IEntity
{

}

/// <summary>
///  Completely abstract
///  Represents a entity that has no interactions on it for now
/// </summary>
public class Entity : MonoBehaviour, IEntity
{

}
