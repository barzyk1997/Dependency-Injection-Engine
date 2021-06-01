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

        public void ChainConstructor()
        {
            SimpleContainer container = new SimpleContainer();
            container.RegisterType<ChainBar>(false);
            ChainBar bar = container.Resolve<ChainBar>();
            Assert.IsNotNull(bar);
            Assert.IsNotNull(bar.bar);
            Assert.IsNotNull(bar.bar.bar);
        }
    }
}
