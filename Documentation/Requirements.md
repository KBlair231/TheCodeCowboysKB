# Requirements Workup For The Code Cowboys

## Elicitation

1. Is the goal or outcome well-defined?  Does it make sense?

    Yes, the outcome has been well-defined for the skeleton of the team's end goal. As we add more to the features the goal may change to accommodate or improve the quality of the application. 
    The outcome makes sense for what the team has agreed upon for our end vision.

2. What is not clear from the given description?

    The given description is clear for our current goals and explains the top-level of the features we intend to implement. The only thing that may not be clear in the current description would be
    having to bring the enemy's health to 0 to progress, but this is implied through the turn-based roguelike description.

3. How about scope?  Is it clear what is included and what isn't?

    Yes, the description has a brief explanation in regards to each aspect of the game that we currently have thought out. It talks about the fill-in-the-blank aspect for the items that drop and enemies,
    the type of game it is planned to be, how we plan to generate the enemy's images, what options you'll be given to progress through the game, and the main purpose of the game.

4. What do you not understand?
    * Technical domain knowledge - Graphics & Modal Windows, OAUTH 2.0, Handling Expired Tokens, Hosting Site, and Google Sign-in
    * Business domain knowledge - N/A                                 

5. Is there something missing?

    The only thing that may be missing is a little more detail about how the combat will work for the player.


## Analysis

Go through all the information gathered during the previous round of elicitation.  

1. For each attribute, term, entity, relationship, and activity ... precisely determine its bounds, limitations, types, and constraints in both form and function.  Write them down.

    - Enemy Form: The enemy will have health (displayed in a bar), stats, a name, an image, turns, and loot drops. 
    - Enemy Actions: The enemy will be able to attack the player.
    - Player Form: The player will have health (displayed in a bar), stats, a name, an image, turns, xp bar, and an inventory.
    - Player Actions: The player will be able to choose the next encounter for the floor on beating an enemy, to attack/defend/heal on player turn, equip/unequip gear in inventory, and add to stats on level-up.
    - Stats: The player/enemy will have stats for health, damage, and defense. (As of now)
    - Items: There will be pieces of equipment that can be equipped by the player to give bonus stats consisting of: Weapons/Armor/Jewelry
    - Levels: The player will gain levels as they fill their xp bar that allows them to add stats to their base character.
    - Actions: The player will get a choice of actions to take on defeating an enemy that will be randomized from a small pool of an elite monster, a normal monster, a random event, or a campsite.


2. Do they work together or are there some conflicting requirements, specifications or behaviors?

    Everything works together in both form and function.

3. Have you discovered if something is missing?  

    Nothing seems to be missing from our current goal.


## Design and Modeling
Our first goal is to create a **data model** that will support the initial requirements.

1. Identify all entities;  for each entity, label its attributes; include concrete types

    * Entities:
       + Player:
           - Stats   -    int
           - Health  -    int
           - Image   -    Image
           - Level   -    int
           - Type    -    enum
       + Item-
           - Stats   -    int
           - Name    -    string
           - Icon    -    Image
       + Enemy:
           - Stats   -    int
           - Health  -    int
           - Image   -    Image
           - Level   -    int
           - Type    -    enum
       + Event:
           - TBD
       + Campsites:
           - TBD
       + Map:
           - TBD (Will contain Campsites, Enemies, etc. but implementation has not been decided on yet)

3. Identify relationships between entities.  Write them out in English descriptions.

     The player has a portrait, health bar, and items they equip that benefit the player's stats and these are looted from the defeated enemies on the floor levels the player progresses through. 
     Upon beating an enemy the player may have an event to choose.
     
     The enemy has a portrait, health bar, and stats; they are encountered by the player during a level and drop items on defeat.

     The items have stats, an icon, and a name to be equipped by the player modifying the player's stats.

     The events are random modifiers to the player's run through the levels.

4. Draw these entities and relationships in an _informal_ Entity-Relation Diagram.
    - https://miro.com/app/board/uXjVLo-psaQ=/


## Analysis of the Design
The next step is to determine how well this design meets the requirements _and_ fits into the existing system.

1. Does it support all requirements/features/behaviors?
    * For each requirement, go through the steps to fulfill it.  Can it be done?  Correctly?  Easily?       
    It supports all requirements to the best of our knowledge.

2. Does it meet all non-functional requirements?
    * May need to look up specifications of systems, components, etc. to evaluate this.                     
    It meets all non-functional requirements to the best of our knowledge.
