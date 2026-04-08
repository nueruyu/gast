using System;
using System.Collections.Generic;
using Gast.Unity.Shared.Attachments;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Gast.Unity.Features.Characters.IK
{
    public class CharacterIKController : MonoBehaviour
    {
        [SerializeField]
        List<IKGoalBinding> goalBindings = new List<IKGoalBinding>();

        Dictionary<AttachmentAnchorSymbol, IKGoalReferences> goals;

        void Awake()
        {
            goals = new Dictionary<AttachmentAnchorSymbol, IKGoalReferences>();
            foreach (var binding in goalBindings)
                if (binding.Symbol != null)
                    goals[binding.Symbol] = binding.References;
        }

        public void SetIKTargetPose(AttachmentAnchorSymbol symbol, Transform target)
        {
            if (!goals.TryGetValue(symbol, out var references)) 
                return;
            if (references.Target != null && target != null)
            {
                references.Target.position = target.position;
                references.Target.rotation = target.rotation;
            }
        }

        public void SetGoalReferences(AttachmentAnchorSymbol symbol, IKGoalReferences references)
        {
            goals[symbol] = references;
        }

        [Serializable]
        public class IKGoalBinding
        {
            public AttachmentAnchorSymbol Symbol;
            public IKGoalReferences References;
        }

        [Serializable]
        public class IKGoalReferences
        {
            public TwoBoneIKConstraint Constraint;
            public Transform Target;
            public Transform Hint;
        }
    }
}
