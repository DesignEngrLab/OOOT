function [C,Ceq]=g01_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g01_con from some
%aritcles,such as
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



%change the row vector to a column vector.
if size(x,1)==1,
    x=x';
end

if size(x,1)~=13 
    error('the length of x must be 13!');
end

x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);x5=x(5,1);x6=x(6,1);
x7=x(7,1);x8=x(8,1);x9=x(9,1);x10=x(10,1);x11=x(11,1);x12=x(12,1);x13=x(13,1);
C(1,1)=2*x1+2*x2+x10+x11-10;
C(2,1)=2*x1+2*x3+x10+x12-10;
C(3,1)=2*x2+2*x3+x11+x12-10;
C(4,1)=-8*x1+x10;
C(5,1)=-8*x2+x11;
C(6,1)=-8*x3+x12;
C(7,1)=-2*x4-x5+x10;
C(8,1)=-2*x6-x7+x11;
C(9,1)=-2*x8-x9+x12;
Ceq=0;%if there no equation.