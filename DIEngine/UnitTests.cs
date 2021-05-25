using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DIEngine
{

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


    [TestClass]
    public class UnitTests
    {
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
    }
}
