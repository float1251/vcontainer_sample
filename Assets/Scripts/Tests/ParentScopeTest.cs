using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;

public sealed class ParentService
{
}

public sealed class ChildService
{
}

public class ParentScopeTest
{
    [Test]
    public void CheckSameTypeRegisterInChildScope()
    {
        var builder = new ContainerBuilder();
        builder.Register<ParentService>(Lifetime.Singleton);

        var parentContainer = builder.Build();
        var childContainer = parentContainer.CreateScope(childBuilder =>
        {
            // Errorは発生しない...
            childBuilder.Register<ParentService>(Lifetime.Scoped);
            childBuilder.Register<ChildService>(Lifetime.Singleton);
        });

        // singletonだから同じ.
        Assert.That(parentContainer.Resolve<ParentService>(), Is.EqualTo(parentContainer.Resolve<ParentService>()));

        // これは違うものになる
        Assert.That(parentContainer.Resolve<ParentService>(), Is.Not.EqualTo(childContainer.Resolve<ParentService>()));

        var childContainer2 = parentContainer.CreateScope(childBuilder =>
        {
            childBuilder.Register<ChildService>(Lifetime.Singleton);
        });
        // 親から来るので同じものになるはず.
        Assert.That(parentContainer.Resolve<ParentService>(), Is.EqualTo(childContainer2.Resolve<ParentService>()));
    }
}