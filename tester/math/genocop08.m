function [f]=genocop08(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of genocop08 from some
%aritcles,such as
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 8 
%
%algorithm:
%below code is  edited according to 
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 8 
%
%solution:
% n=10;lb=[0.000001*ones(n,1)];ub=[1*ones(n,1)];
%The global minimum is [ 0.0407    0.1477    0.7831    0.0014    0.4853
%0.0007    0.0274    0.0180    0.0373    0.0968]'
%and f= -47.7611;

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

if size(x,1)~=10,
    error('the length of x must be 10!');
end


x1=x(1,1);x2=x(2,1);x3=x(3,1);x4=x(4,1);x5=x(5,1);
x6=x(6,1);x7=x(7,1);x8=x(8,1);x9=x(9,1);x10=x(10,1);
c=[-6.089 -17.164 -34.054 -5.914 -24.721 -14.986 -24.100 -10.708 -26.662 -22.179];
c=c';

sumx=sum(x);
f=sum(x.*(c+log(x/sumx)));



