using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Jib.Controls
{
    public static class PredicateExtensions
    {
        public static Predicate<T> And<T>(this Predicate<T> original, Predicate<T> newPredicate)
        {
            return t => original(t) && newPredicate(t);
        }

        public static Predicate<T> Or<T>(this Predicate<T> original, Predicate<T> newPredicate)
        {
            return t => original(t) || newPredicate(t);
        }
    }
}
