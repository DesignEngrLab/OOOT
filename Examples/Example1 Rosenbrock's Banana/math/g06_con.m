function [C,Ceq]=g06_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g06_con from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
% lb=[13;0];ub=[100;100];
%The global minimum is x=[14.095;0.84296];f=-6961.81388;
%Both constraints are active.

%Copyright:
% programmers:oiltowater.
% It comply with the GPL2.0
% Copyright 2006  oiltowater 

%change the row vector to a column vector.
if size(x,1)==1,
    x=x';
end

if size(x,1)~=2, 
    error('the length of x must be 2!');
end


x1=x(1,1);
x2=x(2,1);

C(1,1)=-(x1-5)*(x1-5)-(x2-5)*(x2-5)+100 ;
C(2,1)=(x1-6)*(x1-6)+(x2-5)*(x2-5)-82.81;




Ceq=0;
