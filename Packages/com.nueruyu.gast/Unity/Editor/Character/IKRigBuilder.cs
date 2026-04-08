using System.Collections.Generic;
using System.Linq;
using Gast.Unity.Features.Characters.IK;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Gast.Unity.Editor.Character
{
    public static class IKRigBuilder
    {
        static readonly (string Label, string Primary, string[] Sides)[] LimbDefinitions =
        {
            ("RightHand", "Hand", new[] { "right", "r" }),
            ("LeftHand",  "Hand", new[] { "left",  "l" }),
            ("RightFoot", "Foot", new[] { "right", "r" }),
            ("LeftFoot",  "Foot", new[] { "left",  "l" }),
        };

        [MenuItem("Gast/Tools/Setup IK Rig for Character")]
        static void SetupIKRig()
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                Debug.LogError("Please select a character GameObject in the hierarchy.");
                return;
            }

            var animator = selectedObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Selected GameObject does not have an Animator component.");
                return;
            }

            Undo.SetCurrentGroupName("Setup IK Rig");

            var rigBuilder = Undo.AddComponent<RigBuilder>(selectedObject);
            var rigGo = new GameObject("IK Rig");
            Undo.RegisterCreatedObjectUndo(rigGo, "Create IK Rig object");
            rigGo.transform.SetParent(selectedObject.transform);
            var rig = Undo.AddComponent<Rig>(rigGo);
            rigBuilder.layers.Add(new RigLayer(rig));

            var ikController = Undo.AddComponent<CharacterIKController>(selectedObject);

            if (animator.isHuman)
            {
                SetupForHumanoid(animator, rigGo, ikController);
                Debug.Log($"Successfully set up Humanoid IK rig for {selectedObject.name}. Assign AttachmentAnchorSymbols to each IK goal binding in the Inspector.");
            }
            else
            {
                SetupForGeneric(animator, rigGo, ikController);
                Debug.Log($"Attempted to set up Generic IK rig for {selectedObject.name}. Verify bone assignments and assign AttachmentAnchorSymbols in the Inspector.");
            }
        }

        static void SetupForHumanoid(Animator animator, GameObject rigGo, CharacterIKController ikController)
        {
            CreateAndAssignConstraint(ikController, "RightHand", rigGo,
                animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
                animator.GetBoneTransform(HumanBodyBones.RightLowerArm),
                animator.GetBoneTransform(HumanBodyBones.RightHand));

            CreateAndAssignConstraint(ikController, "LeftHand", rigGo,
                animator.GetBoneTransform(HumanBodyBones.LeftUpperArm),
                animator.GetBoneTransform(HumanBodyBones.LeftLowerArm),
                animator.GetBoneTransform(HumanBodyBones.LeftHand));

            CreateAndAssignConstraint(ikController, "RightFoot", rigGo,
                animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
                animator.GetBoneTransform(HumanBodyBones.RightFoot));

            CreateAndAssignConstraint(ikController, "LeftFoot", rigGo,
                animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
                animator.GetBoneTransform(HumanBodyBones.LeftFoot));
        }

        static void SetupForGeneric(Animator animator, GameObject rigGo, CharacterIKController ikController)
        {
            var allBones = animator.transform.GetComponentsInChildren<Transform>();

            foreach (var def in LimbDefinitions)
                FindAndCreateConstraint(ikController, def.Label, def.Primary, def.Sides, rigGo, allBones);
        }

        static void FindAndCreateConstraint(
            CharacterIKController controller,
            string label,
            string primaryKeyword,
            string[] sideKeywords,
            GameObject rigGo,
            Transform[] allBones)
        {
            var tip = FindBone(allBones, primaryKeyword, sideKeywords);
            if (tip == null)
            {
                Debug.LogWarning($"Could not find tip bone for {label}. Please assign it manually.");
                CreateAndAssignConstraint(controller, label, rigGo, null, null, null);
                return;
            }

            var mid = tip.parent;
            if (mid == null)
            {
                Debug.LogWarning($"Could not find mid bone for {label} (parent of {tip.name}). Please assign it manually.");
                CreateAndAssignConstraint(controller, label, rigGo, null, null, tip);
                return;
            }

            var root = mid.parent;
            if (root == null)
            {
                Debug.LogWarning($"Could not find root bone for {label} (parent of {mid.name}). Please assign it manually.");
                CreateAndAssignConstraint(controller, label, rigGo, null, mid, tip);
                return;
            }

            Debug.Log($"Found bone chain for {label}: Root=[{root.name}], Mid=[{mid.name}], Tip=[{tip.name}]");
            CreateAndAssignConstraint(controller, label, rigGo, root, mid, tip);
        }

        static Transform FindBone(IEnumerable<Transform> bones, string primaryKeyword, string[] sideKeywords)
        {
            return bones.FirstOrDefault(b =>
            {
                var lowerName = b.name.ToLower();
                return lowerName.Contains(primaryKeyword.ToLower()) && sideKeywords.Any(s => lowerName.Contains(s));
            });
        }

        static void CreateAndAssignConstraint(
            CharacterIKController controller,
            string label,
            GameObject rigGo,
            Transform root,
            Transform mid,
            Transform tip)
        {
            var goalGo = new GameObject($"{label} IK");
            Undo.RegisterCreatedObjectUndo(goalGo, $"Create {label} IK object");
            goalGo.transform.SetParent(rigGo.transform);

            var constraint = Undo.AddComponent<TwoBoneIKConstraint>(goalGo);
            constraint.data.root = root;
            constraint.data.mid = mid;
            constraint.data.tip = tip;

            var targetGo = new GameObject($"{label} Target");
            Undo.RegisterCreatedObjectUndo(targetGo, $"Create {label} Target");
            targetGo.transform.SetParent(rigGo.transform);
            constraint.data.target = targetGo.transform;

            var hintGo = new GameObject($"{label} Hint");
            Undo.RegisterCreatedObjectUndo(hintGo, $"Create {label} Hint");
            hintGo.transform.SetParent(rigGo.transform);
            constraint.data.hint = hintGo.transform;

            var so = new SerializedObject(controller);
            var listProp = so.FindProperty("goalBindings");
            listProp.arraySize++;
            var element = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
            element.FindPropertyRelative("Symbol").objectReferenceValue = null;
            var refProp = element.FindPropertyRelative("References");
            refProp.FindPropertyRelative("Constraint").objectReferenceValue = constraint;
            refProp.FindPropertyRelative("Target").objectReferenceValue = targetGo.transform;
            refProp.FindPropertyRelative("Hint").objectReferenceValue = hintGo.transform;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(controller);
        }
    }
}
