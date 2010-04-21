function [f]=g11(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g11 from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
% n=2;lb=-1*ones(n,1);ub=1*ones(n,1);
%The known global solutions are x = [+0.70711; 0.5] or x = [-0.70711;0.5] 
%,and f = 0.75000455.

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

if size(x,1)~=2 
    error('the length of x must be 2!');
end



x1=x(1,1);x2=x(2,1);

f=power(x1,2)+power(x2-1,2);
