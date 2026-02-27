using System;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.Characters
{
    public class CharacterHost : MonoBehaviour
    {
        ICharacter character;

        public ICharacter Character

        {
            get
            {
                if (character == null)
                    throw new InvalidOperationException("Character is not assigned");

                return character;
            }
        }

        public void AssignCharacter(ICharacter character)
        {
            this.character = character;
        }
    }
}