
public interface IState
{
    void Tick();
    void LateTick();
    void OnEnter();
    void OnExit();
}
