# Metroidvania Todos and Nots

## Story

 - washed up on beach.
 - Expand to island village : "Cursed Village"
 - NPCs are frozen to stone - require MacGuffin to free each villager

## Todo

Tool Upgrades
Swimming fatigue and reset
better camera angle handling
colorized Gameobjects in heirarchy (eg: color or icon for UnlockNodes)


## Bugs

 - Popups persist after resource is harvested
 - Touch move sometimes keeps moving after touch release


# Future thoughts


## Cutscene Camera

Unlock node would have an optional cutscene camera option.
This would have a Camera transform, and a target transform
While the camera is transitioning to the cutscene camera and back again, the user controls will be disabled.
Possible "Focus time" setting to stay focused on the target for a period of time.
Possible separate monobehavior to encapsulate all of this configuration and running.
Possible message bus for Input control disable/enable triggers.


## Behaviour Trees
https://www.youtube.com/watch?v=aR6wt5BlE-E&ab_channel=MinaP%C3%AAcheux

https://www.youtube.com/watch?v=b6kvr10uWsg&ab_channel=IainMcManus
https://www.youtube.com/watch?v=LL0DtWwIO9A&ab_channel=IainMcManus
https://github.com/GameDevEducation/UnityAITutorial_BehaviourTrees/tree/Part-1-Behaviour-Tree-Base
https://github.com/GameDevEducation/UnityAITutorial_BehaviourTrees/tree/Part-2-Decorators-and-Services
https://github.com/GameDevEducation/UnityAITutorial_BehaviourTrees/tree/Part-3-Parallel-Node