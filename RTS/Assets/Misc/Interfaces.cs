public interface IDamagable
{
    void Damage(float damage);
}
public interface IKillable
{
    void Kill();
}
public interface IBattleUnitCommand
{
    void Execute();
}
public interface ISelectable
{
    bool CanSelect(Team team);
    void Select();
    void Deselect();
}
public interface IHighlightable
{
    void HighlightOn();
    void HighlightOff();
}