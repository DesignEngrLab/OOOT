function [C,Ceq]=g05_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g05_con from some
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

C(1,1)=-x4+x3-0.55;
C(2,1)=x3+x4-0.55;




Ceq(1,1)=1000*sin(-x3-0.25)+1000*sin(-x4-0.25)+894.8-x1;
Ceq(2,1)=1000*sin(x3-0.25)+1000*sin(x3-x4-0.25)+894.8-x2;
Ceq(3,1)=1000*sin(x4-0.25)+1000*sin(x4-x3-0.25)+1294.8;
