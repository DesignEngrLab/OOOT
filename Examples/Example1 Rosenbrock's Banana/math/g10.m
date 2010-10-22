function [f]=g10(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g10 from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
% n=8;lb=[100 ;1000*ones(2,1);10*ones(5,1)];ub=[10000;10000*ones(2,1);1000*ones(5,1)];
%the function g10 is linear and has its
%global minimum at x = [579.3167; 1359.943; 5110.071; 182.0174; 295.5985;
%217.9799; 286.4162; 395.5979];
%f = 7049.330923. All six constraints are active at the global
%optimum.

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

if size(x,1)~=8 
    error('the length of x must be 8!');
end




x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);x5=x(5,1);x6=x(6,1);
x7=x(7,1);x8=x(8,1);

f=x1+x2+x3;
