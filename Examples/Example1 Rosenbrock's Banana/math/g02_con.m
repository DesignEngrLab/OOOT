function [C,Ceq]=g02_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g02_con from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
%n=20;lb=zeros(n,1);ub=10*ones(n,1);
%The global minimum is £¿

%Copyright:
% programmers:oiltowater.
% It comply with the GPL2.0
% Copyright 2006  oiltowater 


%change the row vector to a column vector.
if size(x,1)==1,
    x=x';
end


n=size(x,1);
C(1,1)=0.75-prod(x);
C(2,1)=sum(x)-7.5*n;

Ceq=0;%if there no equation.
