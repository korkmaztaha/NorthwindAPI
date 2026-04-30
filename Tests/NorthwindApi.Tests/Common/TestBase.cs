using Moq;
using NorthwindApi.Application.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Tests.Common
{
    public abstract class TestBase
    {
        protected readonly Mock<IUnitOfWork> MockUnitOfWork;

        protected TestBase()
        {
            MockUnitOfWork = new Mock<IUnitOfWork>();
        }
    }
}
