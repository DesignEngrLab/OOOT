function [f]=genocop09(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of genocop09 from some
%aritcles,such as
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 9 
%
%algorithm:
%below code is  edited according to 
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 9 
%
%solution:
% n=3;lb=[0*ones(n,1)];ub=[10*ones(n,1)];
%The global minimum is x*=[1 0 0]'
%and f=-2.4714


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

if size(x,1)~=3,
    error('the length of x must be 3!');
end


x1=x(1,1);x2=x(2,1);x3=x(3,1);


f=(3*x1+x2-2*x3+0.8)/(2*x1-x2+x3)+(4*x1-2*x2+x3)/(7*x1+3*x2-x3);

f=-f;%maximize




