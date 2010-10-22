function [f]=g04(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g04 from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
% lb=[78;33;27*ones(3,1)];ub=[102;45;45*ones(3,1)];
%the optimum solution (Himmelblau, 1992)
%is [78.0; 33.0; 29.995; 45.0;36.776],f=-30665.539;
%Two constraints areactive (c1 and c6)

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

if size(x,1)~=5, 
    error('the length of x must be 5!');
end


n=size(x,1);
x1=x(1,1);
x2=x(2,1);
x3=x(3,1);
x4=x(4,1);
x5=x(5,1);

f=5.3578547*x3*x3+0.8356891*x1*x5+37.293239*x1-40792.141;
