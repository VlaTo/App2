using System;
using System.Globalization;
using System.Reflection;

namespace LibraProgramming.Windows.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Comparison
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparisonType"></param>
        /// <param name="leftOperand"></param>
        /// <param name="rightOperand"></param>
        /// <returns></returns>
        internal static bool Evaluate(ComparisonConditionType comparisonType, object leftOperand, object rightOperand)
        {
            if (null != leftOperand)
            {
                var expected = leftOperand.GetType();

                if (null != rightOperand)
                {
                    if (expected.GetTypeInfo().IsEnum && Enum.IsDefined(expected, rightOperand))
                    {
                        rightOperand = Enum.ToObject(expected, rightOperand);
                    }
                }
            }

            var left = leftOperand as IComparable;
            var right = rightOperand as IComparable;

            if (null != left && null != right)
            {
                return EvaluateComparable(comparisonType, left, right);
            }

            switch (comparisonType)
            {
                case ComparisonConditionType.Equal:
                {
                    return Object.Equals(left, right);
                }

                case ComparisonConditionType.NotEqual:
                {
                    return false == Object.Equals(left, right);
                }

                case ComparisonConditionType.LessThan:
                case ComparisonConditionType.LessThanOrEqual:
                case ComparisonConditionType.GreaterThan:
                case ComparisonConditionType.GreaterThanOrEqual:
                {
                    if (null == left && null == right)
                    {
                        throw new ArgumentException();
                    }

                    if (null == left)
                    {
                        throw new ArgumentException();
                    }

                    throw new ArgumentException();
                }
            }

            return false;
        }

        private static bool EvaluateComparable(ComparisonConditionType comparisonType, IComparable leftOperand, IComparable rightOperand)
        {
            object right;

            try
            {
                right = Convert.ChangeType(rightOperand, leftOperand.GetType(), CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return false;
            }

            if (null == right)
            {
                return ComparisonConditionType.NotEqual == comparisonType;
            }

            var op = leftOperand.CompareTo(right);

            switch (comparisonType)
            {
                case ComparisonConditionType.Equal:
                {
                    return 0 == op;
                }

                case ComparisonConditionType.NotEqual:
                {
                    return 0 != op;
                }

                case ComparisonConditionType.LessThan:
                {
                    return 0 > op;
                }

                case ComparisonConditionType.LessThanOrEqual:
                {
                    return 0 >= op;
                }

                case ComparisonConditionType.GreaterThan:
                {
                    return 0 < op;
                }

                case ComparisonConditionType.GreaterThanOrEqual:
                {
                    return 0 <= op;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}