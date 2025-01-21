# Requirements Workup For The Code Cowboys

## Elicitation

1. Is the goal or outcome well defined?  Does it make sense?

    Yes, the outcome has been well defined for the skeleton of the team's end goal. As we add more to the features the goal may change to accomodate or immprove the quality of the application. 
    The outcome makes sense for what the team has agreed upon for our end vision.

2. What is not clear from the given description?

    The given description is clear for our current goals and explains top-level of the features we intend to implement. The only thing that may not be clear in the current description would be
    having to bring the enemy's health to 0 to progress, but this is implied through the turn-based roguelike description.

3. How about scope?  Is it clear what is included and what isn't?

    Yes, the description has a brief explanation in regards to each aspect of the game that we currently have thought out. It talks about the fill-in-the-blank aspect for the items that drop and enemies,
    the type of game it is planned to be, how we plan to generate the enemy's images, what options you'll be given to progress through the game, and the main purpose of the game.

4. What do you not understand?
    * Technical domain knowledge
    * Business domain knowledge                                         **DO AS GROUP**

5. Is there something missing?

    The only thing that may be missing is a little more detail into how the combat will work for the player.

6. Get answers to these questions.

## Analysis

Go through all the information gathered during the previous round of elicitation.  

1. For each attribute, term, entity, relationship, activity ... precisely determine its bounds, limitations, types and constraints in both form and function.  Write them down.

    Enemy Form: The enemy will have health (displayed in a bar), stats, a name, an image, turns, and loot drops. 
    Enemy Actions: The enemy will be able to attack the player.
    Player Form: The player will have health (displayed in a bar), stats, a name, an image, turns, xp bar, and an inventory.
    Player Actions: The player will be able to choose the next encounter for the floor on beating an enemy, to attack/defend/heal on player turn, equip/unequip gear in inventory, and add to stats on level-up.
    Stats: The player/enemy will have stats for health, damage, and defense. (As of now)
    Items: There will be pieces of equipment that can be equipped by the player to give bonus stats consisting of: Weapons/Armor/Jewelry
    Levels: The player will gain levels as they fill their xp bar that allows them to add stats to their base character.
    Actions: The player will get a choice of actions to take on defeating an enemy that will be randomized from a small pool of an elite monster, a normal monster, a random event, or a campsite.


2. Do they work together or are there some conflicting requirements, specifications or behaviors?

    Everything works together in both form and function.

3. Have you discovered if something is missing?  

    Nothing seems to be missing from our current goal.

4. Return to Elicitation activities if unanswered questions remain.


## Design and Modeling
Our first goal is to create a **data model** that will support the initial requirements.

1. Identify all entities;  for each entity, label its attributes; include concrete types

    Entities: 
        Player- Stats, Health, Image, XP Bar, Player Level, Class/Type
        Items- Stats, Names, Icon
        Enemies- Stats, Health, Image, Enemy Level, Type
        Events- Campsite, Elite Enemy, Normal Enemy, Random Event 
        Floor Levels- Floor Type, Enemy Types present for Floor

2. Identify relationships between entities.  Write them out in English descriptions.

     The player has a portrait, health bar, and items they equip that benefits the player's stats and these are looted from the defeated enemies on the floor levels the player progresses through. 
     Upon beating an enemy the player may have an event to choose.
     
     The enemy has a portrait, health bar, and stats that is encountered by the player on a floor level and drops items on defeat.

     The items have stats, an icon, and a name to be equipped by the player modifying the player's stats.

     The events are random modifiers to the player's run through the floor levels.

     The floor levels have enemies in them that the player progresses through.

3. Draw these entities and relationships in an _informal_ Entity-Relation Diagram.                          **DO AS GROUP**

4. If you have questions about something, return to elicitation and analysis before returning here.


## Analysis of the Design
The next step is to determine how well this design meets the requirements _and_ fits into the existing system.

1. Does it support all requirements/features/behaviors?
    * For each requirement, go through the steps to fulfill it.  Can it be done?  Correctly?  Easily?       **DO AS GROUP**


2. Does it meet all non-functional requirements?
    * May need to look up specifications of systems, components, etc. to evaluate this.                     **DO AS GROUP**

