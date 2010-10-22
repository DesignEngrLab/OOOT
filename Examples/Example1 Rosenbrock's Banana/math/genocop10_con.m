function [C,Ceq]=genocop10_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of genocop10_con from some
%aritcles,such as
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 10 
%
%algorithm:
%below code is  edited according to 
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 10 
%
%solution:
% n=4;lb=[0*ones(n,1)];ub=[3; 10; 10; 1];
%The global minimum is x*=[4/3 4 0 0];
%and f= 4.5142;
%our test can not get the same result.



%Copyright:
% programmers:oiltowater.
% It comply with the GPL2.0
% Copyright 2006  oiltowater 





%change the row vector to a column vector.
if size(x,1)==1,
    x=x';
end

if size(x,1)~=4,
    error('the length of x must be 4!');
end


x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);




Ceq(1,1)=-3*x1+x2-3*x3;

C(1,1)=x1+2*x3-4;
C(2,1)=x2+2*x4-4;



