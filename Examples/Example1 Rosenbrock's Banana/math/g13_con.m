function [C,Ceq]=g13_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g13_con from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
% n=5;lb=[-2.3*ones(2,1);-3.2*ones(3,1)];ub=[2.3*ones(2,1);3.2*ones(3,1)];
%x=  [-1.7171    1.5957    1.8272   -0.7636   -0.7636]'
%f=    0.0539; 

%Copyright:
% programmers:oiltowater.
% It comply with the GPL2.0
% Copyright 2006  oiltowater 



%change the row vector to a column vector.
if size(x,1)==1,
    x=x';
end

if size(x,1)~=5 
    error('the length of x must be 5!');
end

x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);x5=x(5,1);


C=0;
Ceq(1,1)=sum(x.*x)-10;
Ceq(2,1)=x2*x3-5*x4*x5;
Ceq(3,1)=power(x1,3)+power(x2,3)+1;

