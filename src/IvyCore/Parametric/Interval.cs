using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IvyCore.Parametric
{
    /// <summary>
    /// An ordered interval [T0,T1] in R (T0 <= T1).
    /// </summary>
    public class Interval
    {
        private double t0;
        private double t1;
        private double length;

        /// <summary>
        /// Interval lower bound. Must be <= to T1.
        /// </summary>
        public double T0
        {
            get
            {
                return t0;
            }
            protected set
            {
                SetInterval(value, T1);
            }
        }

        /// <summary>
        /// Interval upper bound. Must be >= to T0.
        /// </summary>
        public double T1
        {
            get
            {
                return t1;
            }
            protected set
            {
                SetInterval(T0, value);
            }
        }
        
        /// <summary>
        /// Interval length.
        /// </summary>
        public double Length
        {
            get
            {
                return this.length;
            }
        }

        /// <summary>
        /// Construct a new interval [T0,T1] such that T0 < T1.
        /// </summary>
        /// <param name="t0">Lower bound.</param>
        /// <param name="t1">Upper bound.</param>
        public Interval(double t0, double t1)
        {
            SetInterval(t0, t1);
        }

        /// <summary>
        /// Construct a new interval default to [0,1].
        /// </summary>
        public Interval() : this(0, 1)
        {
        }

        private void SetInterval(double t0, double t1)
        {
            if (t1 > t0)
            {
                this.t0 = t0;
                this.t1 = t1;
                length = t1 - t0;
                
            }
            else
            {
                throw new System.ArgumentException("t1 must be strictly superior to t0");
            }
        }

        /// <summary>
        /// Enforce a given parameter to be constrained in the interval [T0,T1].
        /// </summary>
        /// <param name="t">The parameter to legalize.</param>
        /// <returns>The legalized parameter</returns>
        public double Legalize(double t)
        {
            if (t < this.T0)
            {
                return this.T0;
            }
            if (t > this.T1)
            {
                return this.T1;
            }
            return t;
        }

        /// <summary>
        /// Test if a given parameter is in the interval [T0,T1].
        /// </summary>
        /// <param name="t">The parameter to test.</param>
        /// <returns>True is the parameter is in [T0,T1].</returns>
        public bool IsLegal(double t)
        {
            if (t < this.T0)
            {
                return false;
            }
            if (t > this.T1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Normalized a given parameter in [0,1].
        /// </summary>
        /// <param name="t">The parameter to normalize in [T0,T1].</param>
        /// <returns>The normalized parameter in [0,1].</returns>
        public double Normalized(double t)
        {
            if (!IsLegal(t))
            {
                throw new System.ArgumentOutOfRangeException("The input parameter " + t + " is not in the interval [" + T0 + "," + T1 + "]");
            }

            return (t - t0) / length;
        }

        public override string ToString()
        {
            return String.Format("[{0:F3},{1:F3}]", T0, T1);
        }
    }
}
