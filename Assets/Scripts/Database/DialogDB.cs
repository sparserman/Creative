using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class DialogDB : ScriptableObject
{
	public List<DialogDBEntity> Tutorial; // Replace 'EntityType' to an actual type that is serializable.
}
