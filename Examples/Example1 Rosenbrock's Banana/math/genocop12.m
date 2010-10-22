function [f]=genocop12(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of genocop12 from some
%aritcles,such as
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 12 
%
%algorithm:
%below code is  edited according to 
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 12 
%
%solution:
% n=6;lb=[0;0];ub=[6;100;];
%The global minimum is [0 0]',[3 sqrt(3)]'and [4 0]'
%and f=-1


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

if size(x,1)~=2,
    error('the length of x must be 2!');
end


x1=x(1,1);x2=x(2,1);

f=1000; %be enough large!

if (x1>=0) && (x1<2)  ,
f=x2+power(10,-5)*power(x2-x1,2)-1.0;
end

if (x1>=2) && (x1<4),
 f=1/27/sqrt(3)*(power(x1-3,2)-9)*power(x2,3);
end
     
if (x1>=4) && (x1<=6),
 f=1/3*power(x1-2,3)+x2-11/3;
end


