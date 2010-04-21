function [C,Ceq]=genocop07_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of genocop07_con from some
%aritcles,such as
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 7 
%
%algorithm:
%below code is  edited according to 
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 7 
%
%solution:
% n=6;lb=[0*ones(5,1);0];ub=[1*ones(5,1);100];
%The global minimum is [0; 1; 0; 1; 1;20].
%and f=-213;


%Copyright:
% programmers:oiltowater.
% It comply with the GPL2.0
% Copyright 2006  oiltowater 





%change the row vector to a column vector.
if size(x,1)==1,
    x=x';
end

if size(x,1)~=6 
    error('the length of x is 6');
end


x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);x5=x(5,1);
y=x(6,1);

C(1,1)=6*x1+3*x2+3*x3+2*x4+x5-6.5;
C(2,1)=10*x1+10*x3+y-20;

Ceq=0;%if there no equation.

