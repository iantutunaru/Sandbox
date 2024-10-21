![Logo](./Assets/Icon/Icon.png?raw=true)
# Sandbox
## "Flow like sand and box... like a boxer!"

Welcome to **Sandbox**, a Unity project focused on player movement, interaction, and combat mechanics. 
This project started with the single goal of researching how to implement an active ragdoll movement system similar to those found in projects like [Gang Beasts](https://store.steampowered.com/app/285900/Gang_Beasts/) or [Party Animals](https://store.steampowered.com/app/1260320/Party_Animals/).
However, it became more, with the potential to be a Unity project focused on player movement, interaction, and combat mechanics. This project serves as a robust foundation for creating complex gameplay involving player stats, camera control, animations, and physical interactions between game objects.

## Features
### 1. **Player Stats**
   - Tracks player health, death, and respawn states.
   - Supports damage application based on specific body parts (head, limbs, torso).
   - Automatic respawn system after a configurable waiting time.
### 2. **Player Movement & Locomotion**
   - Handles player movement based on camera orientation.
   - Supports walking, running, sprinting, jumping, and falling mechanics.
   - Includes smooth rotation and animation synchronization for active ragdoll effects.
### 3. **Combat System**
   - Supports light and heavy attacks, both for players and NPCs.
   - Collision detection system for dealing damage to specific body parts of enemies.
   - Integrates with player animations to trigger attack and death animations.
### 4. **Camera Management**
   - Smooth third-person camera controls that follow the player with adjustable rotation.
   - Collision detection ensures that the camera doesn't clip through the environment.
### 5. **Animation System**
   - Synchronizes character movement and animation, ensuring smooth transitions between idle, movement, and combat states.
   - Plays specific animations during interaction (such as attacking or jumping) and restricts player input during critical animations.
### 6. **Health System**
   - Dynamic health bar with UI integration, tracking player health and visual feedback for damage taken.
### 7. **Physics Interactions**
   - Configurable joints and physical syncing for active ragdoll mechanics.
   - Physics-based interactions using Unity's `ConfigurableJoint` system to manage player and object movement.

## Setup Instructions
**Environment Setup:** Ensure your Unity project is set up with the latest version of Unity. Import all necessary Unity packages and ensure your project settings are configured for a 3D game environment.

## Contributing
Your contributions are welcome! Please feel free to submit pull requests or open issues to discuss potential improvements or fixes.

## References
  - [Starter Assets: Character Controllers | URP](https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961?srsltid=AfmBOorIRbQxpIgeugMWwn579AOXt710GpmXXMskVYN0r4gZCVaKG75M) - Animator base, idle animation and avatar configuration.
  - [POLYGON - Office Pack](https://syntystore.com/products/polygon-office-pack?srsltid=AfmBOop6Qzh26eVqE9uMoD0jaZ-oTSOHeVxC49F_zJaSzAXP3zjr2Hf0) - player character and environment.
  - [Mixamo](https://www.mixamo.com/#/) - animations.
