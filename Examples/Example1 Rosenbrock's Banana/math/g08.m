function [f]=g08(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g08 from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%(2)S Koziel, Z Michalewicz  'Evolutionary algorithms, homomorphous mappings, and constrained parameter optimization. '
% Evol Comput, 1999 
%solution:
% n=2;lb=zeros(2,1);ub=10*ones(2,1);
%The global minimum is     x=[    1.2280 ;     4.2454];f=0.0958;


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

if size(x,1)~=2, 
    error('the length of x must be 2!');
end


x1=x(1,1);x2=x(2,1);


f=power(sin(2*pi*x1),3)*sin(2*pi*x2)/power(x1,3)/(x1+x2);

f=-f;