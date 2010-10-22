function [C,Ceq]=g10_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g10_con from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
% n=8;lb=[100;1000*ones(2,1);10*ones(5,1)];ub=[10000;10000*ones(2,1);1000*ones(5,1)];
%the function g10 is linear and has its
%global minimum at x = [579.3167; 1359.943; 5110.071; 182.0174; 295.5985;
%217.9799; 286.4162; 395.5979];
%f = 7049.330923. All six constraints are active at the global
%optimum.

%Copyright:
% programmers:oiltowater.
% It comply with the GPL2.0
% Copyright 2006  oiltowater 


%change the row vector to a column vector.
if size(x,1)==1,
    x=x';
end

if size(x,1)~=8 
    error('the length of x must be 8!');
end



x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);x5=x(5,1);x6=x(6,1);
x7=x(7,1);x8=x(8,1);

C(1,1)=1-0.0025*(x4+x6);
C(2,1)=1-0.0025*(x5+x7-x4);
C(3,1)=1-0.01*(x8-x5);
C(4,1)=x1*x6-833.33252*x4-100*x1+83333.333;
C(5,1)=x2*x7-1250*x5-x2*x4+1250*x4;
C(6,1)=x3*x8-1250000-x3*x5+2500*x5;
C=-C;

Ceq=0;%if there no equation.