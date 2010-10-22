function [f]=g05(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g05 from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
% lb=[0;0;-0.55;-0.55];ub=[1200;1200;0.55;0.55];
%The best known solution is x=[679.9453;1026.067;0.1188764;-0.3962336];
%f=5126.4981;

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

if size(x,1)~=4, 
    error('the length of x must be 4!');
end



x1=x(1,1);
x2=x(2,1);
x3=x(3,1);
x4=x(4,1);
f=3*x1+0.000001*x1*x1*x1+2*x2+(0.000002/3)*x2*x2*x2;
