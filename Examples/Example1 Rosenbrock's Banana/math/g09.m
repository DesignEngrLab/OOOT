function [f]=g09(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g09 from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
% n=7;lb=-10*ones(n,1);ub=10*ones(n,1);
%The global minimum is x=[2.330499; 1.951372;-0.4775414;4.365726;-0.6244870; 1.038131; 1.594227];
%f=680.6300573;
%Two (out of four) constraints are active at the global optimum.

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

if size(x,1)~=7 
    error('the length of x must be 7!');
end

x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);x5=x(5,1);x6=x(6,1);
x7=x(7,1);

f=power(x1-10,2)+5*power(x2-12,2)+power(x3,4)+3*power(x4-11,2)+10*power(x5,6)+7*power(x6,2)+power(x7,4)+...
    -4*x6*x7-10*x6-8*x7;
