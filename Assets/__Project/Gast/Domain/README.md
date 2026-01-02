# Gast.Core

Foundation assembly containing pure C# interfaces and utilities with no Unity dependencies.

## Role

This assembly provides the core data contracts, interfaces, and platform-agnostic utilities that other assemblies depend on. It serves as the foundation layer that can be used in both Unity and non-Unity contexts.

## What Belongs Here

- **Pure C# interfaces** with no Unity dependencies (e.g., `IInteractable`, `IInteractionConfig`)
- **Enums and value types** that define game-wide concepts (e.g., `InteractionType`)
- **Data contracts** that establish common protocols between assemblies
- **Platform-agnostic utilities** that don't require Unity-specific APIs
- **Core domain logic** that is independent of Unity's lifecycle

## What Doesn't Belong Here

- MonoBehaviour classes (those belong in implementation assemblies)
- Unity-specific types (GameObject, Transform, etc.)
- Serialization attributes like `[SerializeField]` or `[Serializable]`
- Any code that depends on UnityEngine or other Unity packages

## Dependencies

- **.NET Standard 2.1**: Pure C# only
- **No Unity dependencies**: Can be used in non-Unity contexts

## Key Principles

### Interface-Only Design

Core defines **data interfaces** without implementations. This allows:

- Unity assemblies to provide MonoBehaviour implementations
- Testing without Unity dependencies
- Future platform flexibility

### Data vs Behavior Separation

Interfaces in Core should contain only data properties, not behavior methods:

- ✅ `IInteractable` has `Config` and `CanInteract` properties
- ❌ `IInteractable` should NOT have `OnInteract()` methods

Behavior is provided by implementation assemblies (e.g., `InteractableBase` in `Gast.Interactions`).

## Integration

Other assemblies implement Core interfaces:

- **Gast.Interactions**: Unity MonoBehaviour implementations
- **Future assemblies**: Can provide alternative implementations
