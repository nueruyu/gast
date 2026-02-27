using Gast.Domain.Characters;
using System;
using UnityEngine;

namespace Gast.Features.Characters
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