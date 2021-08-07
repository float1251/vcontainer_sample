using UnityEngine;
using UnityEngine.Assertions;
using VContainer;
using VContainer.Unity;

namespace DefaultNamespace
{
    /// <summary>
    /// SampleScene1を親として登録する予定.
    /// Additiveでシーンを呼び出す.
    /// </summary>
    public class SampleScene3LifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ここ大事.
            Assert.IsNotNull(this.Parent);
            // 親にあるからNullにはならない.
            Assert.IsNotNull(Parent.Container.Resolve<HelloWorldService>());
            
            // 親に登録してるけど、falseになる...
            Debug.Log(builder.Exists(typeof(HelloWorldService)).ToString());
            
            // registerすれば当然trueになる
            // builder.Register<HelloWorldService>(Lifetime.Scoped);
            // Debug.Log(builder.Exists(typeof(HelloWorldService)).ToString());
            
            // ここでregisterしないでentry pointでresolveした場合はどうなるのか...
            builder.RegisterEntryPoint<SampleScene3Presenter>(Lifetime.Scoped);
        }
    }

    public class SampleScene3Presenter: IInitializable
    {
        [Inject]
        public SampleScene3Presenter(HelloWorldService service)
        {
            Debug.Log("Test: SampleScene3");
            service.Hello();
        }

        public void Initialize()
        {
        }
    }
}