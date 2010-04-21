function [C,Ceq]=g04_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g04_con from some
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
c1=(85.334407+0.0056858*x2*x5+0.0006262*x1*x4-0.0022053*x3*x5);
c2=(80.51249+0.0071317*x2*x5+0.0029955*x1*x2+0.0021813*x3*x3);
c3=(9.300961+0.0047026*x3*x5+0.0012547*x1*x3+0.0019085*x3*x4);

C(1,1)=c1-92;
C(2,1)=0-c1;
C(3,1)=c2-110;
C(4,1)=90-c2;
C(5,1)=c3-25;
C(6,1)=20-c3;

Ceq(1,1)=0;%if there no equation.
