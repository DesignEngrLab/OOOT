function [f]=g07(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g07 from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
%n=10;lb=-10*ones(n,1);ub=10*ones(n,1);
%The global optimum is [2.171996; 2.363683; 8.773926; 5.095984; 0.9906548;
%1.430574; 1.321644; 9.828726; 8.280092; 8.375927];
%the global value is 24.3062091. Six (c1,c2,c3,c4,c5 and c6 ) constraints are active
%at the global optimum.

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

if size(x,1)~=10, 
    error('the length of x must be 10!');
end


x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);x5=x(5,1);x6=x(6,1);
x7=x(7,1);x8=x(8,1);x9=x(9,1);x10=x(10,1);


f=x1*x1+x2*x2+x1*x2-14*x1-16*x2+(x3-10)*(x3-10)+4*(x4-5)*(x4-5)+(x5-3)*(x5-3)+2*(x6-1)*(x6-1)+...
  5*x7*x7+7*(x8-11)*(x8-11)+ 2*(x9-10)*(x9-10)+(x10-7)*(x10-7)+45;
