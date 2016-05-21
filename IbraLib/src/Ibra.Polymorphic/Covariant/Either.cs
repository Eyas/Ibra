using System;

namespace Ibra.Polymorphic.Covariant
{
    // Discriminated Type Unions inspired by http://stackoverflow.com/a/3199453/864313

    public interface Either<out A, out B>
    {
        R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB);
    }

    public interface Either<out A, out B, out C>
    {
        R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB, Func<C, R> mapperC);
    }

    public static class EitherBuilders
    {
        #region Either<A, B>
        public static Either<A, B> Either<A, B>(A item) => new Either_Ab<A, B>(item);
        public static Either<A, B> Either<A, B>(B item) => new Either_aB<A, B>(item);

        public static Either<A, B> Either<A, B>(Maybe<A> a, Maybe<B> b)
        {
            if (a.HasValue) return Either<A, B>(((Just<A>)a).Value);
            else if (b.HasValue) return Either<A, B>(((Just<B>)b).Value);
            else throw new ArgumentException("There is no value contained in any of the passed arguments.");
        }

        private sealed class Either_Ab<A, B> : Either<A, B>
        {
            private readonly A _item;
            public Either_Ab(A item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB) => mapperA(_item);
        }
        private sealed class Either_aB<A, B> : Either<A, B>
        {
            private readonly B _item;
            public Either_aB(B item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB) => mapperB(_item);
        }
        #endregion

        #region Either<A, B, C>
        public static Either<A, B, C> Either<A, B, C>(A item) => new Either_Abc<A, B, C>(item);
        public static Either<A, B, C> Either<A, B, C>(B item) => new Either_aBc<A, B, C>(item);
        public static Either<A, B, C> Either<A, B, C>(C item) => new Either_abC<A, B, C>(item);

        public static Either<A, B, C> Either<A, B, C>(Maybe<A> a, Maybe<B> b, Maybe<C> c)
        {
            if (a.HasValue) return Either<A, B, C>(((Just<A>)a).Value);
            else if (b.HasValue) return Either<A, B, C>(((Just<B>)b).Value);
            else if (c.HasValue) return Either<A, B, C>(((Just<C>)c).Value);
            else throw new ArgumentException("There is no value contained in any of the passed arguments.");
        }

        private sealed class Either_Abc<A, B, C> : Either<A, B, C>
        {
            private readonly A _item;
            public Either_Abc(A item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB, Func<C, R> mapperC) => mapperA(_item);
        }
        private sealed class Either_aBc<A, B, C> : Either<A, B, C>
        {
            private readonly B _item;
            public Either_aBc(B item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB, Func<C, R> mapperC) => mapperB(_item);
        }
        private sealed class Either_abC<A, B, C> : Either<A, B, C>
        {
            private readonly C _item;
            public Either_abC(C item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB, Func<C, R> mapperC) => mapperC(_item);
        }
        #endregion
    }
}
