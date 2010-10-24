using System;
using OptimizationToolbox;

namespace tester.G01
{
    /*%function:
 %(1)input:the input variable ,x is one point.it can be a column vector or a row
 %vector.
 %*****note that It can not evaluate the function at multiple points at
 %once,but you can call the fuction multiple times.
 %(2)output:f is a scalar,not a vector.
 %
 %reference:
 %note that you can get the formulation of g01 from some
 %aritcles,such as
 %(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
 % IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
 %
 %algorithm:
 %below code is  edited according to 
 %(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
 % IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
 %
 %solution:
 %n=13;lb=zeros(n,1);ub=[ones(9,1) ;100*ones(3,1); 1];
 %The global minimum is [1; 1; 1; 1; 1; 1; 1; 1; 1; 3; 3; 3; 1].
 %where six constraints are active ( c1,c2,c3,c7,c8 and c9 ) and f=-15;

 %Copyright:
 % programmers:oiltowater.
 % It comply with the GPL2.0
 % Copyright 2006  oiltowater 




 %for get the number of evaluation of function
 global functionAcount;
 functionAcount=functionAcount+1;


 %change the row vector to a column vector.
 if size(x,1)==1,
     x=x';
 end

 if size(x,1)~=13 
     error('the length of x must be 13!');
 end


 f=5*sum(x(1:4,1))-5*sum(x(1:4,1).*x(1:4,1))-sum(x(5:13,1)); */

    internal class go1ObjFn : IObjectiveFunction, IDifferentiable
    {
        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            var f = 5 * (x[0] + x[1] + x[2] + x[3]);
            f -= 5 * (x[0] * x[0] + x[1] * x[1] + x[2] * x[2] + x[3] * x[3]);
            for (var i = 4; i < 13; i++)
                f -= x[i];
            return f;
        }

        #endregion

        #region Implementation of IDifferentiable

        public double deriv_wrt_xi(double[] x, int i)
        {
            if (i >= 4) return -1;
            return 5 - 10 * x[i];
        }

        #endregion
    }
}