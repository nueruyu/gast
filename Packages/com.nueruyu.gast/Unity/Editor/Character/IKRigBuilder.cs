using System;
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
                Debug.Log($"Successfully set up Humanoid IK rig for {selectedObject.name}.");
            }
            else
            {
                SetupForGeneric(animator, rigGo, ikController);
                Debug.Log($"Attempted to set up Generic IK rig for {selectedObject.name}. Please verify the bone assignments in the Inspector.");
            }
        }

        static void SetupForHumanoid(Animator animator, GameObject rigGo, CharacterIKController ikController)
        {
            CreateAndAssignConstraint(ikController, AvatarIKGoal.RightHand, rigGo,
                animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
                animator.GetBoneTransform(HumanBodyBones.RightLowerArm),
                animator.GetBoneTransform(HumanBodyBones.RightHand)
            );
            CreateAndAssignConstraint(ikController, AvatarIKGoal.LeftHand, rigGo,
                animator.GetBoneTransform(HumanBodyBones.LeftUpperArm),
                animator.GetBoneTransform(HumanBodyBones.LeftLowerArm),
                animator.GetBoneTransform(HumanBodyBones.LeftHand)
            );
            CreateAndAssignConstraint(ikController, AvatarIKGoal.RightFoot, rigGo,
                animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
                animator.GetBoneTransform(HumanBodyBones.RightFoot)
            );
            CreateAndAssignConstraint(ikController, AvatarIKGoal.LeftFoot, rigGo,
                animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
                animator.GetBoneTransform(HumanBodyBones.LeftFoot)
            );
        }

        static void SetupForGeneric(Animator animator, GameObject rigGo, CharacterIKController ikController)
        {
            var allBones = animator.transform.GetComponentsInChildren<Transform>();

            FindAndCreateConstraint(ikController, AvatarIKGoal.RightHand, "Hand", new[] { "right", "r" }, rigGo, allBones);
            FindAndCreateConstraint(ikController, AvatarIKGoal.LeftHand, "Hand", new[] { "left", "l" }, rigGo, allBones);
            FindAndCreateConstraint(ikController, AvatarIKGoal.RightFoot, "Foot", new[] { "right", "r" }, rigGo, allBones);
            FindAndCreateConstraint(ikController, AvatarIKGoal.LeftFoot, "Foot", new[] { "left", "l" }, rigGo, allBones);
        }

        static void FindAndCreateConstraint(CharacterIKController controller, AvatarIKGoal goal, string primaryKeyword, string[] sideKeywords, GameObject rigGo, Transform[] allBones)
        {
            var tip = FindBone(allBones, primaryKeyword, sideKeywords);
            if (tip == null)
            {
                Debug.LogWarning($"Could not find tip bone for {goal}. Please assign it manually.");
                CreateAndAssignConstraint(controller, goal, rigGo, null, null, null);
                return;
            }

            var mid = tip.parent;
            if (mid == null)
            {
                Debug.LogWarning($"Could not find mid bone for {goal} (parent of {tip.name}). Please assign it manually.");
                CreateAndAssignConstraint(controller, goal, rigGo, null, null, tip);
                return;
            }

            var root = mid.parent;
            if (root == null)
            {
                Debug.LogWarning($"Could not find root bone for {goal} (parent of {mid.name}). Please assign it manually.");
                CreateAndAssignConstraint(controller, goal, rigGo, null, mid, tip);
                return;
            }

            Debug.Log($"Found bone chain for {goal}: Root=[{root.name}], Mid=[{mid.name}], Tip=[{tip.name}]");
            CreateAndAssignConstraint(controller, goal, rigGo, root, mid, tip);
        }

        static Transform FindBone(IEnumerable<Transform> bones, string primaryKeyword, string[] sideKeywords)
        {
            return bones.FirstOrDefault(b =>
            {
                var lowerName = b.name.ToLower();
                return lowerName.Contains(primaryKeyword.ToLower()) && sideKeywords.Any(s => lowerName.Contains(s));
            });
        }

        static void CreateAndAssignConstraint(CharacterIKController controller, AvatarIKGoal goal, GameObject rigGo, Transform root, Transform mid, Transform tip)
        {
            var goalGo = new GameObject($"{goal} IK");
            Undo.RegisterCreatedObjectUndo(goalGo, $"Create {goal} IK object");
            goalGo.transform.SetParent(rigGo.transform);

            var constraint = Undo.AddComponent<TwoBoneIKConstraint>(goalGo);
            constraint.data.root = root;
            constraint.data.mid = mid;
            constraint.data.tip = tip;

            var targetGo = new GameObject($"{goal} Target");
            Undo.RegisterCreatedObjectUndo(targetGo, $"Create {goal} Target");
            targetGo.transform.SetParent(rigGo.transform);
            constraint.data.target = targetGo.transform;

            var hintGo = new GameObject($"{goal} Hint");
            Undo.RegisterCreatedObjectUndo(hintGo, $"Create {goal} Hint");
            hintGo.transform.SetParent(rigGo.transform);
            constraint.data.hint = hintGo.transform;

            var references = new CharacterIKController.IKGoalReferences
            {
                Constraint = constraint,
                Target = targetGo.transform,
                Hint = hintGo.transform
            };

            controller.SetGoalReferences(goal, references);
            EditorUtility.SetDirty(controller);
        }
    }
}
