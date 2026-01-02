# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity game project that uses **Descrio**, a YAML-based scripting interpreter for game logic, dialogues, and event sequences. The project integrates Descrio as a Unity package via a symbolic link from a git submodule.

## Architecture

### Package Management Structure

- **External/descrio-dotnet/**: Git submodule containing the Descrio .NET library and Unity package
  - Core library is .NET Standard (platform-agnostic)
  - Unity package source is in `Descrio.Unity/Packages/com.nueruyu.descrio/`
- **Packages/com.nueruyu.descrio**: Symbolic link to `External/descrio-dotnet/Descrio.Unity/Packages/com.nueruyu.descrio`
  - Created by running `Tools/setup-symlink.sh` or `Tools/setup-symlink.ps1`
  - This symlink is gitignored and must be recreated on each machine

### Unity Configuration

- Uses **Universal Render Pipeline (URP)** with separate configurations:
  - Mobile: `Assets/Settings/Mobile_RPAsset.asset`
  - PC: `Assets/Settings/PC_RPAsset.asset`
- Uses **Unity Input System** (new input system, not legacy)
  - Input actions defined in `Assets/InputSystem_Actions.inputactions`

### Third-Party Libraries

The project uses the following third-party libraries:

- **[R3](https://github.com/Cysharp/R3)** - Reactive Extensions for Unity
  - Modern reactive programming library by Cysharp
  - Provides observables, subjects, and reactive operators
  - Used for event-driven architecture and data flow management

- **[UniTask](https://github.com/Cysharp/UniTask)** - Efficient async/await for Unity
  - High-performance async/await implementation optimized for Unity
  - Zero allocation async operations
  - Integrates with Unity's PlayerLoop for frame-based operations

- **[VContainer](https://vcontainer.hadashikick.jp/)** - Lightweight DI container
  - Fast and lightweight dependency injection framework
  - Designed specifically for Unity with performance in mind
  - Supports constructor injection, property injection, and method injection

- **[NuGet for Unity](https://github.com/GlitchEnzo/NuGetForUnity)** - NuGet package manager
  - Enables use of .NET NuGet packages in Unity projects
  - Packages installed in `Assets/Packages/` (gitignored, restored from `packages.config`)
  - Configuration in `Assets/NuGet.config`

- **[Unity MCP](https://github.com/CoplayDev/unity-mcp)** - Model Context Protocol server for Unity
  - Enables AI assistant integration with Unity Editor
  - Allows automated scene setup, GameObject manipulation, and script management
  - Server runs on `localhost:9999`, configured in `.mcp.json`

## Essential Commands

### Initial Setup

After cloning the repository, you must create the Descrio package symlink:

**PowerShell:**
```powershell
.\Tools\setup-symlink.ps1
```

**Git Bash/WSL:**
```bash
bash Tools/setup-symlink.sh
```

**Note:** On Windows, symlink creation may require:
- Running PowerShell as Administrator, OR
- Enabling Developer Mode in Windows Settings

### Working with Descrio Submodule

The Descrio library is in `External/descrio-dotnet/` as a git submodule. When making changes to Descrio:

1. Navigate to the submodule: `cd External/descrio-dotnet/`
2. Build the .NET library: `dotnet build`
   - This automatically copies DLLs to `Descrio.Unity/Packages/com.nueruyu.descrio/Runtime/`
3. Unity will automatically detect the updated DLLs through the symlink

To run Descrio core tests (non-Unity):
```bash
cd External/descrio-dotnet/
dotnet test
```

## Descrio Integration

Descrio allows writing game logic in YAML files that can be edited without recompiling C#. Key features:

- **C# Methods Exposure**: Use `[Callable]` attribute to expose C# methods to YAML scripts
- **YAML Tags**: `!run`, `!let`, `!when`, `!match`, `!for`, `!while`, `!try`, `!expr`, etc.
- **Async Support**: Scripts can call and await async C# methods with `!dispatch` and `!run all/any`

When implementing game features that use Descrio:
1. Create a MonoBehaviour with `[Callable]` methods
2. Initialize `ScriptRunner` with a module loader and register callables via `.AddCallables(this)`
3. Execute YAML scripts with `await runner.ExecuteAsync(modulePath, scopePath)`

Refer to `External/descrio-dotnet/README.md` for detailed syntax and examples.

## Code Organization

### Assembly Structure

The project uses a flat modular structure with assembly definition files:

```
Assets/__Project/
├── DescrioGames.Core/          # Foundation utilities (no dependencies)
├── DescrioGames.Inputs/        # Input System wrapper
├── DescrioGames.Characters/    # Character controllers and movement
├── DescrioGames.Cameras/       # Third-person camera systems
├── DescrioGames.UI/            # UI components (dialogue, HUD, menus)
├── DescrioGames.Scripting/     # Descrio [Callable] implementations
└── DescrioGames.Gameplay/      # Main game integration layer
```

**Dependency flow:**
```
Gameplay → Scripting → (Characters, Cameras, UI) → Inputs/Core
```

Each module has:
- `ModuleName.asmdef` at the root
- `Runtime/` folder containing scripts
- Namespace matching the module name (e.g., `DescrioGames.Characters`)

### Namespace Convention

Namespaces follow the folder structure:
- Module root: `DescrioGames.ModuleName`
- Subfolders: `DescrioGames.ModuleName.SubfolderName`

Example:
```csharp
// Assets/__Project/DescrioGames.Characters/Runtime/Movement/CharacterMover.cs
namespace DescrioGames.Characters.Movement
{
    public class CharacterMover { }
}
```

## Coding Conventions

### Access Modifiers

**Do not use explicit `private` modifiers.** C# defaults to private, so omit the keyword for cleaner code.

**Good:**
```csharp
public class Example : MonoBehaviour
{
    [SerializeField] float speed = 5f;  // Implicitly private
    Transform cachedTransform;          // Implicitly private

    void Awake() { }                    // Implicitly private
    void Update() { }                   // Implicitly private

    public void PublicMethod() { }      // Explicitly public
}
```

**Bad:**
```csharp
public class Example : MonoBehaviour
{
    [SerializeField] private float speed = 5f;  // Redundant 'private'
    private Transform cachedTransform;          // Redundant 'private'

    private void Awake() { }                    // Redundant 'private'
    private void Update() { }                   // Redundant 'private'
}
```

### Type Inference with `var`

**Use `var` for local variable declarations** when the type is obvious from the right-hand side.

**Good:**
```csharp
var position = transform.position;
var camera = GetComponent<ThirdPersonCamera>();
var direction = Vector3.back;
var rotation = Quaternion.AngleAxis(angle, Vector3.up);
```

**Bad:**
```csharp
Vector3 position = transform.position;
ThirdPersonCamera camera = GetComponent<ThirdPersonCamera>();
Vector3 direction = Vector3.back;
Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
```

### Prefer Unity Math APIs Over Manual Trigonometry

**Use Vector3 and Quaternion operations** instead of manual `Mathf.Sin`/`Mathf.Cos` calculations when possible.

**Good:**
```csharp
var direction = Vector3.back;
var horizontalRotation = Quaternion.AngleAxis(horizontalAngle, Vector3.up);
direction = horizontalRotation * direction;

var right = Vector3.Cross(Vector3.up, direction);
var verticalRotation = Quaternion.AngleAxis(verticalAngle, right);
direction = verticalRotation * direction;
```

**Bad:**
```csharp
float horizontalRad = horizontalAngle * Mathf.Deg2Rad;
float verticalRad = verticalAngle * Mathf.Deg2Rad;

float x = distance * Mathf.Sin(horizontalRad) * Mathf.Cos(verticalRad);
float y = distance * Mathf.Sin(verticalRad);
float z = distance * Mathf.Cos(horizontalRad) * Mathf.Cos(verticalRad);
```

### Core Assembly Purity

**DescrioGames.Core must remain Unity-independent** to enable testing, Descrio integration, and platform flexibility.

**Core should contain:**
- Pure C# data types
- Enums and value types
- Data contracts with no behavior methods

**Core should NOT contain:**
- MonoBehaviour classes
- Unity types (`GameObject`, `Transform`, etc.)
- Unity serialization attributes (`[SerializeField]`, `[Serializable]`)

### Enums in Separate Files

**Define each enum in its own file** for better organization and discoverability.

**Good:**
```
DescrioGames.Core/Runtime/Interactions/
├── IInteractable.cs
├── IInteractionConfig.cs
└── InteractionType.cs
```

**Bad:**
```csharp
// IInteractionConfig.cs
public interface IInteractionConfig { }
public enum InteractionType { }  // ❌ Should be in separate file
```

### Assembly README Documentation

**Assembly READMEs should focus on roles and principles**, not detailed API documentation.

**Good README content:**
- Assembly's role in the project
- What types of code belong/don't belong in the assembly
- Key design patterns and principles
- High-level dependency information

**Avoid in READMEs:**
- Detailed API references with method signatures
- Step-by-step tutorials and code examples
- Exhaustive file listings
- Configuration details that frequently change

**Rationale:** Detailed documentation becomes brittle and requires constant maintenance. High-level documentation remains stable as implementation details change.

### Language and Comments

**All code must be written in English only.**

- Variable names, function names, class names: English
- Comments: English
- Debug messages, log messages: English
- String literals for user-facing text: English (localization can be added later)

**Good:**
```csharp
// Calculate player damage based on attack power
int CalculateDamage(int attackPower)
{
    Debug.Log("Calculating damage");
    return attackPower * 2;
}
```

**Bad:**
```csharp
// プレイヤーダメージを計算
int ダメージ計算(int 攻撃力)
{
    Debug.Log("ダメージ計算中");
    return 攻撃力 * 2;
}
```

**Keep comments minimal and non-redundant.**

- Only add comments when the code's intent is not self-evident
- Avoid stating the obvious
- Focus on "why" rather than "what"
- Do not add excessive XML documentation for simple methods

**Good:**
```csharp
// Cache for performance optimization
Dictionary<string, Texture> textureCache;

// Workaround for Unity's missing feature in 2022.3
void FixColliderBug() { }
```

**Bad:**
```csharp
// This variable stores the player's health points
float health;

// This method returns true if the player is alive, otherwise false
bool IsAlive() => health > 0;
```

### Git Commit Messages

**Follow [Conventional Commits](https://www.conventionalcommits.org/) specification.**

Format: `<type>: <description>`

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code restructuring without changing behavior
- `docs`: Documentation changes
- `style`: Code style/formatting changes
- `test`: Adding or updating tests
- `chore`: Maintenance tasks, dependencies

**Rules:**
- **Single line only** - no multi-line commit messages or detailed descriptions
- Keep the description concise (under 72 characters)
- Use imperative mood ("add feature" not "added feature")
- Lowercase after the colon
- No period at the end

**Good:**
```
feat: add inventory system with item stacking
fix: correct player jump animation timing
refactor: simplify enemy AI pathfinding logic
docs: update API documentation for CharacterFactory
```

**Bad:**
```
Added a new inventory system
Fixed bug
Update stuff
feat: Added a new inventory system that allows players to collect items and stack them.
feat: add GameRootBootstrap for UI initialization

Created GameRootBootstrap to separate UI hierarchy initialization from HUD binding...
(❌ Multi-line commits are not allowed)
```

### General Guidelines

- Use XML documentation comments (`///`) for public APIs
- Keep MonoBehaviour methods (Awake, Start, Update, etc.) without access modifiers
- Prefer composition over inheritance
- Follow Unity naming conventions for serialized fields
