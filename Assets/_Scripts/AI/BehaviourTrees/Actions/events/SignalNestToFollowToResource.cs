using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Signal Nest To Follow to Resource")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Signal Nest To Follow to Resource", message: "Signal Nest to FOllow [Agent] to Resource", category: "Events", id: "89420f7d812d98687a768ad21b1d8ef3")]
public sealed partial class SignalNestToFollowToResource : EventChannel<GameObject> { }

