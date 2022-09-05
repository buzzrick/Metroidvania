namespace Metroidvania.Player.Animation
{
    public interface ICharacterMovementDriver
    {
        void RegisterCharacterAnimationView(ICharacterAnimationView animationView);
        void RegisterCharacterBlinker(CharacterBlinker blinker);
    }

}
