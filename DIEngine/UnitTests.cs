using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DIEngine
{
    #region ClassesForTesting
    public interface IFoo
    {

    }

    public class Foo : IFoo
    {

    }

    public class OtherFoo : IFoo
    {

    }

    public abstract class AbstractBar
    {

    }

    public class Bar : AbstractBar
    {

    }

    public class OtherBar : AbstractBar
    {

    }

    public class ConstructorBar
    {
        public Bar bar { get; set; }
        public ConstructorBar(Bar bar)
        {
            this.bar = bar;
        }
    }

    public class StringBar
    {
        public string str { get; set; }
        public StringBar(string a)
        {
            str = a;
        }
    }


    public class MultiStringBar
    {
        public string str { get; set; }
        public StringBar stringBar { get; set; }
        public MultiStringBar(string a)
        {
            str = a;
            stringBar = new StringBar(a);
        }
        public MultiStringBar(string a, StringBar b)
        {
            str = a;
            stringBar = b;
        }
    }

    public class MultiBarBar
    {
        public Bar bar { get; set; }
        public Foo foo { get; set; }

        public MultiBarBar() { }
        public MultiBarBar(Bar bar)
        {
            this.bar = bar;
        }

        public MultiBarBar(Bar bar, Foo foo)
        {
            this.bar = bar;
            this.foo = foo;
        }

    }
    public class MultiStringBar1
    {
        public string string1 { get; set; }
        public StringBar stringBar { get; set; }

        [DependencyConstructor]
        public MultiStringBar1(string a)
        {
            string1 = a;
            stringBar = new StringBar(a);
        }
        public MultiStringBar1(string a, StringBar b)
        {
            string1 = a;
            stringBar = b;
        }
    }

    public class ChainBar
    {
        public ConstructorBar bar {get; set;}

        public ChainBar(ConstructorBar bar)
        {
            this.bar = bar;
        }
    }

    public class A
    {
        public A a;
        public A(A a)
        {
            this.a = a;
        }
    }

    public class BuildUpObject
    {
        public Bar bar1;
        public Bar bar2;
        public Bar bar3;
        public ConstructorBar bar4;

        [DependencyProperty]
        public Foo foo { get; set; }

        public OtherFoo foo2 { get; set; }

        public BuildUpObject(Bar bar3, ConstructorBar bar4)
        {
            this.bar3 = bar3;
            this.bar4 = bar4;
        }


        [DependencyMethod]
        public void setBar1(Bar bar1)
        {
            this.bar1 = bar1;
        }
        
        public void setBar2(Bar bar2)
        {
            this.bar2 = bar2;
        }
    }

    public class NestedBuildUpObject
    {
        [DependencyProperty]
        public BuildUpObject obj1 { get; set; }

        public BuildUpObject obj2;

        public BuildUpObject obj3;

        public NestedBuildUpObject(BuildUpObject obj2)
        {
            this.obj2 = obj2;
        }

        [DependencyMethod]
        public void SetObj3(BuildUpObject obj3)
        {
            this.obj3 = obj3;
        }
    }

    public class LoopedBuildUpObject1
    {
        [DependencyProperty]
        public LoopedBuildUpObject1 obj { get; set; }
    }

    public class LoopedBuildUpObject2
    {
        [DependencyMethod]
        public void SetObj(LoopedBuildUpObject2 obj) { }
    }

    public class CyclicBuildUpObject1
    {
        [DependencyProperty]
        public CyclicBuildUpObject1 obj { get; set; }
    }

    public class CyclicBuildUpObject2
    {
        [DependencyMethod]
        public void SetObj(CyclicBuildUpObject1 obj) { }
    }
    #endregion
    [TestClass]
    public class UnitTests
    {
        #region ContainerTests
        [TestMethod]
        public void SimpleConcrete()
        {
            SimpleContainer container = new SimpleContainer();

            IFoo foo = container.Resolve<Foo>();
            Assert.IsNotNull(foo);
            Assert.IsInstanceOfType(foo, typeof(Foo));

            IFoo otherFoo = container.Resolve<OtherFoo>();
            Assert.IsNotNull(otherFoo);
            Assert.IsInstanceOfType(otherFoo, typeof(OtherFoo));

            Bar bar = container.Resolve<Bar>();
            Assert.IsNotNull(bar);
            Assert.IsNotInstanceOfType(bar, typeof(OtherBar));
            Assert.IsInstanceOfType(bar, typeof(Bar));

            // niezarejestrowane typy s¹ zwracane nowe za ka¿dym razem
            Bar bar2 = container.Resolve<Bar>();
            Assert.AreNotEqual(bar, bar2);

            OtherBar otherBar = container.Resolve<OtherBar>();
            Assert.IsNotNull(otherBar);
            Assert.IsInstanceOfType(otherBar, typeof(OtherBar));
            Assert.IsNotInstanceOfType(otherBar, typeof(Bar));
        }

        public void UnregisteredAbstract()
        {
            SimpleContainer container = new SimpleContainer();

            Assert.ThrowsException<MissingTypeException>(() =>
            {
                AbstractBar bar = container.Resolve<AbstractBar>();
            });
        }

        [TestMethod]
        public void TransientAbstract()
        {
            SimpleContainer container = new SimpleContainer();

            container.RegisterType<AbstractBar, Bar>(false);

            AbstractBar bar1 = container.Resolve<AbstractBar>();

            Assert.IsNotNull(bar1);
            Assert.IsInstanceOfType(bar1, typeof(Bar));
            
            AbstractBar bar2 = container.Resolve<AbstractBar>();

            Assert.IsNotNull(bar2);
            Assert.IsInstanceOfType(bar2, typeof(Bar));
            Assert.AreNotEqual(bar1, bar2);

            container.RegisterType<AbstractBar, OtherBar>(false);

            AbstractBar bar3 = container.Resolve<AbstractBar>();

            Assert.IsNotNull(bar3);
            Assert.IsInstanceOfType(bar3, typeof(OtherBar));
        }

        [TestMethod]
        public void SingletonAbstract()
        {
            SimpleContainer container = new SimpleContainer();

            container.RegisterType<AbstractBar, Bar>(true);

            AbstractBar bar1 = container.Resolve<AbstractBar>();

            Assert.IsNotNull(bar1);
            Assert.IsInstanceOfType(bar1, typeof(Bar));

            AbstractBar bar2 = container.Resolve<AbstractBar>();

            Assert.IsNotNull(bar2);
            Assert.IsInstanceOfType(bar2, typeof(Bar));

            Assert.AreEqual(bar1, bar2);
        }

        [TestMethod]
        public void UnregisteredInterface()
        {
            SimpleContainer container = new SimpleContainer();

            Assert.ThrowsException<MissingTypeException>(() =>
            {
                IFoo foo = container.Resolve<IFoo>();
            });
        }


        [TestMethod]
        public void TransientInterface()
        {
            SimpleContainer container = new SimpleContainer();

            container.RegisterType<IFoo, Foo>(false);

            IFoo foo1 = container.Resolve<IFoo>();

            Assert.IsNotNull(foo1);
            Assert.IsInstanceOfType(foo1, typeof(Foo));

            IFoo foo2 = container.Resolve<IFoo>();

            Assert.IsNotNull(foo2);
            Assert.IsInstanceOfType(foo2, typeof(Foo));
            Assert.AreNotEqual(foo1, foo2);

            container.RegisterType<IFoo, OtherFoo>(false);

            IFoo foo3 = container.Resolve<IFoo>();

            Assert.IsNotNull(foo3);
            Assert.IsInstanceOfType(foo3, typeof(OtherFoo));
        }


        [TestMethod]
        public void SingletonInterface()
        {
            SimpleContainer container = new SimpleContainer();

            container.RegisterType<IFoo, Foo>(true);

            IFoo foo1 = container.Resolve<IFoo>();

            Assert.IsNotNull(foo1);
            Assert.IsInstanceOfType(foo1, typeof(Foo));

            IFoo foo2 = container.Resolve<IFoo>();

            Assert.IsNotNull(foo2);
            Assert.IsInstanceOfType(foo2, typeof(Foo));

            Assert.AreEqual(foo1, foo2);
        }
        #endregion
        #region ConstructorTests
        [TestMethod]
        public void NonEmptyConstructor()
        {
            SimpleContainer container = new SimpleContainer();
            container.RegisterType<ConstructorBar>(false);
            ConstructorBar bar = container.Resolve<ConstructorBar>();
            Assert.IsNotNull(bar);
            Assert.IsNotNull(bar.bar);
        }

        [TestMethod]
        public void UnregisteredStringConstructor()
        {
            SimpleContainer container = new SimpleContainer();
            container.RegisterType<StringBar>(false);
            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                StringBar foo = container.Resolve<StringBar>();
            });
        }

        [TestMethod]
        public void RegisteredStringConstructor()
        {
            SimpleContainer container = new SimpleContainer();
            string slowo = "test";
            container.RegisterInstance(slowo);
            container.RegisterType<StringBar>(false);
            StringBar foo = container.Resolve<StringBar>();
            Assert.IsNotNull(foo);
            Assert.AreEqual(slowo, foo.str);
        }

        [TestMethod]
        public void MultiArgsConstructor()
        {
            SimpleContainer container = new SimpleContainer();
            container.RegisterType<MultiBarBar>(false);
            MultiBarBar foo = container.Resolve<MultiBarBar>();
            Assert.IsNotNull(foo);
            Assert.IsNotNull(foo.foo);
            Assert.IsNotNull(foo.bar);
        }

        [TestMethod]
        public void OnlyOneString()
        {
            SimpleContainer container = new SimpleContainer();
            string slowo = "test";
            container.RegisterInstance(slowo);
            container.RegisterType<MultiStringBar>(false);
            MultiStringBar bar = container.Resolve<MultiStringBar>();
            Assert.IsNotNull(bar);
            Assert.IsNotNull(bar.str);
            Assert.AreEqual(bar.str, slowo);
            Assert.IsNotNull(bar.stringBar);
            Assert.AreEqual(bar.stringBar.str, slowo);
        }
        [TestMethod]
        public void BothStrings()
        {
            SimpleContainer container = new SimpleContainer();
            string slowo = "test";
            string slowo2 = "test1";
            StringBar strBar = new StringBar(slowo2);
            container.RegisterInstance(slowo);
            container.RegisterInstance(strBar);
            container.RegisterType<MultiStringBar>(false);
            MultiStringBar bar = container.Resolve<MultiStringBar>();
            Assert.IsNotNull(bar);
            Assert.IsNotNull(bar.str);
            Assert.AreEqual(bar.str, slowo);
            Assert.IsNotNull(bar.stringBar);
            Assert.AreEqual(bar.stringBar.str, slowo2);
        }

        [TestMethod]
        public void DependencyConstructor()
        {
            SimpleContainer container = new SimpleContainer();
            string slowo = "test";
            string slowo2 = "test1";
            StringBar strBar = new StringBar(slowo2);
            container.RegisterInstance(slowo);
            container.RegisterInstance(strBar);
            container.RegisterType<MultiStringBar1>(false);
            MultiStringBar1 bar = container.Resolve<MultiStringBar1>();
            Assert.IsNotNull(bar);
            Assert.IsNotNull(bar.string1);
            Assert.AreEqual(bar.string1, slowo);
            Assert.IsNotNull(bar.stringBar);
            Assert.AreEqual(bar.stringBar.str, slowo);
        }

        [TestMethod]
        public void LoopedConstructor()
        {
            SimpleContainer container = new SimpleContainer();
            container.RegisterType<A>(false);
            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                A foo = container.Resolve<A>();
            });
        }

        [TestMethod]
        public void ChainConstructor()
        {
            SimpleContainer container = new SimpleContainer();
            container.RegisterType<ChainBar>(false);
            ChainBar bar = container.Resolve<ChainBar>();
            Assert.IsNotNull(bar);
            Assert.IsNotNull(bar.bar);
            Assert.IsNotNull(bar.bar.bar);
        }
        #endregion
        #region BuildTests
        [TestMethod]
        public void SimpleBuildUp()
        {
            Bar bar3 = new Bar();
            ConstructorBar bar4 = new ConstructorBar(bar3);
            BuildUpObject obj = new BuildUpObject(bar3, bar4);
            SimpleContainer container = new SimpleContainer();

            container.BuildUp(obj);

            Assert.IsNull(obj.foo2);
            Assert.IsNull(obj.bar2);

            Assert.IsNotNull(obj.foo);
            Assert.IsNotNull(obj.bar1);
        }

        [TestMethod]
        public void ConstructAndBuild()
        {
            SimpleContainer container = new SimpleContainer();
            BuildUpObject obj = container.Resolve<BuildUpObject>();
            
            Assert.IsNull(obj.foo2);
            Assert.IsNull(obj.bar2);

            Assert.IsNotNull(obj.foo);
            Assert.IsNotNull(obj.bar1);
        }

        [TestMethod]
        public void NestedBuildInConstruction()
        {
            SimpleContainer container = new SimpleContainer();
            NestedBuildUpObject obj = container.Resolve<NestedBuildUpObject>();

            Assert.IsNotNull(obj.obj1);
            Assert.IsNotNull(obj.obj2);
            Assert.IsNotNull(obj.obj3);
        }

        [TestMethod]
        public void LoopedBuild()
        {
            SimpleContainer container = new SimpleContainer();
            LoopedBuildUpObject1 looped1 = new LoopedBuildUpObject1();

            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                container.BuildUp(looped1);
            });

            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                container.Resolve<LoopedBuildUpObject1>();
            });

            LoopedBuildUpObject2 looped2 = new LoopedBuildUpObject2();

            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                container.BuildUp(looped2);
            });

            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                container.Resolve<LoopedBuildUpObject2>();
            });
        }

        [TestMethod]
        public void CyclicBuild()
        {
            SimpleContainer container = new SimpleContainer();
            CyclicBuildUpObject1 cyclic1 = new CyclicBuildUpObject1();

            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                container.BuildUp(cyclic1);
            });

            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                container.Resolve<CyclicBuildUpObject1>();
            });

            CyclicBuildUpObject2 cyclic2 = new CyclicBuildUpObject2();

            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                container.BuildUp(cyclic2);
            });

            Assert.ThrowsException<DependencyResolvingException>(() =>
            {
                container.Resolve<CyclicBuildUpObject2>();
            });
        }
        #endregion
        #region ServiceLocatorTests
        [TestMethod]
        public void AlwaysTheSameServiceLocatorTest1()
        {
            SimpleContainer c = new SimpleContainer();
            ContainerProviderDelegate containerProvider = () => c;
            ServiceLocator.SetContainerProvider(containerProvider);

            c.RegisterType<Foo>(true);

            Assert.IsNotNull(ServiceLocator.Current.GetInstance<Foo>());
            Assert.AreEqual(ServiceLocator.Current.GetInstance<Foo>(), c.Resolve<Foo>());
        }

        [TestMethod]
        public void AlwaysTheSameServiceLocatorTest2()
        {
            SimpleContainer c = new SimpleContainer();
            ContainerProviderDelegate containerProvider = () => c;
            ServiceLocator.SetContainerProvider(containerProvider);

            Assert.AreEqual(ServiceLocator.Current.GetInstance<SimpleContainer>(), c);
        }

        [TestMethod]
        public void AlwaysNewServiceLocatorTest()
        {
            ContainerProviderDelegate containerProvider = () => new SimpleContainer();
            ServiceLocator.SetContainerProvider(containerProvider);

            Assert.AreNotEqual(ServiceLocator.Current.GetInstance<SimpleContainer>(), ServiceLocator.Current.GetInstance<SimpleContainer>());
        }
        #endregion
        #region LocalFactoryTests
        [TestMethod]
        public void LocalFactoryUsage()
        {
            var container = new SimpleContainer();
            container.RegisterType<IFoo, Foo>(true);
            LocalFactory<IFoo> factory = new LocalFactory<IFoo>();
            factory.SetProvider(() => container.Resolve<IFoo>());

            Assert.AreEqual(factory.CreateService(), factory.CreateService());
            Assert.AreEqual(container.Resolve<IFoo>(), factory.CreateService());
        }
        #endregion
    }
}
