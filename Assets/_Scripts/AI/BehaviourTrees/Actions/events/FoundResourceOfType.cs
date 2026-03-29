using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Found Resource Of Type")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Found Resource Of Type", message: "Found [Resource] Of [Strength]", category: "Events", id: "ef0e58af9a553ac9a93bc013e3d9bd6d")]
public sealed partial class FoundResourceOfType : EventChannel<Resource, float> { }

