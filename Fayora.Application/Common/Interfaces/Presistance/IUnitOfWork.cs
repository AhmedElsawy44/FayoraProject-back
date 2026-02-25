using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IUnitOfWork
{
    Task CommitChangesAsync();
}
