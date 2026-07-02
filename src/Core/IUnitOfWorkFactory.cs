namespace Applique.MyHotel.Core;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}