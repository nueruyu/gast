# AGENTS.md

## 1. Introduction

This document outlines the architectural structure of the `gast` project. The project is built upon a layered architecture, inspired by Clean Architecture principles. This design promotes separation of concerns, testability, and maintainability by isolating business logic from external frameworks and implementations.

The primary goal of this architecture is to create a clear flow of dependencies, ensuring that core game logic is independent of how it is presented, stored, or interacted with.

## 2. Core Principles

The architecture adheres to the following core principles:

- **The Dependency Rule:** Source code dependencies can only point inwards. Inner layers must not have any knowledge of outer layers. For example, the `Domain` layer cannot depend on the `Infrastructure` or `UI` layers.
- **Abstraction:** Inner layers define interfaces (abstractions), and outer layers provide the concrete implementations. Dependency Injection is used at the composition root to connect these parts.
- **Separation of Concerns:** Each assembly has a single, well-defined responsibility.
  - **Business Logic (`Domain`, `Application`)**: Defines _what_ the game can do and the rules that govern it.
  - **Implementation Details (`Infrastructure`, `Features`, `UI`)**: Defines _how_ the business logic is executed, presented, and persisted.

## 3. Assembly Overview

The project is divided into several assemblies, each representing a layer or a specific concern.

### `Gast.Core`

- **Purpose:** The innermost layer of the architecture. It contains the most fundamental and stable code, with no dependencies on other project assemblies.
- **Contents:** Project-agnostic interfaces and types, such as `ICommand`, `IDomainEvent`, observable patterns (`ILive`, `ISignal`), and basic dependency injection abstractions (`IInstaller`). It contains no game logic or Unity-specific code.
- **Dependencies:** None.

### `Gast.Domain`

- **Purpose:** Defines the core business logic, rules, and data structures of the game. This layer represents the "enterprise business rules."
- **Contents:**
  - **Entities:** Core objects with a unique identity (e.g., Character, Item).
  - **Value Objects:** Immutable data structures (e.g., `CharacterId`, `ItemId`).
  - **Repository Interfaces:** Abstractions for data persistence (e.g., `ICharacterRepository`).
  - **Domain Events:** Describe significant occurrences in the domain.
- **Dependencies:** Depends only on `Gast.Core`. It must remain pure and independent of any other layer.

### `Gast.Application`

- **Purpose:** The use-case layer. It orchestrates the flow of data between the `Domain` layer and the outer layers. It contains application-specific business rules.
- **Contents:**
  - **Use Cases:** Classes that implement specific application operations (e.g., creating a character, purchasing an item).
  - **Command/Query Handlers:** Logic that responds to requests from the outer layers and uses domain entities to fulfill them.
- **Dependencies:** Depends on `Gast.Domain` and `Gast.Core`.

### `Gast.Infrastructure`

- **Purpose:** The outermost layer for external concerns. It provides concrete implementations for the interfaces defined in the inner layers (`Domain`, `Application`).
- **Contents:**
  - **Repository Implementations:** Concrete data access logic (e.g., in-memory repositories).
  - **External Service Clients:** Code for communicating with external APIs (e.g., AI planning services).
  - **Framework-Specific Implementations:** Services that are tightly coupled to the underlying framework (Unity).
- **Dependencies:** Depends on `Application`, `Domain`, and `Core`.

### `Gast.Features`

- **Purpose:** This layer contains concrete gameplay features implemented as Unity `MonoBehaviour`s and related classes. It acts as the bridge between the abstract application logic and the Unity engine's scene-based components.
- **Contents:**
  - `MonoBehaviour` components for characters, interactable objects, spawn points, etc.
  - Systems that manage game mechanics within a Unity scene.
  - Code that translates Unity events (e.g., collisions, input) into application commands or queries.
- **Dependencies:** Can depend on `Application`, `Domain`, and `Core`. It should use interfaces to interact with `Infrastructure` services whenever possible.

### `Gast.UI`

- **Purpose:** Handles all user interface elements, including HUDs, menus, and prompts. It is responsible for presenting data to the user and capturing user input.
- **Contents:**
  - **Views:** UI Toolkit visual elements.
  - **ViewModels:** Classes that prepare and manage data for the views, mediating between the UI and the `Application`/`Domain` layers.
- **Dependencies:** Depends on `Application`, `Domain`, and `Core`.

### `Gast.Shared`

- **Purpose:** A utility assembly for code that is shared across multiple layers but is not fundamental enough to be in `Gast.Core`. It often contains framework-specific helpers.
- **Contents:** Extension methods (e.g., for Unity's UI Toolkit or Input System), helper classes, and other reusable utilities.
- **Dependencies:** Has minimal dependencies, typically `Gast.Core` and third-party libraries. It can be referenced by any other layer.

### `Gast.Composition`

- **Purpose:** The composition root. This is where the application's object graph is constructed using a Dependency Injection (DI) container.
- **Contents:** The `LifetimeScope` which initializes the DI container, registering all interfaces with their concrete implementations.
- **Dependencies:** This is the only place in the project that has knowledge of all other assemblies. It brings all the layers together.

### `GastGame`

- **Purpose:** This assembly contains the specific logic and implementations for the actual game being built, as opposed to the generic framework provided by the other `Gast.*` assemblies.
- **Contents:** Game-specific character actions, AI behaviors (brains), stat definitions, and other logic that defines the unique rules and feel of this game.
- **Dependencies:** Depends on the public APIs of the `Gast` framework assemblies (`Domain`, `Application`, `Features`, etc.).

### `Gast.Lib.*`

- **Purpose:** A collection of self-contained, reusable libraries that are not specific to this project's domain.
- **Contents:** A generic AI planning library (`Lib.AI`), a client for an external service (`Lib.Gaia`), etc.
- **Dependencies:** These libraries must be self-contained and must **not** depend on any other `Gast.*` assemblies. They can be depended upon by other layers.
