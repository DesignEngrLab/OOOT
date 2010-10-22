function [f]=g12(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g12 from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
% n=3;lb=zeros(n,1);ub=10*ones(n,1);
%The feasible region of the search space consists of  9^3 disjointed
%spheres. 
%The optimum is located at x= (5,5,5) where f = -1. 
%The solution lies within  the feasible region.

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

if size(x,1)~=3 
    error('the length of x must be 3!');
end



x1=x(1,1);x2=x(2,1);x3=x(3,1);

f=(100-power(x1-5,2)-power(x2-5,2)-power(x3-5,2))/100;
f=-f;