using Marten;
using Applique.MyHotel.Core;

namespace Applique.MyHotel.Infra;

public class UnitOfWorkFactory(IDocumentStore store) : IUnitOfWorkFactory
{
    public IUnitOfWork Create() => new MartenUnitOfWork(store.LightweightSession());
}