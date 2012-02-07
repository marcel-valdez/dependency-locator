using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyLocation
{
    public interface IDependencyLocator : IDependencyConfigurator, IDependencyProvider
    {
    }
}
