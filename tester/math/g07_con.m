function [C,Ceq]=g07_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of g07_con from some
%aritcles,such as
%(1)TP Runarsson, X Yao 'Stochastic Ranking for Constrained Evolutionary Optimization'
% IEEE TRANSACTIONS ON EVOLUTIONARY COMPUTATION, 2000 
%
%solution:
%n=10;lb=-10*ones(n,1);ub=10*ones(n,1);
%The global optimum is [2.171996; 2.363683; 8.773926; 5.095984; 0.9906548;
%1.430574; 1.321644; 9.828726; 8.280092; 8.375927];
%the global value is 24.3062091. Six (c1,c2,c3,c4,c5 and c6 ) constraints are active
%at the global optimum.

%Copyright:
% programmers:oiltowater.
% It comply with the GPL2.0
% Copyright 2006  oiltowater 

%change the row vector to a column vector.
if size(x,1)==1,
    x=x';
end

if size(x,1)~=10, 
    error('the length of x must be 10!');
end


x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);x5=x(5,1);x6=x(6,1);
x7=x(7,1);x8=x(8,1);x9=x(9,1);x10=x(10,1);

C(1,1)=105-4*x1-5*x2+3*x7-9*x8;
C(2,1)=-10*x1+8*x2+17*x7-2*x8;
C(3,1)=8*x1-2*x2-5*x9+2*x10+12;
C(4,1)=-3*(x1-2)*(x1-2)-4*(x2-3)*(x2-3)-2*x3*x3+120+7*x4;
C(5,1)=-5*x1*x1-8*x2-(x3-6)*(x3-6)+2*x4+40;   
C(6,1)=-x1*x1-2*(x2-2)*(x2-2)+2*x1*x2-14*x5+6*x6;
C(7,1)=-0.5*(x1-8)*(x1-8)-2*(x2-4)*(x2-4)-3*x5*x5+x6+30;
C(8,1)=3*x1-6*x2-12*(x9-8)*(x9-8)+7*x10;
%note that above C>=0;
C=-C;
Ceq=0;%if there no equation.