using System;

namespace Ibra.Polymorphic.Covariant
{
    // Discriminated Type Unions inspired by http://stackoverflow.com/a/3199453/864313

    public interface Either<out A, out B>
        where A : notnull
        where B : notnull
    {
        R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB);
    }

    public interface Either<out A, out B, out C>
        where A : notnull
        where B : notnull
        where C : notnull
    {
        R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB, Func<C, R> mapperC);
    }

    public static class EitherBuilders
    {
        #region Either<A, B>
        public static Either<A, B> Either<A, B>(A item)
            where A : notnull
            where B : notnull
            => new Either_Ab<A, B>(item);
        public static Either<A, B> Either<A, B>(B item)
            where A : notnull
            where B : notnull
            => new Either_aB<A, B>(item);

        public static Either<A, B> Either<A, B>(Maybe<A> a, Maybe<B> b)
            where A : notnull
            where B : notnull
        {
            if (a.HasValue) return Either<A, B>(((Just<A>)a).Value);
            else if (b.HasValue) return Either<A, B>(((Just<B>)b).Value);
            else throw new ArgumentException("There is no value contained in any of the passed arguments.");
        }

        private sealed class Either_Ab<A, B> : Either<A, B>
            where A : notnull
            where B : notnull
        {
            private readonly A _item;
            public Either_Ab(A item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB) => mapperA(_item);
        }
        private sealed class Either_aB<A, B> : Either<A, B>
            where A : notnull
            where B : notnull
        {
            private readonly B _item;
            public Either_aB(B item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB) => mapperB(_item);
        }
        #endregion

        #region Either<A, B, C>
        public static Either<A, B, C> Either<A, B, C>(A item)
            where A : notnull
            where B : notnull
            where C : notnull => new Either_Abc<A, B, C>(item);
        public static Either<A, B, C> Either<A, B, C>(B item)
            where A : notnull
            where B : notnull
            where C : notnull => new Either_aBc<A, B, C>(item);
        public static Either<A, B, C> Either<A, B, C>(C item)
            where A : notnull
            where B : notnull
            where C : notnull => new Either_abC<A, B, C>(item);

        public static Either<A, B, C> Either<A, B, C>(Maybe<A> a, Maybe<B> b, Maybe<C> c)
            where A : notnull
            where B : notnull
            where C : notnull
        {
            if (a.HasValue) return Either<A, B, C>(((Just<A>)a).Value);
            else if (b.HasValue) return Either<A, B, C>(((Just<B>)b).Value);
            else if (c.HasValue) return Either<A, B, C>(((Just<C>)c).Value);
            else throw new ArgumentException("There is no value contained in any of the passed arguments.");
        }

        private sealed class Either_Abc<A, B, C> : Either<A, B, C>
            where A : notnull
            where B : notnull
            where C : notnull
        {
            private readonly A _item;
            public Either_Abc(A item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB, Func<C, R> mapperC) => mapperA(_item);
        }
        private sealed class Either_aBc<A, B, C> : Either<A, B, C>
            where A : notnull
            where B : notnull
            where C : notnull
        {
            private readonly B _item;
            public Either_aBc(B item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB, Func<C, R> mapperC) => mapperB(_item);
        }
        private sealed class Either_abC<A, B, C> : Either<A, B, C>
            where A : notnull
            where B : notnull
            where C : notnull
        {
            private readonly C _item;
            public Either_abC(C item) { _item = item; }
            public R Map<R>(Func<A, R> mapperA, Func<B, R> mapperB, Func<C, R> mapperC) => mapperC(_item);
        }
        #endregion
    }
}
