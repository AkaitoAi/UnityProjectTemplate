public interface ITriggerEnterable
{
    public void TriggerEnter(Interactor interactor);
}
public interface ITriggerStayable
{
    public void TriggerStay(Interactor interactor);
}
public interface ITriggerExitable
{
    public void TriggerExit(Interactor interactor);
}
public interface ICollisionEnterable
{
    public void CollisionEnter(Interactor interactor);
}
public interface ICollisionStayable
{
    public void CollisionStay(Interactor interactor);
}
public interface ICollisionExitable
{
    public void CollisionExit(Interactor interactor);
}
