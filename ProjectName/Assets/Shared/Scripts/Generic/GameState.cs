using Assets.Network;

namespace Assets.Generic
{
    /// <summary>
    /// Enumerator that wraps android and pc gamestates. Add mores values has more GameStates are added to the game.
    /// </summary>
    public enum GameState : byte
    {
        MenuState,
        InGameState
    }
    /// <summary>
    /// Interface that binds all GameStates together.
    /// </summary>
    public interface  IGameState
    {
         void OnDataReceived(MessageBase message, int connectionID);
    }
}
