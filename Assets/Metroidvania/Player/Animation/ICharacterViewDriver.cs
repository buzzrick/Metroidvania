namespace Metroidvania.Player.Animation
{
    public interface ICharacterViewDriver
    {
        void RegisterCharacterAnimationView(ICharacterAnimationView animationView);
        void RegisterCharacterBlinker(CharacterBlinker blinker);
    }

}
