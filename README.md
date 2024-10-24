<p align="center">
     <img src="./Assets/Icon/Icon_small.png">
</p>

#      Sandbox
## "Flow like the sand and box... like a boxer!"

Welcome to **Sandbox**!
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

## Screenshots
### v0.2.1.x.
### Screenshot 1 - Player character and environment
<p align="center">
     <img src="./Assets/Screenshots/v0.2_/Screen1.PNG">
     Displayed are the player character, office environment, and health bar UI.
</p>

### Screenshot 2 - Player movement - running
<p align="center">
     <img src="./Assets/Screenshots/v0.2_/Screen2.PNG">
     The player character runs in the office environment.
</p>

### Screenshot 3 - Player movement - jumping
<p align="center">
     <img src="./Assets/Screenshots/v0.2_/Screen3.PNG">
     The player character jumps in the office environment.
</p>

### Screenshot 4 - Local split-screen multiplayer
<p align="center">
     <img src="./Assets/Screenshots/v0.2_/Screen4.PNG">
     Two players looking at each other.
</p>

### Screenshot 5 - Local split-screen multiplayer - fighting
<p align="center">
     <img src="./Assets/Screenshots/v0.2_/Screen5.PNG">
     Two players attacking each other. Player 1 is using a normal attack - a series of punches. Player 2 is using a heavy attack - standing karate crane kick.
</p>

### Screenshot 6 - Local split-screen multiplayer - death
<p align="center">
     <img src="./Assets/Screenshots/v0.2_/Screen6.PNG">
     Player 1 lost the battle and is currently dead. They will respawn in 10 seconds and stand back up.
</p>

## Setup Instructions
**Environment Setup:** Ensure your Unity project is set up with the latest version of Unity. Import all necessary Unity packages and ensure your project settings are configured for a 3D game environment.

## Contributing
Your contributions are welcome! Please feel free to submit pull requests or open issues to discuss potential improvements or fixes.

## References
  - [Starter Assets: Character Controllers | URP](https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961?srsltid=AfmBOorIRbQxpIgeugMWwn579AOXt710GpmXXMskVYN0r4gZCVaKG75M) - Animator base, idle animation and avatar configuration.
  - [POLYGON - Office Pack](https://syntystore.com/products/polygon-office-pack?srsltid=AfmBOop6Qzh26eVqE9uMoD0jaZ-oTSOHeVxC49F_zJaSzAXP3zjr2Hf0) - Player character and environment.
  - [Mixamo](https://www.mixamo.com/#/) - Animations.
  - [ChatGPT](https://openai.com/index/chatgpt/) - Logo creation.

## NOTE: Currently there are more comments than required
     - If the project is picked up by someone unfamiliar with Unity or some of its aspects, the comments should help them get working faster. 
     - In the future, the number of comments will be reduced when the project is near the final version.
