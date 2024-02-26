namespace Metroidvania.Characters.NPC.AI
{
    /// <summary>
    /// An NPC behaviour. Each NPC can have multiple GoalAIs running.
    /// <br/>    
    /// </summary>
    public interface IGoalAI
    {
        /// <summary>
        /// Checks whether the AI is currently enabled.
        /// </summary>
        /// <returns></returns>
        bool IsEnabled();
        
        /// <summary>
        /// Sets whether the AI is currently enabled.
        /// Disabled AIs don't run either their ObservePhase or ActionPhases
        /// </summary>
        void SetEnabled(bool isEnabled);

        /// <summary>
        /// Whether this behavior can run
        /// </summary>
        /// <returns></returns>
        bool CanRun();

        /// <summary>
        /// The calculate the GoalAI priority.
        /// This can change over time (eg: a ChaseTarget AI will have a low priority when there is no visible target) 
        /// </summary>
        int CalculatePriority();
        
        /// <summary>
        /// </summary>
        void OnTickGoal();
        
        /// <summary>
        /// Inject the NPCCharacterAI object to allow this GoalAI to send move commands
        /// </summary>
        /// <param name="characterAI"></param>
        void InjectNPCCharacterAI(NPCCharacterAI characterAI);
    }
}