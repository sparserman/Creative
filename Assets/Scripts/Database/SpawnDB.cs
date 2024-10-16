using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class SpawnDB : ScriptableObject
{
	public List<SpawnDBEntity> Tutorial; // Replace 'EntityType' to an actual type that is serializable.
}
